using FluentValidation.Validators;
using FluentValidation;
using OCPEG.Framework.Resource;

namespace OCPEG.Application.UseCasesServices.Validator
{
    public class PasswordValidator<T> : PropertyValidator<T, string>
    {
        public override bool IsValid(ValidationContext<T> context, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                context.MessageFormatter.AppendArgument("ErrorMessage", OcPegResource.PASSWORD_EMPTY);

                return false;
            }

            if (password.Length < 6)
            {
                context.MessageFormatter.AppendArgument("ErrorMessage", OcPegResource.INVALID_PASSWORD);

                return false;
            }

            return true;
        }

        public override string Name => "PasswordValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{ErrorMessage}";
    }

}
