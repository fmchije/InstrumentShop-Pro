using InstrumentShop.Data;
using InstrumentShop.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery; // Proveri da li imaš ovo
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
public class InstrumentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InstrumentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstrumentDto>>> Get()
    {
        var instruments = await _context.Instruments.ToListAsync();

        return Ok(instruments.Select(i => new InstrumentDto
        {
            Id = i.Id,
            Name = i.Name,
            Price = i.Price,
            StockQuantity = i.StockQuantity
        }));
    }

    [HttpPost]
    public async Task<ActionResult<InstrumentDto>> Post([FromBody] InstrumentDto instrumentDto)
    {
        // 1. Pretvaramo DTO nazad u Model za bazu
        var noviInstrument = new Instrument
        {
            Name = instrumentDto.Name,
            Price = instrumentDto.Price,
            StockQuantity = instrumentDto.StockQuantity
        };

        // 2. Dodajemo u EF Core context
        _context.Instruments.Add(noviInstrument);

        // 3. Čuvamo promene u SQL bazi
        await _context.SaveChangesAsync();

        // 4. Vraćamo kreirani objekat sa njegovim novim ID-jem iz baze
        instrumentDto.Id = noviInstrument.Id;
        return Ok(instrumentDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var instrument = await _context.Instruments.FindAsync(id);
        if (instrument == null)
        {
            return NotFound();
        }

        _context.Instruments.Remove(instrument);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    [IgnoreAntiforgeryToken]
    [HttpPut("{id}")]
    [HttpPost("izmeni-instrument/{id}")] // Promenili smo ime rute skroz
    
    public async Task<IActionResult> UpdateInstrument(int id, [FromBody] InstrumentDto dto)
    {
        if (id != dto.Id) return BadRequest("ID se ne poklapa.");
        
        var instrument = await _context.Instruments.FindAsync(id);
        if (instrument == null) return NotFound();

        
        instrument.Name = dto.Name;
        instrument.Price = dto.Price;
        instrument.StockQuantity = dto.StockQuantity;

        
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    

}