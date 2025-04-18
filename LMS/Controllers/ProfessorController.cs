using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            return Json(null);
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var assignmentsQuery = db.Assignments.Where(a => a.Category.Class.Semester == season &&
                a.Category.Class.Year == year &&
                a.Category.Class.CIdNavigation.DIdNavigation.Subject == subject &&
                a.Category.Class.CId == num);

            if(category != null)
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.Category.CatName == category);
            }

            var assignments = assignmentsQuery
                .Select(a => new
                {
                    aname = a.AName,
                    cname = a.Category.CatName,
                    due = a.DueDate,
                    submissions = a.Submissions.Count
                }).ToList();

            return Json(assignments);
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var cats = db.AssignmentCategories.Where(ac => ac.Class.CIdNavigation.CNum == num &&
                ac.Class.CIdNavigation.DIdNavigation.Subject == subject &&
                ac.Class.Semester == season && ac.Class.Year == year).Select(ac => new
                {
                    name = ac.CatName,
                    weight = ac.Weight
                }).ToList();

            return Json(cats);
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            try
            {
                var classID = db.Classes.Where(c => c.CIdNavigation.CNum == num && c.CIdNavigation.DIdNavigation.Subject == subject && c.Semester == season && c.Year == year).Select(c => c.ClassId).FirstOrDefault();

                if (classID == 0)
                {
                    return Json(new { success = false });
                }

                if (db.AssignmentCategories.Any(ac => ac.ClassId == classID && ac.CatName == category))
                {
                    return Json(new { success = false });
                }

                db.AssignmentCategories.Add(new AssignmentCategory
                {
                    CatName = category,
                    Weight = (uint)catweight,
                    ClassId = classID
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
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var categoryID = db.AssignmentCategories
                .Where(ac => ac.Class.CIdNavigation.CNum == num && ac.Class.CIdNavigation.DIdNavigation.Subject == subject &&
                ac.Class.Semester == season && ac.Class.Year == year && ac.CatName == category)
                .Select(ac => ac.CategoryId).FirstOrDefault();

            if (categoryID == 0)
            {
                return Json(new { success = false });
            }

            if (db.Assignments.Any(a => a.CategoryId == categoryID && a.AName == asgname))
            {
                return Json(new { success = false });
            }

            db.Assignments.Add(new LMS.Models.LMSModels.Assignment
            {
                AName = asgname,
                Points = (uint)asgpoints,
                DueDate = asgdue,
                AContents = asgcontents,
                CategoryId = categoryID
            });

            db.SaveChanges();
            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            return Json(null);
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            return Json(new { success = false });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            try
            {
                var classes = db.Classes
                    .Where(c => c.PIdNavigation.UId == uid)
                    .Select(c => new
                    {
                        subject = c.CIdNavigation.DIdNavigation.Subject,
                        number = c.CIdNavigation.CNum,
                        name = c.CIdNavigation.CName,
                        season = c.Semester,
                        year = c.Year
                    }).ToList();
                return Json(classes);
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }

        }
        
        /*******End code to modify********/
    }
}

public class GradeCalculator
{
    public static string CalculateGrade(LMSContext db, string uid, uint classID)
    {
        var categories = db.AssignmentCategories.Where(ac => ac.ClassId == classID).Include(ac => ac.Assignments).ToList();

        var validCategories = categories.Where(ac => ac.Assignments.Any()).ToList();

        if (!validCategories.Any())
        {
            return "--";
        }

        long totalScaledScore = 0;
        long totalWeights = 0;

        foreach(var category in validCategories)
        {
            long? categoryMaxPoints = category.Assignments.Sum(a => a.Points);
            long studentPoints = 0;

            foreach(var assignment in category.Assignments)
            {
                var submission = db.Submissions.FirstOrDefault(s => s.AssignmentId == assignment.AssignmentId && s.SIdNavigation.UId == uid);
                if (submission != null)
                {
                    studentPoints += submission?.Score ?? 0;
                }

                
            }

            double categoryPercent = categoryMaxPoints > 0 ? (double)studentPoints / (double)categoryMaxPoints : 0;
            totalScaledScore += (long)(categoryPercent * category.Weight);
            totalWeights += (long)category.Weight;
        }

        if (totalWeights == 0)
        {
            return "--";
        }

        double scalingFactor = 100.0 / (double)totalWeights;
        double finalScore = (double)totalScaledScore * scalingFactor;

        return PercentToLetterGrade(finalScore);
    }

    private static string PercentToLetterGrade(double percent)
    {
        return percent switch
        {
            >= 93 => "A",
            >= 90 => "A-",
            >= 87 => "B+",
            >= 83 => "B",
            >= 80 => "B-",
            >= 77 => "C+",
            >= 73 => "C",
            >= 70 => "C-",
            >= 67 => "D+",
            >= 63 => "D",
            >= 60 => "D-",
            _ => "F"
        };
    }

}