using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Data;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<OrdersApplicationDbContext>(options =>
//     options.UseSqlite(builder.Configuration.GetConnectionString("OrdersApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'OrdersApplicationDbContext' not found.")));
// builder.Services.AddDbContext<OrdersApplicationDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersApplicationDbContext2") ?? throw new InvalidOperationException("Connection string 'OrdersApplicationDbContext' not found.")));

builder.Services.AddDbContext<OrdersApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersApplicationDbContext3") ?? throw new InvalidOperationException("Connection string 'OrdersApplicationDbContext' not found.")));
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options=>
{
    var supportedCultrues =new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ru-RU")
    };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedUICultures = supportedCultrues;
});

var app = builder.Build();
app.UseRequestLocalization();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
