using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Entities
{
    public class MessagesSkill
    {
        public int MessagesId { get; set; }
        public int SkillId { get; set; }

        public virtual Message Messages { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
