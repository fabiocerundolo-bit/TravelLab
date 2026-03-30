using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;

namespace TravelLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticheController : ControllerBase
{
    private readonly TravelLabContext _context;

    public StatisticheController(TravelLabContext context)
    {
        _context = context;
    }

    [HttpGet("top-destinazioni")]
    public async Task<IActionResult> GetTopDestinazioni()
    {
        var top = await _context.Viaggi
            .GroupJoin(_context.Prenotazioni, v => v.Id, p => p.ViaggioId, (v, prenotazioni) => new { v.Destinazione, Count = prenotazioni.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .Select(x => new { x.Destinazione, NumeroPrenotazioni = x.Count })
            .ToListAsync();

        return Ok(top);
    }

    [HttpGet("ricavi-mensili")]
    public async Task<IActionResult> GetRicaviMensili()
    {
        var ricavi = await _context.Fatture
            .GroupBy(f => new { f.DataEmissione.Year, f.DataEmissione.Month })
            .Select(g => new
            {
                Anno = g.Key.Year,
                Mese = g.Key.Month,
                RicavoTotale = g.Sum(f => f.ImportoTotale),
                NumeroFatture = g.Count()
            })
            .OrderBy(r => r.Anno).ThenBy(r => r.Mese)
            .ToListAsync();

        // Formattiamo il risultato in memoria (client evaluation) dopo aver ottenuto i dati
        var result = ricavi.Select(r => new
        {
            MeseAnno = $"{r.Anno}-{r.Mese:D2}",
            r.RicavoTotale,
            r.NumeroFatture
        }).ToList();

        return Ok(result);
    }
}
    
