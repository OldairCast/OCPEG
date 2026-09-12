
using System;

namespace OCPEG.Framework
{
    public sealed class Constants
    {
        private Constants()
        {
        }

        public const string OperacaoRealizadaSucesso = "Operação Realizada com Sucesso";
        public const string OcorreuErro = "Ocorreu um erro! Consulte o Administrador.";
        public const string UserMain = "OCPEG";
        public const string JwtConfig = "JwtConfig";
        public const string SecretKey = "SecretKey";
        public const string Bearer = "Bearer";
        public const string BearerDescription = "JWT Authorization header usando Bearer.\r\nEntre com 'Bearer ' [espaço] então coloque seu token.\r\n                                Exemplo: 'Bearer 12345abcdef'";

        public const string PasswordMain = GetEncryptedPass;
        public const string PasswordDefault = GetEncryptedPassDef;
        private const string GetEncryptedPass = "ocpeg123";
        private const string GetEncryptedPassDef = "123Mudar";

        public const string Inicio = "Inicio";
        public const string Fim = "Fim";
        public const string CHAT_MODEL = "gpt-5-nano";

    }
}