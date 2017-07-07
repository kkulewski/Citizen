using Citizen.Models;
using System.Linq;

namespace Citizen.DAL
{
    public interface IMarketplaceService
    {
        IQueryable<MarketplaceOffer> GetOffers();

        IQueryable<MarketplaceOffer> GetOffers(ItemType itemType);

        MarketplaceOffer GetOfferById(int id);
    }
}
