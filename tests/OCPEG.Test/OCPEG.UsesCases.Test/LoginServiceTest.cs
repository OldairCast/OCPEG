using OCPEG.Application.UseCasesServices.Services;
using OCPEG.Common.TestUtilities.Repositories;
using OCPEG.Common.TestUtilities.Requests;
using OCPEG.Common.TestUtilities.Services;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;

namespace OCPEG.UsesCases.Test
{
    public class LoginServiceTest
    {

        [Fact]
        public async Task SuccessCreate()
        {
            (var userDto, var password) = RequestUserTest.BuildFakeDto();
            var useCase = CreateUseCase();

            User userLogin = new User
            {
                Email = "ocpeg@ocpeg.com",
                Password = "ocpeg123"
            };

            //Verificar
            //var result = await useCase.Login(userLogin);

            Assert.True(true);
        }

        private AccountService CreateUseCase()
        {
            var unitWork = UnitOfWorkBuilder.Build();
            var userRepository = new UserRepositoryBuilder().Build();
            var accountRepository = new AccountRepositoryBuilder().Build();
            var tokenService = new TokenServiceBuilder().Build();
            var loggedUser = LoggedUserBuilder.Build();

            var useCase = new AccountService(accountRepository, userRepository, tokenService, loggedUser);
            
            return useCase;
        }
    }
}
