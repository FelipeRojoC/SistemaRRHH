using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaRRHH.Models;

namespace SistemaRRHH.Pages
{
    public class EditarModel : PageModel
    {
        private readonly RrhhDbContext _context;

        public EditarModel(RrhhDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Mecanico Trabajador { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string rut)
        {
            if (rut == null)
            {
                return NotFound();
            }

            var trabajador = await _context.Mecanicos.FindAsync(rut);
            if (trabajador == null)
            {
                return NotFound();
            }

            Trabajador = trabajador;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Mecanicos.Update(Trabajador);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Trabajadores");
        }
    }
}
