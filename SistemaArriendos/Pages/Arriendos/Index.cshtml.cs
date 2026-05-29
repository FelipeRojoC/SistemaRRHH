using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Arriendos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public IndexModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IList<Arriendo> Arriendos { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Arriendos = await _context.Arriendos
                .Include(a => a.CodigoVehiculoNavigation)
                .Include(a => a.RutClienteNavigation)
                .OrderByDescending(a => a.FechaInicio)
                .ToListAsync();
        }
    }
}
