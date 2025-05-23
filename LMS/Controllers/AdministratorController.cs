﻿using System;
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
    public class AdministratorController : Controller
    {
        private readonly LMSContext db;

        public AdministratorController(LMSContext _db)
        {
            db = _db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Create a department which is uniquely identified by it's subject code
        /// </summary>
        /// <param name="subject">the subject code</param>
        /// <param name="name">the full name of the department</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the department already exists, true otherwise.</returns>
        public IActionResult CreateDepartment(string subject, string name)
        {
            try
            {


                if (db.Departments.Any(d => d.Subject == subject))
                {
                    return Json(new { success = false });
                }

                db.Departments.Add(new Department
                {
                    Subject = subject,
                    DName = name

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
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subjCode">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            try
            {
                var courses = db.Courses
                .Where(c => c.DIdNavigation.Subject == subject).OrderBy(c => c.CNum)
                .Select(c => new
                {
                    number = c.CNum,
                    name = c.CName
                }).ToList();

                return Json(courses);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            try{
                var professors = db.Professors.Where(p => p.WorksInNavigation.Subject == subject).OrderBy(p => p.LName).Select(p => new
                {
                    lname = p.LName,
                    fname = p.FName,
                    uid = p.UId
                }).ToList();
                return Json(professors);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            try
            {
                if (db.Courses.Any(c => c.CNum == number && c.CName == name))
                {
                    return Json(new { success = false });
                }

                db.Courses.Add(new Course
                {
                    CNum = (uint)number,
                    CName = name,
                    DId = db.Departments.FirstOrDefault(d => d.Subject == subject).DId
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
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            try
            {


                if (end <= start)
                {
                    return Json(new { success = false });
                }
                TimeOnly startTime = TimeOnly.FromDateTime(start);
                TimeOnly endTime = TimeOnly.FromDateTime(end);

                var course = db.Courses.FirstOrDefault(c => c.CNum == number && c.DIdNavigation.Subject == subject);
                if (course == null)
                {
                    return Json(new { success = false });
                }

                if (db.Classes.Any(c => c.CIdNavigation.CNum == number && c.Semester == season && c.Year == year))
                {
                    return Json(new { success = false });
                }

                bool existingConflict = db.Classes.Any(c => c.Location == location && c.Semester == season && c.Year == year &&
                    ((startTime >= c.StartTime && startTime < c.EndTime) ||
                     (endTime > c.StartTime && endTime <= c.EndTime) ||
                     (startTime < c.StartTime && endTime > c.EndTime)));

                if (existingConflict)
                {
                    return Json(new { success = false });
                }

                db.Classes.Add(new Class
                {
                    Semester = season,
                    Year = (uint)year,
                    Location = location,
                    StartTime = startTime,
                    EndTime = endTime,
                    CId = course.CId,
                    PId = db.Professors.FirstOrDefault(p => p.UId == instructor).PId
                });

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch(Exception e)
            {
                return Json(new { success = false });
            }
        }


        /*******End code to modify********/

    }
}

