using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
        }

        public uint Id { get; set; }
        public string SemesterSeason { get; set; }
        public int? SemesterYear { get; set; }
        public uint OfferingOf { get; set; }
        public string Location { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Teacher { get; set; }

        public virtual Courses OfferingOfNavigation { get; set; }
        public virtual Professors TeacherNavigation { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollment> Entrollments { get; set; }
    }
}
