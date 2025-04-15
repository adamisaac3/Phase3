using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Classes = new HashSet<Class>();
        }

        public uint PId { get; set; }
        public string UId { get; set; } = null!;
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public uint WorksIn { get; set; }

        public virtual Department WorksInNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
