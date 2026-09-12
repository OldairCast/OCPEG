using OCPEG.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Para uso de session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    /* NOSONAR options.IdleTimeout = TimeSpan.FromSeconds(10); */
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddGoogleAuthentication(builder.Configuration);

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    //Habilita o refreh em Runtime
   mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Utilização de HttpContext no serviço
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection(); //Utilização de Https (colocar depois de IsDevelopment)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();

app.MapControllerRoute(
    name: "Login",
    pattern: "Login/",
    defaults: new { controller = "Login", action = "Index" }
).WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
)
.WithStaticAssets();

app.UseMiddleware<ErrorHandlingMiddleware>();

await app.RunAsync();

