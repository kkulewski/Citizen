using System.ComponentModel.DataAnnotations;

namespace Citizen.Models.WorkViewModels
{
    public class DeleteCompanyViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public ItemType Product { get; set; }

        [Required]
        public ItemType Source { get; set; }
    }
}
