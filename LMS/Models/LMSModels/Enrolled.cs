using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public uint SId { get; set; }
        public uint ClassId { get; set; }
        public string Grade { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;
        public virtual Student SIdNavigation { get; set; } = null!;
    }
}
