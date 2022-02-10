using System;
using System.Collections.Generic;
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
        public string Title { get; set; }

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
        [DisplayName("Ar išrinktas renginio koordinatorius?")]
        public bool? Has_coordinator { get; set; }

        [Required]
        [DisplayName("Renginio koordinatoriaus vardas")]
        public string CoordinatorName { get; set; }

        [Required]
        [DisplayName("Renginio koordinatoriaus pavardė")]
        public string CoordinatorSurname { get; set; }

        [Required]
        [DisplayName("Ar atšauktas")]
        public bool? Is_canceled { get; set; }

        [Required]
        [DisplayName("Ar viešas")]
        public bool? Is_public { get; set; }

        [Required]
        [DisplayName("Ar gyvai įvyks?")]
        public bool? Is_live { get; set; }

        [Required]
        [DisplayName("Planuojamas dalyvių kiekis")]
        public int PlannedPeopleCount { get; set; }

        [DisplayName("Atvykusių dalyvių kiekis")]
        public int PeopleCount { get; set; }

        public EventType EventType { get; set; }

        [DisplayName("Organizatorius")]
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
