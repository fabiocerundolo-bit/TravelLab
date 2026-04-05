using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

namespace TravelLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BigliettiController : ControllerBase
    {
        private readonly TravelLabContext _context;

        public BigliettiController(TravelLabContext context)
        {
            _context = context;
        }
        
        

        // Endpoint per creare un biglietto treno
        [HttpPost("treno")]
        public async Task<IActionResult> CreateBigliettoTreno([FromBody] CreateBigliettoTrenoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica che la prenotazione esista
            var prenotazione = await _context.Prenotazioni.FindAsync(dto.PrenotazioneId);
            if (prenotazione == null)
                return NotFound("Prenotazione non trovata");

            // Verifica che il treno esista (tramite id_servizio)
            var treno = await _context.Treni.FindAsync(dto.TrenoId);
            if (treno == null)
                return NotFound("Treno non trovato");

            var biglietto = new Biglietto
            {
                PrenotazioneId = dto.PrenotazioneId,
                ServizioId = dto.TrenoId,
                PrezzoEffettivo = dto.PrezzoEffettivo
            };

            _context.Biglietti.Add(biglietto);
            await _context.SaveChangesAsync();

            return Ok(biglietto);
        }
        

        // Endpoint per creare un biglietto nave
        [HttpPost("nave")]
        public async Task<IActionResult> CreateBigliettoNave([FromBody] CreateBigliettoNaveDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prenotazione = await _context.Prenotazioni.FindAsync(dto.PrenotazioneId);
            if (prenotazione == null)
                return NotFound("Prenotazione non trovata");

            var nave = await _context.Navi.FindAsync(dto.NaveId);
            if (nave == null)
                return NotFound("Nave non trovata");

            var biglietto = new Biglietto
            {
                PrenotazioneId = dto.PrenotazioneId,
                ServizioId = dto.NaveId,
                PrezzoEffettivo = dto.PrezzoEffettivo
            };

            _context.Biglietti.Add(biglietto);
            await _context.SaveChangesAsync();

            return Ok(biglietto);
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAllBiglietti()
        {
            var biglietti = await _context.Biglietti
                .Include(b => b.Prenotazione)
                .ThenInclude(p => p.Cliente)
                .Include(b => b.Servizio)
                .Select(b => new
                {
                    b.Id,
                    b.PrenotazioneId,
                    Cliente = b.Prenotazione != null ? b.Prenotazione.Cliente.Nome + " " + b.Prenotazione.Cliente.Cognome : null,
                    ServizioTipo = b.Servizio != null ? b.Servizio.TipoServizio : null,
                    PrezzoEffettivo = Math.Round(b.PrezzoEffettivo, 2)   // 👈 arrotonda a 2 decimali
                })
                .ToListAsync();

            return Ok(biglietti);
        }
        
        
    }
}