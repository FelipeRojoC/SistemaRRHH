using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Messaging;
using SistemaArriendos.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddScoped<PublicadorArriendo>();
builder.Services.AddHostedService<ConsumidorVehiculo>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ArriendosMantencionesDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.44-mysql")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArriendosMantencionesDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS vehiculo_cache (
        codigo VARCHAR(20) PRIMARY KEY,
        patente VARCHAR(10) NOT NULL DEFAULT '',
        marca VARCHAR(50) NOT NULL DEFAULT '',
        modelo VARCHAR(50) NOT NULL DEFAULT '',
        tipo VARCHAR(50) NOT NULL DEFAULT '',
        kilometraje INT NOT NULL DEFAULT 0,
        estado VARCHAR(30) NOT NULL DEFAULT '',
        precioArriendoDiario INT NOT NULL DEFAULT 0
    )");

    // Semilla para asegurar que al menos haya un cliente disponible para pruebas
    if (!db.clientes.Any())
    {
        db.clientes.Add(new Cliente
        {
            rut = "11111111-1",
            nombre = "Cliente de Prueba",
            direccion = "Calle Falsa 123"
        });
        db.SaveChanges();
    }
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
