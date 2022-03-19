using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class Requirement
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Reikalavimo pavadinimas yra privalomas")]
        [MaxLength(300)]
        [DisplayName("Pavadinimas")]
        public string Name { get; set; }

        //[MaxLength(200)]
        //[DisplayName("Kategorija")]
        //public string Category { get; set; }

        [Required(ErrorMessage = "Reikalavimo aprašymas yra privalomas")]
        [DisplayName("Aprašymas")]
        public string Description { get; set; }

        [DisplayName("Komentaras")]
        public string Comment { get; set; }

        [DisplayName("Yra bendrinis")]
        public bool Is_general { get; set; }

        [DisplayName("Yra įvykdytas")]
        public bool Is_fulfilled { get; set; }

        public Event Event { get; set; }
    }
}
