using OCPEG.Application.UseCasesServices.Validator;
using OCPEG.Common.TestUtilities.Requests;
using OCPEG.Domain.BusinessObject;


namespace OCPEG.Validators.Test
{
    public class UserValidatorTest
    {
        [Fact]
        public void SuccessOrigem()
        {
            var validator = new UserValidator(true);

            var request = new User
            {
                Name = "Jose",
                Email = "jose@jose.com",
                PasswordHash = "1234567"
            };

            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Success()
        {
            var validator = new UserValidator(true);

            var request = RequestUserTest.Build();

            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var validator = new UserValidator(true);

            var request = RequestUserTest.Build();
            request.Name = string.Empty;

            var result = validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory] // Executa o teste varias vezes como em looping
        [InlineData(1)]  // Cada item é um Parametro a ser passsado
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Password_Invalid(int passwordLength)
        {
            var validator = new UserValidator(true);

            var request = RequestUserTest.Build(passwordLength);

            var result = validator.Validate(request);

            Assert.True(result.IsValid);
        }

    }
}
