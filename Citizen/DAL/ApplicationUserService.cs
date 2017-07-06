using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Citizen.Data;
using Citizen.Models;
using Microsoft.EntityFrameworkCore;

namespace Citizen.DAL
{
    public class ApplicationUserService : IApplicationUserService
    {
        private ApplicationDbContext _dbContext;

        public ApplicationUserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ApplicationUser> GetApplicationUsers()
        {
            return _dbContext.ApplicationUsers
                .Include(p => p.Items)
                .Include(p => p.MarketplaceOffers)
                .Include(p => p.Country)
                .Include(p => p.UserStorage);
        }

        public ApplicationUser GetApplicationUserById(string id)
        {
            return GetApplicationUsers().First(u => u.Id == id);
        }
    }
}
