using Microsoft.EntityFrameworkCore;
using InstrumentShop.Shared;

namespace InstrumentShop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Ovo će postati tabela u SQL-u
        public DbSet<Instrument> Instruments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instrument>().HasData(
                new Instrument { Id = 1, Name = "Fender Stratocaster", Price = 1200, StockQuantity = 5 },
                new Instrument { Id = 2, Name = "Gibson Les Paul", Price = 2500, StockQuantity = 2 },
                new Instrument { Id = 3, Name = "Yamaha FG800", Price = 350, StockQuantity = 10 },
                new Instrument { Id = 4, Name = "Ibanez SR300", Price = 500, StockQuantity = 7 }
            );
        }
    }
}