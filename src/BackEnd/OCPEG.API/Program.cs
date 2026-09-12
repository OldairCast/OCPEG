using Microsoft.Extensions.Diagnostics.HealthChecks;
using OCEPG.Infrastructure.DataAccess.Base;
using OCEPG.Infrastructure.DataAccess.Migrations;
using OCEPG.Infrastructure.Extension;
using OCPEG.API.Extensions;
using OCPEG.API.Filters;
using OCPEG.API.Middleware;
using OCPEG.Application.Extension;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Configuração NLog
builder.Services.ConfigureNLogService();

//Configuração Serilog
builder.Services.ConfigureSerilogService(builder.Configuration);

//é responsável por registrar o Serilog como provedor padrão da aplicação,
//com isso é possível também utilizar a interface ILogger para registrar os logs da aplicação.
builder.Host.UseSerilog();

//Configuração Mapper
builder.Services.ConfigureMapping();

//Configuração RabbitMQ
builder.Services.ConfigureRabbitMQ(builder.Configuration);

//Configuração Evolve
builder.Services.AddEvolveConfiguration(builder.Configuration, builder.Environment);

//Configuração Controllers
builder.Services.ConfigureController();

//Configuração Jwt Token
builder.Services.ConfigureJWT(builder.Configuration);

//Configura Swagger
builder.Services.ConfigureSwagger(builder.Configuration);

//Configura Sqids (Critografia para ID)
builder.Services.ConfigureIdEncoder(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Registra Dependencias
builder.Services.RegisterDependencies();

//Registra CORs (Para React)
builder.Services.RegisterCors(builder.Configuration);


builder.Services.AddApplication(builder.Configuration);

//Logs de Inicio e Fim
builder.Services.AddScoped<LogFilter>();

//Adiciona Repositorios e Configuração de Banco
builder.Services.AddInfrastructure(builder.Configuration);

//Torna todas as rotas como minusculas
builder.Services.AddRouting(opt => opt.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHostedService();

builder.Services.AddGoogleAuthentication(builder.Configuration);

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

var app = builder.Build();

app.MapHealthChecks("/Health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OC Cadastro de Atendimentos v1"));
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection(); //Utilização de Https (colcar depois de IsDevelopment)

app.UseRouting();

app.UseCors();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseScalarConfiguration();
app.MapControllers();

//Verifica se o banco existe, se não existir cria
if (app.Environment.IsDevelopment())
{
    MigrateDatabase();
}

await app.RunAsync();


void MigrateDatabase()
{
    if (builder.Configuration.IsUnitTestEnviroment())
        return;

    var databaseType = builder.Configuration.DatabaseType();
    var connectionString = builder.Configuration.ConnectionString();
    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    
    DatabaseMigration.Migrate(databaseType, connectionString, serviceScope.ServiceProvider);
}

/// <summary>
/// 
/// </summary>
public partial class Program
{
    /// <summary>
    /// 
    /// </summary>
    protected Program() { }
}