using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models;

namespace SistemaRRHH.Pages
{
    public class TrabajadoresModel : PageModel
    {
        private readonly RrhhDbContext _context;

        public TrabajadoresModel(RrhhDbContext context)
        {
            _context = context;
        }

        public IList<Mecanico> Mecanicos { get; set; } = default!;

        public string? ErrorMensaje { get; set; }

        [BindProperty]
        public Mecanico NuevoMecanico { get; set; } = default!;

        [BindProperty]
        public string? RutEnEdicion { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Mecanicos != null)
            {
                Mecanicos = await _context.Mecanicos.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Mecanicos == null || NuevoMecanico == null)
            {
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(RutEnEdicion))
            {
                var mecanicoExistente = await _context.Mecanicos.FindAsync(RutEnEdicion);

                if (mecanicoExistente == null)
                {
                    return NotFound();
                }

                mecanicoExistente.Rut = NuevoMecanico.Rut;
                mecanicoExistente.Nombre = NuevoMecanico.Nombre;
                mecanicoExistente.SueldoBase = NuevoMecanico.SueldoBase;
                mecanicoExistente.ValorHora = NuevoMecanico.ValorHora;
            }
            else
            {
                _context.Mecanicos.Add(NuevoMecanico);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Trabajadores");
        }

        public async Task<IActionResult> OnPostEditAsync(string rut)
        {
            if (_context.Mecanicos == null)
            {
                return NotFound();
            }

            var mecanico = await _context.Mecanicos.FindAsync(rut);

            if (mecanico == null)
            {
                return NotFound();
            }

            NuevoMecanico = new Mecanico
            {
                Rut = mecanico.Rut,
                Nombre = mecanico.Nombre,
                SueldoBase = mecanico.SueldoBase,
                ValorHora = mecanico.ValorHora
            };

            RutEnEdicion = mecanico.Rut;
            await OnGetAsync();
            return Page();
        }

        // --- CÓDIGO NUEVO PARA ELIMINAR ---
        public async Task<IActionResult> OnPostDeleteAsync(string rut)
        {
            if (rut == null || _context.Mecanicos == null)
            {
                return NotFound();
            }

            // Busca al trabajador en la base de datos usando su RUT
            var trabajador = await _context.Mecanicos.FindAsync(rut);

            if (trabajador != null)
            {
                // Si lo encuentra, lo elimina y guarda los cambios
                _context.Mecanicos.Remove(trabajador);
                await _context.SaveChangesAsync();
            }

            // Recarga la página para actualizar la tabla
            return RedirectToPage();
        }
        // ----------------------------------
    }
}