using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

namespace  TravelLab
{
    
}
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ViaggiController : ControllerBase
{
    private readonly TravelLabContext _context;
    public ViaggiController(TravelLabContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetViaggi()
    {
        var viaggi = await _context.Viaggi
            .Select(v => new
            {
                v.Id,
                v.Descrizione,
                v.DataInizio,
                v.DataFine,
                v.Destinazione,
                v.PrezzoBase
            })
            .ToListAsync();
        return Ok(viaggi);
    }

    [HttpPost]
    public async Task<IActionResult> CreateViaggio([FromBody] Viaggio viaggio)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.Viaggi.Add(viaggio);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetViaggi), new { id = viaggio.Id }, viaggio);
    }
    
}