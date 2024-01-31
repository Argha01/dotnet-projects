using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using WebApp;
using WebApp.Data;
using WebApp.Data.Account;
using WebApp.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Identity"));
});

//the below method internally calls AddAuthentication method
//builder.Services.AddIdentity<User, IdentityRole>(options =>
//{
//    options.Password.RequiredLength = 8;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireLowercase = true;

//    options.User.RequireUniqueEmail = true;

//    options.SignIn.RequireConfirmedEmail = true;

//    options.Lockout.MaxFailedAccessAttempts = 5;
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

//})
//.AddEntityFrameworkStores<AppDbContext>()
//.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    //.AddMicrosoftAccount(options =>
    //{
    //    options.ClientId = builder.Configuration["Microsoft:AppId"]!;
    //    options.ClientSecret = builder.Configuration["Microsoft:AppSecret"]!;
    //    options.CallbackPath = "/signin-microsoft";

    //})
    //.AddFacebook(options =>
    //{
    //    options.ClientId = builder.Configuration["FaceBook:AppId"]!;
    //    options.ClientSecret = builder.Configuration["FaceBook:AppSecret"]!;
    //    options.CallbackPath = "/signin-facebook";
    //})
    //.AddGoogle(options =>
    //{
    //    options.ClientId = builder.Configuration["Google:AppId"]!;
    //    options.ClientSecret = builder.Configuration["Google:AppSecret"]!;
    //    options.CallbackPath = "/signin-google";
    //})
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.ClientId = builder.Configuration["OpenId:AppId"]!;
        options.ClientSecret = builder.Configuration["OpenId:AppSecret"]!;
        options.CallbackPath = "/signin-oidc";
        options.Instance = builder.Configuration["OpenId:Instance"]!;
        options.TenantId = builder.Configuration["OpenId:TenantId"]!;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IT", policy => policy.RequireClaim("Department", "IT"));
    options.AddPolicy("BANK", policy => policy.RequireClaim("Department", "BANK"));
});
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));

builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
