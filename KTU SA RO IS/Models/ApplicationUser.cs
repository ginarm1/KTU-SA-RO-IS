using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Required(ErrorMessage = "Vardas yra privalomas")]
        [DisplayName("Vardas")]
        public string Name { get; set; }
        [PersonalData]
        [Required(ErrorMessage = "Pavardė yra privaloma")]
        [DisplayName("Pavardė")]
        public string Surname { get; set; }

        public Representative Representative { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
