using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public uint AssignmentId { get; set; }
        public string AName { get; set; } = null!;
        public uint? Points { get; set; }
        public string AContents { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public uint CategoryId { get; set; }

        public virtual AssignmentCategory Category { get; set; } = null!;
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
