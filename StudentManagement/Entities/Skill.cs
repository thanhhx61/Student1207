using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class Skill
    {
        public Skill()
        {
            MessagesSkills = new HashSet<MessagesSkill>();
        }

        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Style { get; set; }

        public virtual ICollection<MessagesSkill> MessagesSkills { get; set; }
    }
}
