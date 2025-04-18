using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var classes = db.Enrolleds
                .Where(e => e.SIdNavigation.UId == uid)
                .Join(
                    db.Classes,
                    e => e.ClassId,
                    c => c.ClassId,
                    (e, c) => new
                    {
                        subject = c.CIdNavigation.DIdNavigation.Subject,
                        number = c.CIdNavigation.CNum,
                        name = c.CIdNavigation.CName,
                        season = c.Semester,
                        year = c.Year,
                        grade = e.Grade
                    }
                )
                .ToList();

            return Json(classes);
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var assignments = db.Assignments.Where(a => a.Category.Class.CIdNavigation.DIdNavigation.Subject == subject &&
                a.Category.Class.CIdNavigation.CNum == num &&
                a.Category.Class.Semester == season &&
                a.Category.Class.Year == year &&
                db.Enrolleds.Any(s => s.Class == a.Category.Class && s.SIdNavigation.UId == uid)).Select(a => new
                {
                    aname = a.AName,
                    cname = a.Category.CatName,
                    due = a.DueDate,
                    score = db.Submissions.Where(s => s.AssignmentId == a.AssignmentId && s.SIdNavigation.UId == uid)
                    .Select(s => s.Score).FirstOrDefault()
                }).ToList();

            return Json(assignments);
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {           
            return Json(new { success = false });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            try
            {
                var classToEnroll = db.Classes.FirstOrDefault(c => c.CIdNavigation.DIdNavigation.Subject == subject &&
                c.CIdNavigation.CNum == num &&
                c.Semester == season &&
                c.Year == year
                );

                if(classToEnroll == null)
                {
                    return Json(new { success = false });
                }

                if(db.Enrolleds.Any(e => e.ClassId == classToEnroll.ClassId && e.SIdNavigation.UId == uid))
                {
                    return Json(new { success = false });
                }
                db.Enrolleds.Add(new Enrolled
                {
                    ClassId = classToEnroll.ClassId,
                    SId = db.Students.FirstOrDefault(s => s.UId == uid).SId,
                    Grade = "--"
             
                });
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch(Exception e)
            {
                return Json(new { success = false });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {            
            var gradePoint = new Dictionary<string, double>
            {
               {"A", 4.0},
               {"A-", 3.7},
               {"B+", 3.3},
               {"B", 3.0},
               {"B-", 2.7},
               {"C+", 2.3},
               {"C", 2.0},
               {"C-", 1.7},
               {"D+", 1.3},
               {"D", 1.0},
               {"D-", 0.7},
               {"E", 0.0}
            };

            var grades = db.Enrolleds.Where(s => s.SIdNavigation.UId == uid && s.Grade != "--").Select(s => s.Grade).ToList();

            if(grades.Count == 0)
            {
                return Json(new { gpa = 0.0 });
            }

            double totalPoints = 0;
            foreach(var grade in grades)
            {
                if(gradePoint.TryGetValue(grade, out double points))
                {
                    totalPoints += points;
                }
            }

            double gpa = totalPoints / grades.Count;
            return Json(new { gpa = Math.Round(gpa, 2) });
        }
                
        /*******End code to modify********/

    }
}

