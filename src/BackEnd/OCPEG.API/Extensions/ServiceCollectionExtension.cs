using EvolveDb;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OCEPG.Infrastructure.DataAccess.Base;
using OCEPG.Infrastructure.Mappings;
using OCEPG.Infrastructure.Messages.RabbitMQSender;
using OCPEG.API.Filters;
using OCPEG.API.Token;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message;
using OCPEG.Domain.BusinessObject.Message;
using OCPEG.Domain.Security;
using OCPEG.Framework;
using Scalar.AspNetCore;
using Serilog;
using Sqids;
using System.Text;


namespace OCPEG.API.Extensions
{
    /// <summary>
    /// Extensão do Program.cs
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Configuração NLog
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureNLogService(this IServiceCollection services){
            services.AddScoped<NLogManager, NLogManager>();
        }

        /// <summary>
        /// Configuração NLog
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSerilogService(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }


        /// <summary>
        /// Injeção de dependencia do Contexto (registra o Contexto como serviço)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ConnectionSQLServer");
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));
        }

        /// <summary>
        /// Injeção de dependencia do Contexto (registra o Contexto como serviço)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Método Elaborado
            var host = configuration["DBHOST"] ?? "localhost";
            var port = configuration["DBPORT"] ?? "3306";
            var password = configuration["DBPASSWORD"] ?? "DicMySql#123";

            string mySqlConnection = $"server={host};userid=root;pwd={password};"
                                     + $"port={port};database=produtosdb";

            services.AddDbContext<AppDbContext>(options =>
                                          options.UseMySql(mySqlConnection,
                                          ServerVersion.AutoDetect(mySqlConnection)));


            // Método Simples
            var connectionString = configuration.GetConnectionString("ConnectionMySQLServer");
            services.AddDbContext<AppDbContext>(x => x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        }

        /// <summary>
        /// Injeção de dependencia do Contexto (registra o Contexto como serviço)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureOracleContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ConnectionOracleServer");
            services.AddDbContext<AppDbContext>(x => x.UseOracle(connectionString));
        }


        /// <summary>
        /// Configura o Mapper
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(
                (serviceProvider, cfg) =>
                {
                    var sqids = serviceProvider.GetRequiredService<SqidsEncoder<int>>();
                    var sqlds = serviceProvider.GetRequiredService<SqidsEncoder<long>>();

                    cfg.AddProfile<OcpegMappingProfile>();
                    cfg.AddProfile(new ComplexMappingProfile(sqids, sqlds));
                },
                Array.Empty<Type>() // 👈 resolve ambiguidade
            );
        }

        /// <summary>
        /// Configura RabittMq
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

            //*** Desabilitando o BackgroudService 
            //services.AddHostedService<RabbitMQEmailMessageConsumer>();
        }

        /// <summary>
        /// Configura os controllers
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureController(this IServiceCollection services)
        {
            services.AddControllers();
        }

        /// <summary>
        /// Configura JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenProvider, HttpContextTokenValue>();

            var jwtConfig = configuration.GetSection(Constants.JwtConfig);
            var secretKey = jwtConfig[Constants.SecretKey] ?? "";

            // Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["validIssuer"],
                    ValidAudience = jwtConfig["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        /// <summary>
        /// Configura o Swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var openApiContact = configuration.GetSection("AppSettings:OpenApiContact");
            var name = openApiContact["name"] ?? "";
            var url = openApiContact["url"] ?? "";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OC - Cadastro de Atendimentos",
                    Version = "v1",
                    Description = "Cadastro de Atendimentos",
                    Contact = new OpenApiContact
                    {
                        Name = name,
                        Url = new Uri(url)
                    }
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition(Constants.Bearer, new OpenApiSecurityScheme
                {
                    Description = Constants.BearerDescription,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = Constants.Bearer
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = Constants.Bearer
                            },
                            Scheme = "oauth2",
                            Name = Constants.Bearer,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        /// <summary>
        /// Configura Sqids (Critografia para ID)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureIdEncoder(this IServiceCollection services, IConfiguration configuration)
        {
            var sqids = new SqidsEncoder<int>(new()
            {
                MinLength = 3,
                Alphabet = configuration.GetValue<string>("AppSettings:IdCryptographyAlphabet")!
            });

            services.AddSingleton(sqids);


            var sqilds = new SqidsEncoder<long>(new()
            {
                MinLength = 5,
                Alphabet = configuration.GetValue<string>("AppSettings:IdCryptographyAlphabet")!
            });

            services.AddSingleton(sqilds);
        }

        /// <summary>
        /// Registra Dependencias
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterDependencies(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<LogFilter>();
        }

        /// <summary>
        /// Registra CORs (Para React)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void RegisterCors(this IServiceCollection services, IConfiguration configuration)
        {
            /* NOSONAR
            Aqui vai um exemplo antigo
            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("http://teste.com");
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
            builder.WithOrigins("https://trustedwebsite.com", "https://anothertrustedwebsite.com");
            //Fecha o exemplo 
            */

            var addresses = configuration.GetValue<string>("MySettings:AllowAddress");
            string[] allowedAddresses = string.IsNullOrEmpty(addresses) ? Array.Empty<string>() : addresses.Split(";");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(allowedAddresses)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });

                options.AddPolicy(name: "EnableAllPolicy", builder =>
                {
                    builder.WithOrigins(allowedAddresses)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        /// <summary>
        /// Executa em Background
        /// </summary>
        /// <param name="services"></param>
        public static void AddHostedService(this IServiceCollection services)
        {
            /* NOSONAR
            //Comentado porque o Azure não está ativo
            //services.AddHostedService<DeleteUserService>();
            */
        }

        /// <summary>
        /// Registra Dependencias
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var clientId = configuration.GetValue<string>("AppSettings:Google:ClientId")!;
            var clientSecret = configuration.GetValue<string>("AppSettings:Google:ClientSecret")!;

            services.AddAuthentication(config =>
            {
                config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = clientId;
                googleOptions.ClientSecret = clientSecret;
            });
        }

        /// <summary>
        /// Configura o Migration Evolve
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddEvolveConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                var connectionString = configuration[
                    "ConnectionStrings:ConnectionSQLServer"];

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException(
                        "Connection string 'ConnectionSQLServer' not found.");
                }

                try
                {
                    using var evolveConnection = new SqlConnection(connectionString);
                    var evolve = new Evolve(
                        evolveConnection,
                        msg => Log.Information(msg))
                    {
                        Locations = new List<string> { "db/migrations", "db/dataset" },
                        IsEraseDisabled = true
                    };
                    evolve.Migrate();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while migrating the database.");
                    throw;
                }
            }
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication UseScalarConfiguration(
            this WebApplication app)
        {
            app.MapScalarApiReference("/scalar", options =>
            {
                options
                    .WithTitle("OCPEG")
                    .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
            });
            return app;
        }

    }
}
