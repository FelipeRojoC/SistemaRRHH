using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Pages.Vehiculos
{
    public class IndexModel : PageModel
    {
        private readonly ArriendosMantencionesDbContext _context;

        public IndexModel(ArriendosMantencionesDbContext context)
        {
            _context = context;
        }

        public IList<Vehiculo> Vehiculos { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Vehiculos = await _context.Vehiculos
                .OrderBy(v => v.Codigo)
                .ToListAsync();
        }
    }
}
