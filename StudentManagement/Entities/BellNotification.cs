using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class BellNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? IsRead { get; set; }
        public int? NotifiId { get; set; }
        public string UserId { get; set; }

        public virtual Message Notifi { get; set; }
    }
}
