using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public string CName { get; set; } = null!;
        public uint CNum { get; set; }
        public uint DId { get; set; }
        public uint CId { get; set; }

        public virtual Department DIdNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
