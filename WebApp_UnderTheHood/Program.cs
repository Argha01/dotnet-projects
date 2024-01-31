using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebApp_UnderTheHood;
using WebApp_UnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("WeatherApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7088");

});

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyCookieAuth";
        options.ExpireTimeSpan = TimeSpan.FromSeconds(200);
    });

builder.Services.AddTransient<ISample, Sample>();
builder.Services.AddTransient<Func<ISample>>(config => (() =>  config.GetService<ISample>()!));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin"));
    options.AddPolicy("BelongsToHRDepartment", 
        policy => policy.RequireClaim("Department", "HR")
                        .RequireClaim("hradmin")
                        .Requirements.Add(new HRManagerProbationRequirement(90)));
});

builder.Services.AddSingleton<IAuthorizationHandler,HRManagerProbationRequirementHandler>();

builder.Services.AddSession(options=>
{
    options.Cookie.Name = "MySessionCookie";
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromSeconds(200);

});


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
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
