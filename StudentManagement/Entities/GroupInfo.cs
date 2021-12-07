using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Entities
{
    public partial class GroupInfo
    {
        public int GroupInfoId { get; set; }
        public int EventId { get; set; }
        public string ConnectionId {get;set;}

        public string Name { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }

        public virtual ICollection<GroupInfo> GroupInfos { get; set; }
    }
}
