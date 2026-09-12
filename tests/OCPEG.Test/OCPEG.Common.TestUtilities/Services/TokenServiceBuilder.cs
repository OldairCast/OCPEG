using Moq;
using OCPEG.Application.UseCasesServices.Interfaces;

namespace OCPEG.Common.TestUtilities.Services
{
    public class TokenServiceBuilder
    {
        private readonly Mock<ITokenService> _mock;

        public TokenServiceBuilder() => _mock = new Mock<ITokenService>();

        public ITokenService Build() => _mock.Object;

    }
}
