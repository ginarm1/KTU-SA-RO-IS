using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class Sponsor
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Rėmėjo pavadinimas yra privalomas")]
        [MaxLength(100)]
        [DisplayName("Pavadinimas")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Įmonės teisinės formos tipas yra privalomas")]
        [MaxLength(50)]
        [DisplayName("Įmonės teisinės formos tipas")]
        public string CompanyType { get; set; }

        [Required(ErrorMessage = "Įmonės kodas yra privalomas")]
        [MaxLength(10)]
        [DisplayName("Įmonės kodas")]
        public string CompanyCode { get; set; }

        [Required(ErrorMessage = "Įmonės PVM mokėtojo kodas yra privalomas")]
        [MaxLength(20)]
        [DisplayName("Įmonės PVM kodas")]
        public string CompanyVAT { get; set; }

        [Required(ErrorMessage = "Įmonės adresas yra privalomas")]
        [MaxLength(300)]
        [DisplayName("Adresas")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Įmonės tel. nr. yra privalomas")]
        [MaxLength(20)]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Tel. nr.")]
        public string PhoneNr { get; set; }

        [Required(ErrorMessage = "Įmonės el. paštas yra privalomas")]
        [MaxLength(200)]
        [DisplayName("El. paštas")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Įmonės vadovo vardas yra privalomas")]
        [MaxLength(50)]
        [DisplayName("Įmonės vadovo vardas")]
        public string CompanyHeadName { get; set; }

        [Required(ErrorMessage = "Įmonės vadovo pavardė yra privaloma")]
        [MaxLength(50)]
        [DisplayName("Įmonės vadovo pavardė")]
        public string CompanyHeadSurname { get; set; }

        [DisplayName("Rėmimai")]
        public ICollection<Sponsorship> Sponsorships { get; set; }

    }
}
