using Microsoft.EntityFrameworkCore;
using Moscow_weather.Repositories;
using Moscow_weather.Repositories.Interfaces;
using Moscow_weather.Services;
using Moscow_weather.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IWeatherDataRepo, WeatherDataRepo>();

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

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "browse",
        pattern: "Browse",
        defaults: new { controller = "Browse", action = "Index" });
    endpoints.MapControllerRoute(
        name: "upload",
        pattern: "Upload",
        defaults: new { controller = "Upload", action = "Index" });
});

app.Run();
