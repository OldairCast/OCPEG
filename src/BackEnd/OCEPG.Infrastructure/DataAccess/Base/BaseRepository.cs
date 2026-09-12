using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static OCPEG.Domain.Enum.Enums;

namespace OCEPG.Infrastructure.DataAccess.Base
{
    public abstract class BaseRepository
    {
        protected readonly IMapper _mapper;

        protected BaseRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Retorna a string de conexão para uso em conexões ADO
        /// </summary>
        protected static string ConnectionString
        {
            get
            {
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true);

                var root = builder.Build();
                string? connectionString = "";

                var databaseType = root.GetConnectionString("DatabaseType");

                if (databaseType == Convert.ToByte(DatabaseTypeEn.SqlServer).ToString())
                {
                    connectionString = root.GetConnectionString("ConnectionSQLServer") == null ? "" : root.GetConnectionString("ConnectionSQLServer");
                }
                else if (databaseType == Convert.ToByte(DatabaseTypeEn.MySql).ToString())
                {
                    connectionString = root.GetConnectionString("ConnectionMySQLServer") == null ? "" : root.GetConnectionString("ConnectionMySQLServer");
                }
                else if(databaseType == Convert.ToByte(DatabaseTypeEn.Oracle).ToString())
                {
                    connectionString = root.GetConnectionString("ConnectionOracleServer") == null ? "" : root.GetConnectionString("ConnectionOracleServer");
                }
                else if (databaseType == Convert.ToByte(DatabaseTypeEn.Postgres).ToString())
                {
                    connectionString = root.GetConnectionString("ConnectionPostgres") == null ? "" : root.GetConnectionString("ConnectionPostgres");
                }

                connectionString = string.IsNullOrEmpty(connectionString) ? string.Empty: connectionString;
                return connectionString;
            }
        }

    }

}
