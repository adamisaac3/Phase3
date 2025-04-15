using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public uint AssignmentId { get; set; }
        public uint SId { get; set; }
        public DateTime SDateTime { get; set; }
        public uint? Score { get; set; }
        public string SContent { get; set; } = null!;

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student SIdNavigation { get; set; } = null!;
    }
}
