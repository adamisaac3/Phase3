using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrolleds = new HashSet<Enrolled>();
            Submissions = new HashSet<Submission>();
        }

        public uint SId { get; set; }
        public string UId { get; set; } = null!;
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public uint Major { get; set; }

        public virtual Department MajorNavigation { get; set; } = null!;
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
