﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KTU_SA_RO.Models
{
    public class EventType
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Event> Event { get; set; }

    }
}