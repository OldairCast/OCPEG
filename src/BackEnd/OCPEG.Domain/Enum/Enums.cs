using System.ComponentModel;

namespace OCPEG.Domain.Enum
{
    public class Enums
    {
        public enum UserRolesEn : byte
        {
            [Description("None")]
            None = 0,

            [Description("Admin")]
            Admin = 1,

            [Description("User")]
            User = 2
        }


        public enum StatusEn : byte
        {
            [Description("None")]
            None = 0,

            [Description("Aberto")]
            Aberto = 1,

            [Description("Fechado")]
            Fechado = 2
        }

        public enum PriorityEn : byte
        {
            [Description("None")]
            None = 0,

            [Description("Low")]
            Baixa = 1,

            [Description("Media")]
            Media = 2,

            [Description("High")]
            Alta = 3
        }

        public enum FunctionEn : byte
        {
            [Description("NaoInformado")]
            None = 0,

            [Description("Participante")]
            Participante = 1,

            [Description("Analista")]
            Analista = 2
        }

        public enum DatabaseTypeEn
        {
            None = 0,
            SqlServer = 1,
            MySql = 2,
            Oracle = 3,
            Postgres = 4
        }
    }
}
