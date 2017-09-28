using System.Linq;
using Citizen.Models;

namespace Citizen.Data
{
    public class ApplicationDbInitializer
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbInitializer(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            _context.Database.EnsureCreated();

            // Check if db table contains any entries
            if (_context.Country.Any())
            {
                return; // it does => no need for seed
            }

            var countries = new[]
            {
                new Country {Capital = "Warsaw", Name = "Poland", CurrencyCode = "PLN"},
                new Country {Capital = "Berlin", Name = "Germany", CurrencyCode = "EUR"},
                new Country {Capital = "Moscow", Name = "Russia", CurrencyCode = "RUB"}
            };

            foreach (var country in countries)
            {
                _context.Country.Add(country);
            }

            _context.SaveChanges();
        }
    }
}
