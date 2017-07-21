using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Citizen.Models.WorkViewModels
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ItemType Product { get; set; }

        public ItemType Source { get; set; }

        [Display(Name = "Max employments")]
        public int MaxEmployments { get; set; }

        public List<Employment> Employments { get; set; }

        public List<JobOffer> JobOffers { get; set; }
    }
}
