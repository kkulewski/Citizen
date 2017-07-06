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

        public Repository(
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _applicationUserService = new ApplicationUserService(dbContext);
        }

        public IApplicationUserService ApplicationUserService() => _applicationUserService;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
