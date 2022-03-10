using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class EventType
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [DisplayName("Renginio tipas")]
        [MaxLength(100)]
        [Required(ErrorMessage = "Renginio tipo pavadinimas yra privalomas")]
        public string Name { get; set; }

        public ICollection<Event> Event { get; set; }

    }
}
