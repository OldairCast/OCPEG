using Moq;
using OCPEG.Domain.DataAccess.Base;

namespace OCPEG.Common.TestUtilities.Repositories
{
    public class UnitOfWorkBuilder
    {
        //Forma de construção quando a função não retorna valor
        public static IUnitOfWork Build()
        {
            var mock = new Mock<IUnitOfWork>();

            return mock.Object;
        }
    }
}
