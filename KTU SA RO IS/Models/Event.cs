using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTU_SA_RO.Models
{
    public class Event
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Renginio pavadinimas yra privalomas")]
        [MaxLength(300)]
        [DisplayName("Pavadinimas")]

        public string Title { get; set; }

        [Required(ErrorMessage = "Renginio pradžios data yra privaloma")]
        [DisplayName("Renginio pradžia")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Renginio pabaigos data yra privaloma")]
        [DisplayName("Renginio pabaiga")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [MaxLength(100)]
        [DisplayName("Vieta")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Renginio aprašymas yra privalomas")]
        [DisplayName("Aprašymas")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Ar išrinktas renginio koordinatorius?")]
        public bool Has_coordinator { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Renginio koordinatoriaus vardas")]
        public string CoordinatorName { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Renginio koordinatoriaus pavardė")]
        public string CoordinatorSurname { get; set; }

        [Required]
        [DisplayName("Ar atšauktas")]
        public bool Is_canceled { get; set; }

        [Required]
        [DisplayName("Ar įvyks gyvai?")]
        public bool Is_live { get; set; }

        [Required(ErrorMessage = "Renginyje planuojamų dalyvių kiekis yra privalomas")]
        [Range(0,99999, ErrorMessage = "Netinkamos ribos")]
        [DisplayName("Planuojamas dalyvių kiekis")]
        public int PlannedPeopleCount { get; set; }

        [DisplayName("Atvykusių dalyvių kiekis")]
        public int PeopleCount { get; set; }

        [Required]
        public EventType EventType { get; set; }

        //[DisplayName("Organizatorius")]
        //public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<EventTeamMember> EventTeamMembers { get; set; }

        [DisplayName("Spec. reikalavimai")]
        public ICollection<Requirement> Requirements { get; set; }

        [DisplayName("Rėmimai")]
        public ICollection<Sponsorship> Sponsorships { get; set; }

        [DisplayName("Pajamos")]
        public ICollection<Revenue> Revenues { get; set; }

        [DisplayName("Išlaidos")]
        public ICollection<Cost> Costs { get; set; }
        [DisplayName("Bilietacijos")]
        public ICollection<Ticketing> Ticketings { get; set; }
    }
}
