using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SistemaRRHH.Models;

namespace SistemaRRHH.Pages
{
    public class CalcularSueldosModel : PageModel
    {
        private readonly RrhhDbContext _context;

        public CalcularSueldosModel(RrhhDbContext context)
        {
            _context = context;
        }

        // Lista final que enviaremos a la pantalla
        public List<Liquidacion> Liquidaciones { get; set; } = new List<Liquidacion>();

        public async Task OnGetAsync()
        {
            // 1. Leer el archivo horas.json
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "horas.json");
            List<HorasTrabajador> horasRegistradas = new List<HorasTrabajador>();

            if (System.IO.File.Exists(filePath))
            {
                string jsonString = System.IO.File.ReadAllText(filePath);
                // Convertimos el texto JSON a una lista de C#
                horasRegistradas = JsonSerializer.Deserialize<List<HorasTrabajador>>(jsonString) ?? new List<HorasTrabajador>();
            }

            // 2. Traer todos los mecánicos de la base de datos
            // IMPORTANTE: Si te da error aquí, cambia _context.Mecanicos a _context.Mecanico
            var mecanicos = await _context.Mecanicos.ToListAsync();
            var mecanicosPorRut = mecanicos.ToDictionary(mecanico => mecanico.Rut, mecanico => mecanico);

            var rutsUnicos = horasRegistradas
                .Select(horas => horas.rut)
                .Where(rut => !string.IsNullOrWhiteSpace(rut))
                .Select(rut => rut!)
                .Concat(mecanicos.Select(mecanico => mecanico.Rut))
                .Distinct()
                .OrderBy(rut => rut)
                .ToList();

            // 3. Procesar y calcular los sueldos
            foreach (var rut in rutsUnicos)
            {
                mecanicosPorRut.TryGetValue(rut, out var mecanico);

                // Buscamos si el mecánico tiene horas registradas en el JSON
                var registroHoras = horasRegistradas.FirstOrDefault(h => h.rut == rut);
                int horas = registroHoras != null ? registroHoras.horasTrabajadas : 0;

                int sueldoBase = mecanico?.SueldoBase ?? 0;
                int valorHora = mecanico?.ValorHora ?? 0;
                string nombre = mecanico?.Nombre ?? "No registrado en trabajadores";
                
                // Aplicamos la fórmula exigida por la rúbrica
                int montoVariable = horas * valorHora;
                int sueldoTotal = sueldoBase + montoVariable;

                // Guardamos el resultado en la lista para mostrarlo
                Liquidaciones.Add(new Liquidacion
                {
                    Rut = rut,
                    Nombre = nombre,
                    SueldoBase = sueldoBase,
                    HorasTrabajadas = horas,
                    MontoVariable = montoVariable,
                    SueldoTotal = sueldoTotal,
                    TieneRegistroEnTrabajadores = mecanico != null
                });
            }
        }
    }

    // --- CLASES AUXILIARES ---
    // Molde para leer el JSON
    public class HorasTrabajador
    {
        public string? rut { get; set; }
        public int horasTrabajadas { get; set; }
    }

    // Molde para enviar los datos listos a la interfaz visual
    public class Liquidacion
    {
        public string? Rut { get; set; }
        public string? Nombre { get; set; }
        public int SueldoBase { get; set; }
        public int HorasTrabajadas { get; set; }
        public int MontoVariable { get; set; }
        public int SueldoTotal { get; set; }
        public bool TieneRegistroEnTrabajadores { get; set; }
    }
}