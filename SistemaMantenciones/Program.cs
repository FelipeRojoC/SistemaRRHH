using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Messaging;
using SistemaMantenciones.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddScoped<PublicadorVehiculo>();
builder.Services.AddHostedService<ConsumidorArriendo>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ArriendosMantencionesDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.44-mysql")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArriendosMantencionesDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
