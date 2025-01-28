using System.ComponentModel.Design;
using System.Reflection;
using System.Text.RegularExpressions;
using WOS.Back.DependencyInjection;
using WOS.Dal.Interfaces;
using WOS.Back.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.ResponseCompression;
using WOS.Front.Services;

IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

var emailSettings = configuration.GetSection("EmailSettings");
var email = emailSettings["Email"];
var password = emailSettings["Password"];
var smtp = emailSettings["Smtp"];
var port = emailSettings["Port"];

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfiguration>(configuration);

string connectionString = configuration.GetConnectionString("database");
string pattern = @"Data Source=(.*?);";
string pathConnectionString = null;
if (Regex.Match(connectionString, pattern).Success)
    pathConnectionString = Regex.Match(connectionString, pattern).Groups[1].Value.Trim();

//var pathFolder = Path.GetDirectoryName(pathConnectionString);
string pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "BDD");
if (!Directory.Exists(pathFolder))
    Directory.CreateDirectory(pathFolder);

builder.Services.LoadServices(connectionString);

builder.Services.AddHttpClient();

builder.Services.AddScoped<IClientSrv, ClientSrv>();
builder.Services.AddScoped<IAdminSrv, AdminSrv>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthenticationSrv, AuthenticationSrv>();
builder.Services.AddScoped<IProduitSrv, ProduitSrv>();
builder.Services.AddScoped<ICommandeSrv, CommandeSrv>();
builder.Services.AddScoped<IQuestionSrv, QuestionSrv>();
builder.Services.AddScoped<IMarqueSrv, MarqueSrv>();
builder.Services.AddScoped<ICategorieSrv, CategorieSrv>();
builder.Services.AddScoped<IAvisSrv, AvisSrv>();
builder.Services.AddScoped<IMondialRelaySrv, MondialRelaySrv>();
builder.Services.AddScoped<IModeLivraisonSrv, ModeLivraisonSrv>();
builder.Services.AddScoped<IAdresseSrv, AdresseSrv>();
builder.Services.AddSingleton<IGlobalDataSrv, GlobalDataSrv>();
builder.Services.AddScoped<GlobalDataInitializer>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddDistributedMemoryCache(); // Nécessaire pour utiliser la session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Durée de vie de la session
    options.Cookie.HttpOnly = true; // Plus sécurisé pour les cookies
    options.Cookie.IsEssential = true; // Nécessaire pour fonctionner même avec le consentement des cookies
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Client", policy => policy.RequireRole("Client"));
});

builder.Services.AddHostedService<DailyTaskService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<GlobalDataInitializer>();
    await initializer.InitializeDataAsync();  // Charger les données
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/erreur");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/erreur");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings =
        {
            [".glb"] = "model/gltf-binary"
        }
    }
});

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
