using InstrumentShop.Shared;

namespace InstrumentShop
{
    public interface IInstrumentRepository
    {
        Task<IEnumerable<Instrument>> GetAllAsync();
        Task<Instrument> GetByIdAsync(int id); // Za Delete i Update
        Task AddAsync(Instrument instrument);
        Task UpdateAsync(Instrument instrument);
        Task DeleteAsync(Instrument instrument);


    }
}
