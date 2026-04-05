using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

namespace TravelLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NaviController : ControllerBase
    {
        private readonly TravelLabContext _context;

        public NaviController(TravelLabContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNavi()
        {
            var navi = await _context.Navi
                .Include(n => n.Servizio)
                .Include(n => n.Mezzo)
                .Select(n => new
                {
                    n.IdServizio,
                    n.NomeNave,
                    Compagnia = n.Mezzo != null ? n.Mezzo.Compagnia : null,
                    PrezzoBase = n.Servizio != null ? n.Servizio.PrezzoBase : 0
                })
                .ToListAsync();

            return Ok(navi);
        }
    }
}