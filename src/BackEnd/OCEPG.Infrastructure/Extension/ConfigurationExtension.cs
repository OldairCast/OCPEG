using Microsoft.Extensions.Configuration;
using OCPEG.Domain.Enum;
using static OCPEG.Domain.Enum.Enums;

namespace OCEPG.Infrastructure.Extension
{
    public static class ConfigurationExtension
    {
        public static bool IsUnitTestEnviroment(this IConfiguration configurarion) => configurarion.GetValue<bool>("InMemoryTest");

        public static DatabaseTypeEn DatabaseType(this IConfiguration configurarion)
        {
            var databaseType = configurarion.GetConnectionString("DatabaseType");

            return (DatabaseTypeEn)Enum.Parse(typeof(DatabaseTypeEn), databaseType!);
        }

        public static string ConnectionString(this IConfiguration configurarion)
        {
            var databaseType = configurarion.DatabaseType();

            if (databaseType == Enums.DatabaseTypeEn.MySql)
                return configurarion.GetConnectionString("ConnectionMySQLServer")!;
            else if (databaseType == Enums.DatabaseTypeEn.Oracle)
                return configurarion.GetConnectionString("ConnectionOracleServer")!;
            else if (databaseType == Enums.DatabaseTypeEn.Postgres)
                return configurarion.GetConnectionString("ConnectionPostgres")!;
            else
                return configurarion.GetConnectionString("ConnectionSQLServer")!;
        }
    }
}
