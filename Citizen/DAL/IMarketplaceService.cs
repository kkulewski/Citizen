using Citizen.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.DAL
{
    public interface IMarketplaceService
    {
        IQueryable<MarketplaceOffer> GetOffers();

        IQueryable<MarketplaceOffer> GetOffers(ItemType itemType);

        Task<MarketplaceOffer> GetOfferById(int id);
    }
}
