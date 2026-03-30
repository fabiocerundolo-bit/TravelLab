using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

namespace AgenziaViaggiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientiController : ControllerBase
    {
        private readonly TravelLabContext _context;

        public ClientiController(TravelLabContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetClienti()
        {
            var clienti = await _context.Clienti
                .OrderBy(c => c.Id)
                .ToListAsync();
            return Ok(clienti);
        }

        [HttpGet("senza-prenotazioni")]
        public async Task<IActionResult> GetClientiSenzaPrenotazioni()
        {
            var clienti = await _context.Clienti
                .Where(c => !_context.Prenotazioni.Any(p => p.ClienteId == c.Id))
                .Select(c => new { c.Id, c.Nome, c.Cognome, c.Email, c.Telefono })
                .ToListAsync();
            return Ok(clienti);
        }
    }
}