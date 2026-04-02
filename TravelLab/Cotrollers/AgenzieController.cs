using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
namespace TravelLab.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AgenzieController : ControllerBase
{
    private readonly TravelLabContext _context;
    public AgenzieController(TravelLabContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAgenzie()
    {
        var agenzie = await _context.Agenzie
            .Select(a => new
            {
                a.Id,
                a.Nome,
                a.Email,
                a.Telefono,
                a.Indirizzo
            })
            .ToListAsync();

        return Ok(agenzie);
    }
}