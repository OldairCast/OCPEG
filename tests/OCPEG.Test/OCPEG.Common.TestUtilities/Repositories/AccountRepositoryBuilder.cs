using Moq;
using OCPEG.Domain.DataAccess.Interfaces;

namespace OCPEG.Common.TestUtilities.Repositories
{
    public class AccountRepositoryBuilder
    {
        private readonly Mock<IAccountRepository> _repository;

        //construtor da classe
        public AccountRepositoryBuilder() => _repository = new Mock<IAccountRepository>();

        //Construtor
        public IAccountRepository Build() => _repository.Object;

    }

}
