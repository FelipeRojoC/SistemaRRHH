using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;
using SistemaMantenciones.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Puerto 5206: HTTP/1.1 para la aplicacion web (Razor Pages)
    options.ListenLocalhost(5206, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // Puerto 5207: HTTP/2 exclusivo para el servicio gRPC sin encriptar (cleartext)
    options.ListenLocalhost(5207, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorPages();
builder.Services.AddGrpc();

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
app.MapGrpcService<ServicioMantencionImpl>();

app.Run();
