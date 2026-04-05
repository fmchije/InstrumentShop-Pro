using InstrumentShop.Data;
using InstrumentShop.Shared;
using Microsoft.EntityFrameworkCore;

namespace InstrumentShop;
public class InstrumentRepository : IInstrumentRepository
{
    private readonly AppDbContext _context;

    public InstrumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Instrument>> GetAllAsync()
    {
        return await _context.Instruments.AsNoTracking().ToListAsync();
    }
    public async Task<Instrument> GetByIdAsync(int id) => await _context.Instruments.FindAsync(id);

    public async Task AddAsync(Instrument instrument)
    {
        _context.Instruments.Add(instrument);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Instrument instrument)
    {
        _context.Instruments.Update(instrument);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Instrument instrument)
    {
        _context.Instruments.Remove(instrument);
        await _context.SaveChangesAsync();
    }
}
