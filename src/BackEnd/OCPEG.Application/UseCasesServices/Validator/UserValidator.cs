using FluentValidation;
using OCPEG.Domain.BusinessObject;

using OCPEG.Framework.Resource;

namespace OCPEG.Application.UseCasesServices.Validator
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(bool pwd)
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(OcPegResource.NAME_EMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(OcPegResource.EMAIL_EMPTY);
            
            When(user => string.IsNullOrEmpty(user.Email), () =>
            {
                RuleFor(user => user.PasswordHash!).SetValidator(new PasswordValidator<User>());
                RuleFor(user => user.PasswordHash).NotNull().MinimumLength(6).WithMessage("Senha inválida");
            });
            
            When(user => string.IsNullOrEmpty(user.Email), () =>
            {
                RuleFor(user => user.Email).EmailAddress().WithMessage(OcPegResource.EMAIL_INVALID);
            });
        }
    }
}
