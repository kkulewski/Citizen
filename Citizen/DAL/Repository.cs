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
        private IApplicationUserRepo _applicationUserRepo;

        public Repository(
            ApplicationDbContext dbContext,
            IApplicationUserRepo applicationUserRepo)
        {
            _dbContext = dbContext;
            _applicationUserRepo = applicationUserRepo;
        }

        public IApplicationUserRepo ApplicationUserRepo() => _applicationUserRepo;
    }
}
