using InstrumentShop;
using InstrumentShop.Data;
using InstrumentShop.Services;
using InstrumentShop.Shared;
using Microsoft.AspNetCore.Antiforgery; // Proveri da li imaš ovo
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
public class InstrumentsController : ControllerBase
{
    private readonly IInstrumentRepository _repository;
    private readonly ILogger<InstrumentsController> _logger;
    private readonly ICurrencyService _currencyService; 

    public InstrumentsController(IInstrumentRepository repository, ILogger<InstrumentsController> logger, ICurrencyService currencyService) 
    {
        _repository = repository;
        _logger = logger;
        _currencyService = currencyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstrumentDto>>> Get()
    {
        // Podaci dolaze iz repozitorijuma, ne direktno iz DB konteksta
        var instruments = await _repository.GetAllAsync();

        return Ok(instruments.Select(i => new InstrumentDto
        {
            Id = i.Id,
            Name = i.Name,
            Price = i.Price,
            StockQuantity = i.StockQuantity
        }));
    }
    [HttpPost]
    public async Task<ActionResult<InstrumentDto>> Post([FromBody] InstrumentDto dto)
    {
        // 1. Provera validacije (Range, Required, itd.)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Ovde test "hvata" grešku
        }

        // 2. Mapiranje DTO -> Model i čuvanje u bazu
        var noviInstrument = new Instrument
        {
            Name = dto.Name,
            Price = dto.Price
        };

        await _repository.AddAsync(noviInstrument);

        // 3. Vraćamo kreirani objekat sa dodeljenim ID-em
        dto.Id = noviInstrument.Id;
        return Ok(dto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var instrument = await _repository.GetByIdAsync(id);

        if (instrument == null)
        {
            _logger.LogWarning("Pokušaj brisanja nepostojećeg instrumenta sa ID-em {Id}", id);
            return NotFound();
        }

        await _repository.DeleteAsync(instrument);

        // Profesionalni log sa parametrima
        _logger.LogInformation("Instrument sa ID-em {Id} ({Name}) je uspešno obrisan u {Time}",
            id, instrument.Name, DateTime.Now);

        return NoContent();
    }


    [IgnoreAntiforgeryToken]
    [HttpPut("{id}")]
    [HttpPost("izmeni-instrument/{id}")] // Promenili smo ime rute skroz

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInstrument(int id, [FromBody] InstrumentDto dto)
    {
        // 1. Validacija (Ovo ostaje u kontroleru)
        if (id != dto.Id) return BadRequest("ID se ne poklapa.");

        // 2. Traženje postojećeg instrumenta preko REPOZITORIJUMA
        var instrument = await _repository.GetByIdAsync(id);
        if (instrument == null) return NotFound();

        // 3. Mapiranje podataka (DTO -> Model)
        instrument.Name = dto.Name;
        instrument.Price = dto.Price;
        instrument.StockQuantity = dto.StockQuantity;

        // 4. Snimanje preko REPOZITORIJUMA
        await _repository.UpdateAsync(instrument);

        return Ok();
    }
    [HttpGet("convert/{rsd}")]
    public async Task<ActionResult<decimal>> GetEurPrice(decimal rsd)
    {
        var eurPrice = await _currencyService.ConvertToEur(rsd);
        return Ok(Math.Round(eurPrice, 2)); // Vraćamo na 2 decimale
    }




}