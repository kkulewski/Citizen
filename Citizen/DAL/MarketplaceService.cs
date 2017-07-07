using Citizen.Data;
using Citizen.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Citizen.DAL
{
    public class MarketplaceService : IMarketplaceService
    {
        private ApplicationDbContext _dbContext;

        public MarketplaceService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<MarketplaceOffer> GetOffers()
        {
            return _dbContext.MarketplaceOffers
                .Include(m => m.ApplicationUser);
        }

        public IQueryable<MarketplaceOffer> GetOffers(ItemType itemType)
        {
            return GetOffers().Where(m => m.ItemType == itemType);
        }

        public MarketplaceOffer GetOfferById(int id)
        {
            return GetOffers().First(m => m.Id == id);
        }
    }
}
