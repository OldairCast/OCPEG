using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OCEPG.Infrastructure.DataAccess.Base;

namespace OCPEG.WebApi.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test")  //Test é o nome do ambiente/environment
                .ConfigureServices(services =>
                {
                    //Aqui está indo nos serviços de dependencia e verificando se já existe adicionado o dbcontext.
                    //se já existe então remove ele porque senão ele vai tentar se conectar com o banco real.
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    //Configurando para utilizar o banco em memória
                    var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                    //var blobStorage = new BlobStorageServiceBuilder().Build();
                    //services.AddScoped(option => blobStorage);

                    //Aqui coloca para ser utilizado no DBContext
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(provider);
                    });

                    using var scope = services.BuildServiceProvider().CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    dbContext.Database.EnsureDeleted();

                    StartDatabase(dbContext);
                });
        }

        private void StartDatabase(AppDbContext dbContext)
        {
            //(_user, _password) = UserBuilder.Build();

            //_recipe = RecipeBuilder.Build(_user);

            //_refreshToken = RefreshTokenBuilder.Build(_user);

            //dbContext.Users.Add(_user);

            //dbContext.Recipes.Add(_recipe);

            //dbContext.RefreshTokens.Add(_refreshToken);

            dbContext.SaveChanges();
        }

    }
}
