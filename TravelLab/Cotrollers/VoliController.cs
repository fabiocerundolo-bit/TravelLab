using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
[Authorize]
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
}