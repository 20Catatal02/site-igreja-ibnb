using Microsoft.EntityFrameworkCore;
using IBNB.Data; 
using IBNB.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

//Traduzir interface para pt-BR
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configuração do Banco de Dados
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// FORÇAR A CONFIGURAÇÃO DE AUTENTICAÇÃO E COOKIE
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

// Configuração do Identity (Autenticação)
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; 
    options.Password.RequireNonAlphanumeric = false; 
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// Serviços MVC e Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Necessário para as telas de Login/Registro do Identity

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<VersiculoDoDiaService>();
builder.Services.AddScoped<VersiculosSeeder>();

// Somente o serviço de feriados usa rede (BrasilAPI, opt-in).
builder.Services.AddHttpClient<IBNB.Services.FeriadoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    try
    {
        await scope.ServiceProvider.GetRequiredService<VersiculosSeeder>().SeedAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao carregar o seed de versículos.");
    }
}

app.Run();
