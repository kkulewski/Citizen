using Citizen.Models;
using System.Linq;

namespace Citizen.DAL
{
    public interface ICountryService
    {
        IQueryable<Country> GetCountries();

        Country GetCountryById(int id);
    }
}
