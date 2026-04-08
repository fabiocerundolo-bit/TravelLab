using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

namespace TravelLab.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class PrenotazioniController : ControllerBase
    {
        private readonly TravelLabContext _context;

        public PrenotazioniController(TravelLabContext context)
        {
            _context = context;
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetPrenotazioniByCliente(int clienteId)
        {
            var prenotazioni = await _context.Prenotazioni
                .Include(p => p.Viaggio)
                .Include(p => p.Fattura)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataPrenotazione)
                .Select(p => new
                {
                    p.Id,
                    Destinazione = p.Viaggio.Destinazione,
                    DataInizio = p.Viaggio.DataInizio,
                    DataFine = p.Viaggio.DataFine,
                    p.DataPrenotazione,
                    p.Stato,
                    ImportoTotale = p.Fattura != null ? p.Fattura.ImportoTotale : (decimal?)null,
                    MetodoPagamento = p.Fattura != null ? p.Fattura.MetodoPagamento : null,
                    DataEmissione = p.Fattura != null ? p.Fattura.DataEmissione : (DateTime?)null
                })
                .ToListAsync();

            return Ok(prenotazioni);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetPrenotazioniCount()
        {
            var count = await _context.Prenotazioni.CountAsync();
            return Ok(new { count });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreatePrenotazione([FromBody] CreatePrenotazioneDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = await _context.Clienti.FindAsync(dto.ClienteId);
            var viaggio = await _context.Viaggi.FindAsync(dto.ViaggioId);
            var agenzia = await _context.Agenzie.FindAsync(dto.AgenziaId);
            if (cliente == null || viaggio == null || agenzia == null)
                return BadRequest("Cliente, viaggio o agenzia non validi.");

            var prenotazione = new Prenotazione
            {
                ClienteId = dto.ClienteId,
                ViaggioId = dto.ViaggioId,
                AgenziaId = dto.AgenziaId,
                DataPrenotazione = dto.DataPrenotazione,
                Stato = dto.Stato
            };

            _context.Prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();

            // Restituisci solo i dati essenziali, senza cicli
            var result = new
            {
                prenotazione.Id,
                prenotazione.ClienteId,
                prenotazione.ViaggioId,
                prenotazione.AgenziaId,
                prenotazione.DataPrenotazione,
                prenotazione.Stato
            };

            return CreatedAtAction(nameof(GetPrenotazioniByCliente), new { clienteId = prenotazione.ClienteId }, result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPrenotazioni()
        {
            var query = from p in _context.Prenotazioni
                join v in _context.Viaggi on p.ViaggioId equals v.Id
                join c in _context.Clienti on p.ClienteId equals c.Id
                select new
                {
                    p.Id,
                    p.ClienteId,
                    ClienteNome = c.Nome,
                    ClienteCognome = c.Cognome,
                    p.ViaggioId,
                    Destinazione = v.Destinazione,
                    DataInizio = v.DataInizio,
                    DataFine = v.DataFine,
                    p.AgenziaId,
                    p.DataPrenotazione,
                    p.Stato
                };

            var prenotazioni = await query.ToListAsync();
            return Ok(prenotazioni);
        }
        

        
        
    }
}