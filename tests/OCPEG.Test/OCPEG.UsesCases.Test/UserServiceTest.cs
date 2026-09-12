using AutoMapper;
using OCPEG.Application.UseCasesServices.Services;
using OCPEG.Common.TestUtilities.Repositories;
using OCPEG.Common.TestUtilities.Requests;
using OCPEG.Common.TestUtilities.Services;
using OCPEG.Domain.BusinessObject;
using OCPEG.Framework;

namespace OCPEG.UsesCases.Test
{
    public class UserServiceTest
    {
        [Fact]
        public async Task SuccessCreate()
        {
            var request = RequestUserTest.Build();
            var useCase = CreateUseCase();

            var result = await useCase.Create(request);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task SuccessDelete()
        {
            //Adiciona referencia via nugget => moq
            //passa para ele uma interface e ele devolve uma implementação fake da nossa interface

            var request = RequestUserTest.Build();
            var useCase = CreateUseCase();
            var result = await useCase.Delete(request.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task GetByIdTest()
        {
            //Adiciona referencia via nugget => moq
            //passa para ele uma interface e ele devolve uma implementação fake da nossa interface

            var request = RequestUserTest.Build();
            var useCase = CreateUseCaseParams(request);
            
            //Modo 1
            var result = await useCase.GetById(request.Id);

            //Modo 2 - Salvando uma função dentro de uma váriavel act. e essa função é que vai executar o usecase
            Func<Task> act = async () => await useCase.GetById(request.Id);
            //Aqui é a validação que precisaria ser aprimorada
            act.Equals(Enums.ResponseResultStatus.Success);
            //Modo 2

            //Modo 1
            Assert.NotNull(result);
        }

        //public async Task act()
        //{
        //    await useCase.GetByCode(request.Code);
        //}

        #region CreateUseCase
        private UserService CreateUseCase()
        {
            var unitWork = UnitOfWorkBuilder.Build();
            var repository = new UserRepositoryBuilder().Build();
            var mapper = MapperBuilder.Build();

            var useCase = new UserService(repository, unitWork, mapper);

            return useCase;
        }

        private static UserService CreateUseCaseParams(User user)
        {
            var request = RequestUserTest.Build();
            var unitWork = UnitOfWorkBuilder.Build();
            var repositoryBuilder = new UserRepositoryBuilder();
            var mapper = MapperBuilder.Build();

            if (user.Id == 0)
                repositoryBuilder.GetById(user);

            var useCase = new UserService(repositoryBuilder.Build(), unitWork, mapper);

            return useCase;

        }
        #endregion
    }
}
