using Sqids;

namespace OCPEG.Common.TestUtilities.Services
{
    public class IdEncripterBuilder
    {
        public static SqidsEncoder<int> Build()
        {
            return new SqidsEncoder<int>(new()
            {
                MinLength = 3,
                Alphabet = "achIugtW19s7vA4ldomHjULNFYbery0EpTMxkBiQ6qJ2SKXZG35Cz8RDfnPOVw"
            });
        }
    }
}
