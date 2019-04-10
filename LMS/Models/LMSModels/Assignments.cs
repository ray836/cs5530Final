using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public DateTime? Due { get; set; }
        public int? Points { get; set; }
        public uint Category { get; set; }

        public virtual AssignmentCategories CategoryNavigation { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
