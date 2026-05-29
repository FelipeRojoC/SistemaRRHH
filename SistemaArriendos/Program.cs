using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuraciones locales de desarrollo opcionales (no rastreadas por Git)
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorPages();

// --- CONFIGURACIÓN DE LA BASE DE DATOS COMPARTIDA (con Sistema de Mantenciones) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ArriendosMantencionesDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.44-mysql")));
// ----------------------------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
