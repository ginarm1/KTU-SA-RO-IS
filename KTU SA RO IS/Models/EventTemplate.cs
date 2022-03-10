using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class EventTemplate
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Path { get; set; }
    }
}
