using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTU_SA_RO.Models
{
    public class EventTeam
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public Event Events { get; set; }

        [ForeignKey("Events")]
        public int EventId { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        public bool Is_event_coord { get; set; }
    }
}
