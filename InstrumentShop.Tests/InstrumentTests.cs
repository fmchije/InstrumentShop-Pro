using InstrumentShop.Services;
using InstrumentShop.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic; // REŠAVA: List<>
using System.Linq;
using System.Threading.Tasks;    // REŠAVA: Task
using Xunit;


namespace InstrumentShop.Tests
{
    public class InstrumentTests
    {
        private readonly Mock<IInstrumentRepository> _mockRepo;
        private readonly Mock<ICurrencyService> _mockCurrency;
        private readonly InstrumentsController _controller;

        public InstrumentTests()
        {
            _mockRepo = new Mock<IInstrumentRepository>();
            _mockCurrency = new Mock<ICurrencyService>();

            // Koristimo NullLogger da ne bismo morali da mock-ujemo ceo logging sistem
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<InstrumentsController>();

            _controller = new InstrumentsController(_mockRepo.Object, logger, _mockCurrency.Object);
        }
        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenPriceIsOutOfRange()
        {
            
            
            

            // Šaljemo 0, a pravilo kaže da je minimum 1
            var badDto = new InstrumentDto { Name = "Test", Price = 0 };

            // Moramo ručno dodati grešku u ModelState jer Unit Test ne pokreće 
            // automatsku validaciju koju inače radi browser/server
            _controller.ModelState.AddModelError("Price", "Cena mora biti između 1 i 10.000 €");

            // 2. ACT
            var result = await _controller.Post(badDto);

            // 3. ASSERT
            // Proveravamo da li je vratio BadRequest (400)
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            // Proveravamo da li je u odgovoru zaista poruka
            var errors = (SerializableError)badRequestResult.Value;
            Assert.Contains("Price", errors.Keys);
        }

        [Fact]
        public async Task GetAllInstruments_ShouldReturnList_WhenDatabaseHasData() 
        {
            // 1. ARRANGE
            

            var fakeInstruments = new List<Instrument>
            {
                new Instrument { Id = 1, Name = "Fender Stratocaster" },
                new Instrument { Id = 2, Name = "Gibson Les Paul" }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(fakeInstruments);

            
           

            // 2. ACT
            var actionResult = await _controller.Get();

            // 3. ASSERT
            // Izvlačimo rezultat i kastujemo ga direktno u listu DTO-ova
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            // Koristimo .Cast<InstrumentDto>().ToList() da budemo 100% sigurni da Count() radi
            var lista = (okResult.Value as IEnumerable<InstrumentDto>);

            Assert.NotNull(lista);
            Assert.Equal(2, lista.Count());
            _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Get_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
        {
            

            // Simuliramo praznu listu iz baze
            var emptyList = new List<Instrument>();

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyList);

            
            
            

            // 2. ACT
            var actionResult = await _controller.Get();

            // 3. ASSERT
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var lista = okResult.Value as IEnumerable<InstrumentDto>;

            Assert.NotNull(lista);
            Assert.Empty(lista); // Proveravamo da li je lista stvarno prazna
            Assert.Equal(0, lista.Count());
        }
        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenInstrumentDoesNotExist()
        {
            // 1. ARRANGE
            

            // Kažemo Moq-u: "Kad god neko traži ID 999, vrati null" (kao da ga nema u bazi)
            _mockRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Instrument)null);

            
            

            // 2. ACT
            var result = await _controller.Delete(999);

            // 3. ASSERT
            // Proveravamo da li je kontroler pametan i da li je vratio 404
            Assert.IsType<NotFoundResult>(result);

            // Proveravamo da metoda za brisanje NIKADA nije pozvana (jer nema šta da se briše)
            _mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Instrument>()), Times.Never);
        }
        [Fact]
        public async Task Convert_ShouldReturnCorrectValue()
        {
            // ARRANGE
            decimal rsdAmount = 1172m;
            decimal expectedEur = 10m;
            
            _mockCurrency.Setup(s => s.ConvertToEur(rsdAmount)).ReturnsAsync(expectedEur);

            // ACT
            var result = await _controller.GetEurPrice(rsdAmount);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedEur, okResult.Value);
        }
    }
}