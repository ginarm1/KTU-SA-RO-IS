using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class Event
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Pavadinimas")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Renginio pradžia")]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayName("Renginio pabaiga")]
        public DateTime EndDate { get; set; }

        [DisplayName("Vieta")]
        public string Location { get; set; }

        [Required]
        [DisplayName("Aprašymas")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Ar atšauktas")]
        public bool? Is_canceled { get; set; }

        [Required]
        [DisplayName("Ar viešas")]
        public bool? Is_public { get; set; }

        [Required]
        [DisplayName("Ar gyvas?")]
        public bool? Is_live { get; set; }

        [Required]
        [DisplayName("Planuojamas dalyvių kiekis")]
        public int PlannedPeopleCount { get; set; }

        [Required]
        [DisplayName("Planuojamas dalyvių kiekis")]
        public int PeopleCount { get; set; }

        public EventType EventType { get; set; }
    }
}
