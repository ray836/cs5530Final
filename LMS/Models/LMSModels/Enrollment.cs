using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrollment
    {
        public uint Id { get; set; }
        public string Grade { get; set; }
        public string UId { get; set; }
        public uint? ClassId { get; set; }

        public virtual Students Student { get; set; }
        public virtual Classes Class { get; set; }
    }
}
