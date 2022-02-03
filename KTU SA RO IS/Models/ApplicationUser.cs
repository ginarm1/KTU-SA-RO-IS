using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Required]
        public string Name { get; set; }
        [PersonalData]
        [Required]
        public string Surname { get; set; }

        public Representative Representative { get; set; }
    }
}
