using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;

[ApiController]
[Route("api/[controller]")]
public class VoliController : ControllerBase
{
    private readonly TravelLabContext _context;

    public VoliController(TravelLabContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetVoli(
        [FromQuery] string destinazione = null,
        [FromQuery] DateTime? dataInizio = null,
        [FromQuery] DateTime? dataFine = null)
    {
        var query = from viaggio in _context.Viaggi
            join prenotazione in _context.Prenotazioni on viaggio.Id equals prenotazione.ViaggioId
            join biglietto in _context.Biglietti on prenotazione.Id equals biglietto.PrenotazioneId
            join servizio in _context.Servizi on biglietto.ServizioId equals servizio.Id
            join volo in _context.Voli on servizio.Id equals volo.ServizioId
            where servizio.TipoServizio == "VOLO"
            select new
            {
                viaggio.Destinazione,
                viaggio.DataInizio,
                viaggio.DataFine,
                volo.NumeroVolo,
                volo.CompagniaAerea,
                biglietto.PrezzoEffettivo
            };

        if (!string.IsNullOrEmpty(destinazione))
            query = query.Where(v => EF.Functions.ILike(v.Destinazione, $"%{destinazione}%"));

        if (dataInizio.HasValue)
            query = query.Where(v => v.DataInizio >= dataInizio.Value);

        if (dataFine.HasValue)
            query = query.Where(v => v.DataInizio <= dataFine.Value);

        var result = await query.OrderBy(v => v.DataInizio).ToListAsync();
        return Ok(result);
    }
}