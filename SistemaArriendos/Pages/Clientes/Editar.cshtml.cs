using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Clientes
{
    public class EditarModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public EditarModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Rut == id);
            if (cliente == null)
            {
                return NotFound();
            }

            Cliente = cliente;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Cliente.Arriendos");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clientes.AnyAsync(c => c.Rut == Cliente.Rut))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = "Cliente actualizado correctamente.";
            return RedirectToPage("./Index");
        }
    }
}
