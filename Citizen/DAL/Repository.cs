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
        private IMarketplaceService _marketplaceService;

        public Repository(
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _applicationUserService = new ApplicationUserService(dbContext);
            _countryService = new CountryService(dbContext);
            _marketplaceService = new MarketplaceService(dbContext);
        }

        public IApplicationUserService ApplicationUserService => _applicationUserService;

        public ICountryService CountryService => _countryService;

        public IMarketplaceService MarketplaceService => _marketplaceService;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
