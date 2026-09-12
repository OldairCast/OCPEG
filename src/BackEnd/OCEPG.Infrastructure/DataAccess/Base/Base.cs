using OCPEG.Framework.Exception;
using System.Data;
using System.Data.Common;

namespace OCEPG.Infrastructure.DataAccess.Base
{
    public static class DbTools
    {

        /// <summary>
        /// Função que tem por objetivo carregar o valor de um campo
        /// de um recordset baseado no nome de campo informado em fieldName
        /// Caso o nome do campo não seja encontrado, será retornado um valor padrão
        /// </summary>
        public static T? GetValue<T>(string fieldName, IDataRecord record)
        {
            try
            {
                if (record.HasColumn(fieldName))
                {
                    var fieldPos = record.GetOrdinal(fieldName);
                    var check = CheckDbNull<T>(record.GetValue(fieldPos));
                    return check;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return default(T);
            }
            return default(T);
        }

        //variável de delegação, define qual a função a ser usada
        private delegate T CheckDbNullDelegate<out T>(object dataRecord);

        /// <summary>
        /// Função com o objetivo de tratar valores null e retornar valores 
        /// base definidos da variável genérica T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public static T? CheckDbNull<T>(object? dataRecord)
        {
            if (dataRecord == null || dataRecord == DBNull.Value)
            {
                // Retorna default (null para tipos referência / Nullable<T>)
                return default;
            }

            Delegate d = typeof(T).Name.ToLower() switch
            {
                "string" => new CheckDbNullDelegate<string>(DbNullToString),
                "boolean" => new CheckDbNullDelegate<bool>(DbNullToBool),
                "datetime" => new CheckDbNullDelegate<DateTime>(DbNullToDateTime),
                "short" or "int16" => new CheckDbNullDelegate<short>(DbNullToShort),
                "byte" => new CheckDbNullDelegate<byte>(DbNullToByte),
                "int" or "int32" => new CheckDbNullDelegate<int>(DbNullToInt),
                "double" => new CheckDbNullDelegate<double>(DbNullToDouble),
                "char" => new CheckDbNullDelegate<char>(DbNullToChar),
                "long" or "int64" => new CheckDbNullDelegate<long>(DbNullToInt64),
                "timespan" => new CheckDbNullDelegate<TimeSpan>(DbNullToTimeSpan),
                "decimal" => new CheckDbNullDelegate<decimal>(DbNullToDecimal),
                _ => throw new InvalidOperationException($"Tipo inválido <T>: {typeof(T).Name}")
            };

            object? result = d.DynamicInvoke(dataRecord);
            return (T?)result;
        }

        /// <summary>
        /// Converte DbNull para o tipo bool
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna false senão o valor definido em dataRecord</returns>
        private static bool DbNullToBool(object dataRecord)
        {
            return Convert.ToBoolean(dataRecord == DBNull.Value ? false : dataRecord);
        }

        /// <summary>
        /// Converte DbNull para o tipo short
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna 0 senão o valor definido em dataRecord</returns>
        private static short DbNullToShort(object dataRecord)
        {
            return Convert.ToInt16(dataRecord == DBNull.Value ? 0 : dataRecord);
        }

        /// <summary>
        /// Converte DbNull para o tipo byte
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna false senão o valor definido em dataRecord</returns>
        private static byte DbNullToByte(object dataRecord)
        {
            return Convert.ToByte(dataRecord == DBNull.Value ? 0 : dataRecord);
        }

        /// <summary>
        /// Converte DbNull para o tipo int
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna 0 senão o valor definido em dataRecord</returns>
        private static int DbNullToInt(object dataRecord)
        {
            return Convert.ToInt32(dataRecord == DBNull.Value ? 0 : dataRecord);
        }

        /// <summary>
        /// Converte DbNull para o tipo string
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna string vazia senão o valor definido em dataRecord</returns>
        private static string DbNullToString(object dataRecord)
        {
            if (dataRecord == null || dataRecord == DBNull.Value)
                return "";

            return (Convert.ToString(dataRecord) ?? "").Trim();
        }

        /// <summary>
        /// Converte DbNull para o tipo double
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna 0 senão o valor definido em dataRecord</returns>
        private static double DbNullToDouble(object dataRecord)
        {
            return Convert.ToDouble(dataRecord == DBNull.Value ? 0 : dataRecord);
        }

        /// <summary>
        /// Converte DbNull para o tipo datetime
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns>Se null retorna DateTime.MinValue senão o valor definido em dataRecord</returns>
        private static DateTime DbNullToDateTime(object dataRecord)
        {
            return Convert.ToDateTime(dataRecord == DBNull.Value ? DateTime.MinValue : dataRecord);
        }

        /// <summary>
        /// Converte DbNull para o tipo char
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna ' ' senão o valor definido em dataRecord</returns>
        private static char DbNullToChar(object dataRecord)
        {
            return Convert.ToChar(dataRecord == DBNull.Value ? ' ' : dataRecord);
        }

        private static long DbNullToInt64(object dataRecord)
        {
            return Convert.ToInt64(dataRecord == DBNull.Value ? ' ' : dataRecord);
        }

        private static TimeSpan DbNullToTimeSpan(object dataRecord)
        {

            if (dataRecord == DBNull.Value)
                return new TimeSpan();

            return (TimeSpan)dataRecord;

        }

        /// <summary>
        /// Converte DbNull para o tipo double
        /// </summary>
        /// <param name="dataRecord">variável que contem o valor a ser convertido, representa uma coluna do recordset</param>
        /// <returns>Se null retorna 0 senão o valor definido em dataRecord</returns>
        private static decimal DbNullToDecimal(object dataRecord)
        {
            return Convert.ToDecimal(dataRecord == DBNull.Value ? 0 : dataRecord);
        }


        public static async Task<string> GetStringAsync(DbDataReader rdr, int index)
        {
            return await rdr.IsDBNullAsync(index) ? string.Empty : rdr.GetString(index);
        }

        public static async Task<long> GetInt64Async(DbDataReader rdr, int index)
        {
            return await rdr.IsDBNullAsync(index) ? 0 : rdr.GetInt64(index);
        }

        public static async Task<short> GetInt16Async(DbDataReader rdr, int index)
        {
            return (short)(await rdr.IsDBNullAsync(index) ? 0 : rdr.GetInt16(index));
        }

        public static async Task<int> GetInt32Async(DbDataReader rdr, int index)
        {
            return await rdr.IsDBNullAsync(index) ? 0 : rdr.GetInt32(index);
        }

        public static async Task<DateTime> GetDateTimeAsync(DbDataReader rdr, int index)
        {
            return await rdr.IsDBNullAsync(index) ? DateTime.MinValue : rdr.GetDateTime(index);
        }

        public static async Task<decimal> GetDecimalAsync(DbDataReader rdr, int index)
        {
            return await rdr.IsDBNullAsync(index) ? 0 : rdr.GetDecimal(index);
        }




    }

    public static class DataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
