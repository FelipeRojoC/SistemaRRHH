using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models; // Esto permite que Program.cs encuentre tus clases generadas

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// --- CONFIGURACIÓN DE LA BASE DE DATOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RrhhDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.46-mysql")));
// -----------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();