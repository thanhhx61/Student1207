using StudentManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class Event
    {
        public Event()
        {
            Messages = new HashSet<Message>();
        }

        public int EventId { get; set; }
        public string UserId { get; set; }
        [Required]
        public string Activities { get; set; }
        [Required]
        public string Act { get; set; }
        [Required]
        public string PowerExerted { get; set; }
        [Required]
        public string PowerDev { get; set; }
        [Required]
        public string Think { get; set; }
        [Required]
        public int? ListEventId { get; set; }
        public EventStatus? Status { get; set; }
        public int SchoolYearId { get; set; }

        public virtual ListEvent ListEvent { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
