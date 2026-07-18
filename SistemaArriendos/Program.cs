using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorPages();

// Configurar el cliente gRPC para conectar al puerto HTTP/2 exclusivo (puerto 5207)
builder.Services.AddGrpcClient<SistemaArriendos.Protos.ServicioMantencion.ServicioMantencionClient>(options =>
{
    options.Address = new Uri("http://localhost:5207");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ArriendosMantencionesDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.44-mysql")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArriendosMantencionesDbContext>();
    db.Database.EnsureCreated();

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
