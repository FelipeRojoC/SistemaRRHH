using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Pages.Mantenciones
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class GenerarHorasModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public GenerarHorasModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public List<ConsolidadoHoras> HorasConsolidadas { get; set; } = new List<ConsolidadoHoras>();

        public async Task OnGetAsync()
        {
            // Agrupar y sumar horas por mecánico desde la base de datos
            if (_context.Mantencions != null)
            {
                HorasConsolidadas = await _context.Mantencions
                    .GroupBy(m => m.RutMecanico)
                    .Select(g => new ConsolidadoHoras
                    {
                        Rut = g.Key,
                        TotalHoras = g.Sum(m => m.Horas),
                        CantidadServicios = g.Count()
                    })
                    .OrderBy(h => h.Rut)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostExportarAsync()
        {
            if (_context.Mantencions == null)
            {
                return Page();
            }

            // Agrupar y consolidar
            var consolidados = await _context.Mantencions
                .GroupBy(m => m.RutMecanico)
                .Select(g => new
                {
                    rut = g.Key,
                    horasTrabajadas = g.Sum(m => m.Horas)
                })
                .ToListAsync();

            // Ruta de salida física hacia el Sistema de Recursos Humanos
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SistemaRecursosHumanos");
            string filePath = Path.Combine(folderPath, "horas.json");

            try
            {
                // Asegurarse de que el directorio del sistema de RRHH existe
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Configurar serialización con camelCase / minúsculas explícitas
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(consolidados, options);
                await System.IO.File.WriteAllTextAsync(filePath, jsonString);

                TempData["SuccessMessage"] = "¡El archivo 'horas.json' ha sido generado y exportado exitosamente al Sistema de Recursos Humanos!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al generar el archivo: {ex.Message}";
            }

            return RedirectToPage();
        }
    }

    public class ConsolidadoHoras
    {
        public string Rut { get; set; } = null!;
        public int TotalHoras { get; set; }
        public int CantidadServicios { get; set; }
    }
}
