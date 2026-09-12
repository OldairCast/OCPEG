using Moq;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;

namespace OCPEG.Common.TestUtilities.Repositories
{
    public class UserRepositoryBuilder
    {
        private readonly Mock<IUserRepository> _repository;

        //construtor da classe
        public UserRepositoryBuilder() => _repository = new Mock<IUserRepository>();

        //Construtor
        public IUserRepository Build() => _repository.Object;

        //public IUserRepository Build()
        //{
        //    return _repository.Object;
        //}
        //Construtor

        public void GetById(User user)
        {
            //Setup Função do Mock que vai dar acesso as funções da interface.
            //Quando alguem chamar essas funções da interface ela vai retornar isso como resposta fazendo isso aqui.
             _repository.Setup(repository => repository.GetById(user.Id)).ReturnsAsync(user);
        }

        public void GetByEmail(User user)
        {
            //Setup Função do Mock que vai dar acesso as funções da interface.
            //Quando alguem chamar essas funções da interface ela vai retornar isso como resposta fazendo isso aqui.
            string email = user.Email ?? string.Empty;
            _repository.Setup(repository => repository.GetByEmail(email)).ReturnsAsync(user);
        }


        //Forma de construção quando a função não retorna valor
        //public static IUserRepository Delete()
        //{
        //    var mock = new Mock<IUserRepository>();

        //    return mock.Object;
        //}


    }
}
