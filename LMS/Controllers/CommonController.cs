using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class CommonController : Controller
    {
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            try
            {
                var departments = db.Departments.Select(d => new
                {
                    name = d.DName,
                    subject = d.Subject
                }).ToList();

                return Json(departments);
            }
            catch(Exception e)
            {
                return Json(new {error = e.Message });
            }
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            try
            {
                var catalog = db.Departments.Select(d => new
                {
                    subject = d.Subject,
                    dname = d.DName,
                    courses = db.Courses.Where(c => c.DIdNavigation.Subject == d.Subject).Select(c => new
                    {
                        number = c.CNum,
                        cname = c.CName
                    }).ToList()
                }).ToList();

                return Json(catalog);
            }
            
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            try
            {
                var offerings = db.Classes
                                .Where(c => c.CIdNavigation.CNum == number && c.CIdNavigation.DIdNavigation.Subject == subject)
                                .Select(c => new
                                {
                                    season = c.Semester,
                                    year = c.Year,
                                    location = c.Location,
                                    start = c.StartTime.ToString("hh:mm:ss"),
                                    end = c.EndTime.ToString("hh:mm:ss"),
                                    fname = db.Professors.FirstOrDefault(p => p.PId == c.PId).FName,
                                    lname = db.Professors.FirstOrDefault(p => p.PId == c.PId).LName
                                }).ToList();

                return Json(offerings);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            try
            {


                var content = db.Assignments.Where(a => a.AName == asgname &&
                a.Category.CatName == category &&
                a.Category.Class.Semester == season &&
                a.Category.Class.Year == year &&
                a.Category.Class.CIdNavigation.CNum == num &&
                a.Category.Class.CIdNavigation.DIdNavigation.Subject == subject).Select(a => a.AContents).FirstOrDefault();


                return Content(content ?? "");
            }
            catch(Exception e)
            {
                return Content("");
            }
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            try
            {
                var submission = db.Submissions
                    .Where(s => s.Assignment.Category.Class.CIdNavigation.CNum == num &&
                                         s.Assignment.Category.Class.CIdNavigation.DIdNavigation.Subject == subject &&
                                         s.Assignment.Category.Class.Semester == season &&
                                         s.Assignment.Category.Class.Year == year &&
                                         s.Assignment.Category.CatName == category &&
                                         s.Assignment.AName == asgname &&
                                         s.SIdNavigation.UId == uid).Select(s => s.SContent).FirstOrDefault();


                return Content(submission ?? "");
            }
            catch (Exception e)
            {
                return Content("");
            }
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            try
            {
                var admin = db.Administrators.FirstOrDefault(a => a.UId == uid);

                if(admin != null)
                {
                    return Json(new
                    {
                        fname = admin.FName,
                        lname = admin.LName,
                        uid = admin.UId
                    });

                }


                var professor = db.Professors.FirstOrDefault(p => p.UId == uid);

                if(professor != null)
                {
                    var department = db.Departments.FirstOrDefault(d => d.DId == professor.WorksIn);

                    return Json(new
                    {
                        fname = professor.FName,
                        lname = professor.LName,
                        uid = professor.UId,
                        department = department.DName ?? ""
                    });
                }

                var student = db.Students.FirstOrDefault(p => p.UId == uid);

                if (student != null)
                {
                    var department = db.Departments.FirstOrDefault(d => d.DId == student.Major);
                    return Json(new
                    {
                        fname = student.FName,
                        lname = student.LName,
                        uid = student.UId,
                        department = department.DName ?? ""
                    });
                }

                return Json(new { sucess = false });
            }
            catch(Exception e)
            {
                return Json(new { sucess = false });
            }
        }


        /*******End code to modify********/
    }
}

