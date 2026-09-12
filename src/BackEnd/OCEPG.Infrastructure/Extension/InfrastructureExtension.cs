using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OCEPG.Infrastructure.DataAccess.Base;
using OCEPG.Infrastructure.DataAccess.Repositories;
using OCEPG.Infrastructure.Messages.RabbitMQSender;
using OCEPG.Infrastructure.Services.LoggedUser;
using OCEPG.Infrastructure.Services.OpenAI;
using OCEPG.Infrastructure.Services.Storage;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.OpenAI;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Domain.DataAccess.Base;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Services.OpenAI;
using OCPEG.Framework;
using OpenAI.Chat;
using static OCPEG.Domain.Enum.Enums;

namespace OCEPG.Infrastructure.Extension
{
    public static class InfrastructureExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepository(services);
            AddLoggedUser(services);
            AddOpenAI(services, configuration);
            AddAzureStorage(services, configuration);
            AddQueue(services, configuration);

            if (configuration.IsUnitTestEnviroment())
                return;

            var databaseType = configuration.DatabaseType();

            if (databaseType == DatabaseTypeEn.MySql)
            {
                AddDbContext_MySqlServer(services, configuration);
            }
            else if (databaseType == DatabaseTypeEn.SqlServer)
            {
                AddDbContext_SqlServer(services, configuration);
            }
            else if (databaseType == DatabaseTypeEn.Oracle)
            {
                AddDbContext_Oracle(services, configuration);
            }
            else if (databaseType == DatabaseTypeEn.Postgres)
            {
                AddDbContext_Oracle(services, configuration);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        private static void AddRepository(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICustomerEfRepository, CustomerEfRepository>();
            services.AddScoped<ICustomerTypeRepository, CustomerTypeRepository>();
            services.AddScoped<ICustomerImageRepository, CustomerImageRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICallRepository, CallRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IResolutionRepository, ResolutionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IOpenAIService, OpenAIService>();
            services.AddScoped<IRabbitMQMessageSender, RabbitMQMessageSender>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        private static void AddLoggedUser(IServiceCollection services) => services.AddScoped<ILoggedUser, LoggedUser>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void AddOpenAI(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOpenAI, ChatGptService>();

            var apiKey = configuration.GetValue<string>("AppSettings:OpenAI:ApiKey");

            services.AddScoped(c => new ChatClient(Constants.CHAT_MODEL, apiKey));
        }

        private static void AddAzureStorage(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("AppSettings:BlobStorage:Azure");

            if (connectionString != null)
            {
                services.AddScoped<IBlobStorageService>(c => new AzureStorageService(new BlobServiceClient(connectionString)));
            }
        }

        private static void AddQueue(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("AppSettings:ServiceBus:DeleteUserAccount")!;

            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });


            // 'user' é o nome da fila que foi configurado no Azure
            var deleteQueue = new AzureMessageService(client.CreateSender("user"));

            var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor("user", new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1 //Significa o CreateProcessor vai ler uma mensagem e depois de processar e vai ler a próxima
            }));

            services.AddSingleton(deleteUserProcessor);

            services.AddScoped<ICloudSendMessageService>(options => deleteQueue);
        }

        #region DataBase
        private static void AddDbContext_MySqlServer(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));

            services.AddDbContext<AppDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseMySql(connectionString, serverVersion);
            });
        }

        private static void AddDbContext_Oracle(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            services.AddDbContext<AppDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseOracle(connectionString);
            });
        }

        private static void AddDbContext_Postgres(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            services.AddDbContext<AppDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseOracle(connectionString);
            });
        }

        private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            services.AddDbContext<AppDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(connectionString);
            });
        }


        #endregion
    }
}
