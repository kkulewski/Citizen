using System.ComponentModel.DataAnnotations;

namespace Citizen.Models.MarketplaceViewModels
{
    public class AddJobOfferViewModel
    {
        [Required]
        public int CompanyId { get; set; }

        public Company Company { get; set; }

        [Required]
        public decimal Salary { get; set; }
    }
}
