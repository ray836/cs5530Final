using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS;
using LMS.Models;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
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
        JsonResult json_query;
        using (Team31LMSContext db = new Team31LMSContext())
        {
            var query =
            from s in db.Students
            join e in db.Enrollment on s.UId equals e.UId
            join c in db.Classes on e.ClassId equals c.Id
            join co in db.Courses on c.OfferingOf equals co.Id
            where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year
            select new { fname = s.FirstName, lname = s.LastName, uid = s.UId, dob = s.Dob, grade = e.Grade };

            json_query = Json(query.ToArray());
        }
        return json_query;
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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                from a in db.Assignments
                join ac in db.AssignmentCategories on a.Category equals ac.Id
                join c in db.Classes on ac.Class equals c.Id
                join co in db.Courses on c.OfferingOf equals co.Id
                where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year && (ac.Name == category || category == null)
                select new { aname = a.Name, cname = ac.Name, due = a.Due,
                            submissions = (from s in a.Submissions select new { id = s.Id }).Count()};

                json_query = Json(query.Distinct().ToArray());
            }

                return json_query;
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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                from co in db.Courses
                join c in db.Classes on co.Id equals c.OfferingOf
                join ac in db.AssignmentCategories on c.Id equals ac.Class
                where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year
                select new { name = ac.Name, weight = ac.Weight };

                json_query = Json(query.ToArray());
            }
                return json_query;
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
            Boolean categoryCreated = false;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                AssignmentCategories newCategories = new AssignmentCategories
                {
                    Name = category,
                    Weight = (ushort)catweight,
                    Class = (from co in db.Courses
                             join c in db.Classes on co.Id equals c.OfferingOf
                             where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year
                             select c.Id).First()
                };

                db.AssignmentCategories.Add(newCategories);
                int result = db.SaveChanges();
                categoryCreated = (result == 1);
            }
                return Json(new { success = categoryCreated });
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
            Boolean assignmentCreated = false;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                Assignments newAssignment = new Assignments
                {
                    Name = asgname,
                    Contents = asgcontents,
                    Due = asgdue,
                    Points = asgpoints,
                    Category = (from co in db.Courses
                             join c in db.Classes on co.Id equals c.OfferingOf
                             join ac in db.AssignmentCategories on c.Id equals ac.Class
                             where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year && ac.Name == category
                             select ac.Id).First()
                };

                db.Assignments.Add(newAssignment);
                int result = db.SaveChanges();
                assignmentCreated = (result == 1);
            }
            return Json(new { success = assignmentCreated });
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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                    from co in db.Courses
                    join c in db.Classes on co.Id equals c.OfferingOf
                    join ac in db.AssignmentCategories on c.Id equals ac.Class
                    join a in db.Assignments on ac.Id equals a.Category
                    join s in db.Submissions on a.Id equals s.AId
                    join st in db.Students on s.UId equals st.UId
                    where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year && ac.Name == category && a.Name == asgname
                    select new { fname = st.FirstName, lname = st.LastName, uid = st.UId, time = s.Time, score = s.Score };

                json_query = Json(query.ToArray());
            }
                return json_query;
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
            Boolean submissionGraded = false;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                Submissions submission = (from co in db.Courses
                                          join c in db.Classes on co.Id equals c.OfferingOf
                                          join ac in db.AssignmentCategories on c.Id equals ac.Class
                                          join a in db.Assignments on ac.Id equals a.Category
                                          join s in db.Submissions on a.Id equals s.AId
                                          where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year && ac.Name == category && a.Name == asgname && s.UId == uid
                                          select s).First();
                submission.Score = score;
                int result = db.SaveChanges();
                submissionGraded = (result == 1);
            }
            return Json(new { success = submissionGraded });
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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                    from c in db.Classes
                    join co in db.Courses on c.OfferingOf equals co.Id
                    where c.Teacher == uid
                    select new { subject = co.Subject, number = co.Crn, name = co.Name, season = c.SemesterSeason, year = c.SemesterYear };

                json_query = Json(query.ToArray());
            }
                return json_query;
    }


    /*******End code to modify********/

  }
}