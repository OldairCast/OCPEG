using AutoMapper;
using Microsoft.Extensions.Logging;
using OCEPG.Infrastructure.Mappings;

namespace OCPEG.Common.TestUtilities.Services
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var idEncripter = IdEncripterBuilder.Build();
            var idlEncripter = IdlEncripterBuilder.Build();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                // opcional: configurar logging
                //builder.AddConsole();
            });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OcpegMappingProfile>();
                cfg.AddProfile(new ComplexMappingProfile(idEncripter, idlEncripter));
            }, loggerFactory);

            return config.CreateMapper();
        }
    }
}
