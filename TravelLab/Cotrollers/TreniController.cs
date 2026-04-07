using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

namespace TravelLab.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TreniController : ControllerBase
    {
        private readonly TravelLabContext _context;

        public TreniController(TravelLabContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTreni()
        {
            var treni = await _context.Treni
                .Include(t => t.Servizio)
                .Include(t => t.Mezzo)
                .Select(t => new
                {
                    t.IdServizio,
                    t.NumeroTreno,
                    t.TipoTreno,
                    Compagnia = t.Mezzo != null ? t.Mezzo.Compagnia : null,
                    PrezzoBase = t.Servizio != null ? t.Servizio.PrezzoBase : 0
                })
                .ToListAsync();

            return Ok(treni);
        }
    }
}