using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class Cost
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Išlaidos pavadinimas yra privalomas")]
        [MaxLength(300)]
        [DisplayName("Pavadinimas")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Išlaidos kaina yra privaloma")]
        [Range(0,9999)]
        [DisplayName("Kaina")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Išlaidos sąskaitos faktūros nr. yra privalomas")]
        [MaxLength(50)]
        [DisplayName("Sąskaitos faktūros nr.")]
        public string InvoiceNr { get; set; }

        [Required(ErrorMessage = "Išlaidos data yra privaloma")]
        [DisplayName("Data")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [MaxLength(300)]
        [DisplayName("Komentaras")]
        public string Comment { get; set; }

        [Required]
        public Event Event { get; set; }
    }
}
