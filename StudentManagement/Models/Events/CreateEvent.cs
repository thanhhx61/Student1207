using StudentManagement.Entities;
using StudentManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models.Events
{
    public class CreateEvent
    {
        [Required(ErrorMessage = "フィールドは必須です。")]
        public string Activities { get; set; }
        [Required(ErrorMessage = "フィールドは必須です。")]
        public string Act { get; set; }
        [Required(ErrorMessage = "フィールドは必須です。")]
        public string PowerExerted { get; set; }
        [Required(ErrorMessage = "フィールドは必須です。")]
        public string PowerDev { get; set; }
        [Required(ErrorMessage = "フィールドは必須です。")]
        public string Think { get; set; }
        [Required(ErrorMessage = "フィールドは必須です。")]
        public int? ListEventId { get; set; }
        public EventStatus? Status { get; set; }
        public int SchoolYearId { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ListEvent ListEvent { get; set; }
    }
}
