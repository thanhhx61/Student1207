using StudentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class Student
    {
        public List<User> Users { get; set; }
        public int? SchoolYearId { get; set; }
        public int EventId { get; set; }
        public string Hastag { get; set; }
        public string StudentName { get; set; }
        public string Style{ get; set; }
        public string SkillName { get; set; }
        public string UserId { get; set; }
    }
}
