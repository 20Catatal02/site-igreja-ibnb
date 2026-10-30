using Microsoft.EntityFrameworkCore;
using IBNB.Data; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;

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

builder.Services.AddHttpClient<IBNB.Services.FeriadoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Ou app.MapStaticAssets() se for .NET 9+

app.UseRouting();

app.UseAuthentication(); // Diz "quem é o usuário" (lê o cookie)
app.UseAuthorization();  // Diz "o que ele pode acessar" ([Authorize])

app.MapRazorPages(); // Mapeia as telas do Identity (/Identity/Account/Login)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();