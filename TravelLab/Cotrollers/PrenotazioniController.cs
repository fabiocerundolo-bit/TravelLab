using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;

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
}