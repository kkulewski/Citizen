using Citizen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.DAL
{
    public class Repository : IRepository
    {
        private ApplicationDbContext _dbContext;
        private IApplicationUserService _applicationUserService;
        private ICountryService _countryService;

        public Repository(
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _applicationUserService = new ApplicationUserService(dbContext);
            _countryService = new CountryService(dbContext);
        }

        public IApplicationUserService ApplicationUserService => _applicationUserService;

        public ICountryService CountryService => _countryService;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
