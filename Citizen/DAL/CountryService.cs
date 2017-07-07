using System.Linq;
using Citizen.Data;
using Citizen.Models;

namespace Citizen.DAL
{
    public class CountryService : ICountryService
    {
        private ApplicationDbContext _dbContext;

        public CountryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Country> GetCountries()
        {
            return _dbContext.Country;
        }

        public Country GetCountryById(int id)
        {
            return GetCountries().First(u => u.Id == id);
        }
    }
}
