using Moq;
using OCPEG.Application.UseCasesServices.Interfaces;

namespace OCPEG.Common.TestUtilities.Repositories
{
    public class LoggedUserBuilder
    {
        public static ILoggedUser Build()
        {
            var mock = new Mock<ILoggedUser>();

            return mock.Object;
        }
    }
}
