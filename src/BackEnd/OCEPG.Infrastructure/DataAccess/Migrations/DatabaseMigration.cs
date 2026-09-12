using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Framework;
using System.Text.RegularExpressions;
using static OCPEG.Domain.Enum.Enums;


namespace OCEPG.Infrastructure.DataAccess.Migrations;

public static class DatabaseMigration
{
    public static void Migrate(DatabaseTypeEn databaseType, string connectionString, IServiceProvider serviceProvider)
    {
        if (databaseType == DatabaseTypeEn.MySql)
            EnsureDatabaseCreated_MySql(connectionString);
        else
            EnsureDatabaseCreated_SqlServer(connectionString);

        MigrationDatabase(serviceProvider);
    }

    private static void EnsureDatabaseCreated_MySql(string connectionString)
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);

        var databaseName = connectionStringBuilder.Database;

        connectionStringBuilder.Remove("Database");

        using var dbConnection = new MySqlConnection(connectionStringBuilder.ConnectionString);

        // Validação de entrada — apenas letras, números e underscore
        if (string.IsNullOrWhiteSpace(databaseName) ||
            !Regex.IsMatch(databaseName, @"^[a-zA-Z0-9_]+$"))
        {
            throw new ArgumentException("Nome de banco de dados inválido.");
        }

        dbConnection.Open();
        var parameters = new DynamicParameters();
        parameters.Add("@name", databaseName);

        var exists = dbConnection.QueryFirstOrDefault<int>(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name",
            parameters
        );

        if (exists == 0)
        {
            // Evita injeção concatenando diretamente — mas aqui não há parâmetro suportado pelo MySQL
            // então validamos e escapamos corretamente o nome
            var safeDatabaseName = $"`{databaseName.Replace("`", "``")}`"; // escapa acentos graves

            // Evitar SQL Injection usando interpolação segura
            if (!Regex.IsMatch(safeDatabaseName, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException("Invalid database name");
            }

            var safeName = databaseName.Replace("]", "]]");
            var createCommand = $"CREATE DATABASE [{safeName}]";
            dbConnection.Execute(createCommand);
        }
    }

    private static void EnsureDatabaseCreated_SqlServer(string connectionString)
    {
        try
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            var databaseName = connectionStringBuilder.InitialCatalog;

            // Remover apenas o nome do banco, conectando-se ao master
            connectionStringBuilder.InitialCatalog = "master";

            using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);
            dbConnection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@name", databaseName);

            var exists = dbConnection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM sys.databases WHERE name = @name",
                parameters
            ) > 0;

            if (!exists)
            {
                // Evitar SQL Injection usando interpolação segura
                if (!Regex.IsMatch(databaseName, @"^[a-zA-Z0-9_]+$"))
                {
                    throw new ArgumentException("Invalid database name");
                }

                var safeName = databaseName.Replace("]", "]]");
                var sql = $"CREATE DATABASE [{safeName}]";
                dbConnection.Execute(sql);
            }
        }
        catch (Exception ex){
            NLogManager.LogError($"{ex}");
        }
    }

    private async static void MigrationDatabase(IServiceProvider serviceProvider)
    {
        try
        {
            using var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            var lastAppliedMigration = (await dbContext.Database.GetAppliedMigrationsAsync());

            if (lastAppliedMigration.Any())
            {
                foreach (var last in pendingMigrations)
                {
                    Console.WriteLine($"- {last}");
                }
            }

            if (pendingMigrations.Any())
            {
                Console.WriteLine($"You have {pendingMigrations.Count()} pending migrations to apply.");
                Console.WriteLine("Applying pending migrations now");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"- {migration}");
                }

                await dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }
}
