using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("WeatherApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7063");

});

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.ClientId = builder.Configuration["OpenId:AppId"]!;
        options.ClientSecret = builder.Configuration["OpenId:AppSecret"]!;
        options.CallbackPath = "/signin-oidc";
        options.Instance = builder.Configuration["OpenId:Instance"]!;
        options.TenantId = builder.Configuration["OpenId:TenantId"]!;
    })
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "api://9cbbdee6-fce8-4c1e-943e-833b7c2f2034/Weather.Read" })
    .AddInMemoryTokenCaches();

// Add services to the container.
builder.Services.AddControllersWithViews();

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
