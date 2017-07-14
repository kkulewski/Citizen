using System.Collections.Generic;

namespace Citizen.Models.WorkViewModels
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ItemType Product { get; set; }

        public ItemType Source { get; set; }

        public int MaxEmployments { get; set; }

        public List<Employment> Employments { get; set; }

        public List<JobOffer> JobOffers { get; set; }
    }
}
