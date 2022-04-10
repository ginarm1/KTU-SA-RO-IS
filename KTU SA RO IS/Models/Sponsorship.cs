using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class Sponsorship
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Rėmimo aprašymas yra privalomas")]
        [MaxLength(300)]
        [DisplayName("Aprašymas")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Rėmimo kiekis yra privalomas")]
        [Range(0,99999,ErrorMessage = "Netinkamos ribos")]
        [DisplayName("Kiekis")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Rėmimo kainos suma yra privalomas")]
        [Range(0, 999999, ErrorMessage = "Netinkamos ribos")]
        [DisplayName("Suma")]
        public double CostTotal { get; set; }

        [DisplayName("Rėmėjas")]
        [Required]
        public Sponsor Sponsor { get; set; }

        [DisplayName("Renginys")]
        [Required]
        public Event Event { get; set; }
    }
}
