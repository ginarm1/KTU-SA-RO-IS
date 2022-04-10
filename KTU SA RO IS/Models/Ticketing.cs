using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class Ticketing
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bilieto kaina yra privaloma")]
        [Range(0, 9999)]
        [DisplayName("Kaina")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Parduotų bilietų kiekis yra privalomas")]
        [Range(0, 99999)]
        [DisplayName("Parduotų bilietų kiekis")]
        public double Count { get; set; }

        [Required]
        public Event Event { get; set; }
    }
}
