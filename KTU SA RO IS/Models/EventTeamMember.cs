using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class EventTeamMember
    {
        [Key]
        [Required]
        public int Id { get; set; }
        //public ICollection<Event> Events { get; set; }

        //[ForeignKey("Events")]
        public int EventId { get; set; }
        //public ICollection<ApplicationUser> Users { get; set; }

        //[ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        [Required]
        [DisplayName("Naudotojo rolė")]
        public string RoleName { get; set; }
        public bool Is_event_coord { get; set; }
    }
}
