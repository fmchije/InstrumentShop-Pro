namespace InstrumentShop.Services
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertToEur(decimal rsdAmount);
    }

    public class CurrencyService : ICurrencyService
    {
        public async Task<decimal> ConvertToEur(decimal rsdAmount)
        {
            // Ovde simuliramo taj SOAP poziv koji traže u oglasu
            // U pravom sistemu, ovde bi išao XML zahtev ka Narodnoj Banci Srbije
            decimal kurs = 117.2m;

            // Simuliramo asinhroni rad (kao da mreža radi)
            await Task.Delay(100);

            return rsdAmount / kurs;
        }
    }
}