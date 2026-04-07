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
                .Select(c => new
                {
                    c.Id,
                    c.Nome,
                    c.Cognome,
                    c.Email,
                    c.Telefono,
                    c.Indirizzo
                })
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
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.Clienti.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClienti), new { id = cliente.Id }, cliente);
        }
        [HttpGet("by-email")]
        public async Task<IActionResult> GetClienteByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { error = "Email richiesta" });
    
            var cliente = await _context.Clienti.FirstOrDefaultAsync(c => c.Email == email);
            return Ok(cliente); // restituisce null se non trovato
        }
    }
}