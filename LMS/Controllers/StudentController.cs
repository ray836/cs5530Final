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
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                    from e in db.Enrollment
                    join c in db.Classes on e.ClassId equals c.Id
                    join co in db.Courses on c.OfferingOf equals co.Id
                    where e.UId == uid
                    select new { subject = co.Subject, number = co.Crn, name = co.Name, season = c.SemesterSeason, year = c.SemesterYear, grade = e.Grade };

                json_query = Json(query.ToArray());
            }
                return json_query;
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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                    from e in db.Enrollment
                    join c in db.Classes on e.ClassId equals c.Id
                    join co in db.Courses on c.OfferingOf equals co.Id
                    join ac in db.AssignmentCategories on c.Id equals ac.Class
                    join a in db.Assignments on ac.Id equals a.Category
                    join s in db.Submissions on a.Id equals s.AId into sub
                    from s in sub.DefaultIfEmpty()
                    where e.UId == uid && co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year
                    select new { aname = a.Name, cname = ac.Name, due = a.Due, score = s == null ? null : s.Score };

                json_query = Json(query.ToArray());
            }
                return json_query;
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
            Boolean submitted = false;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                Submissions submission =
                    (from s in db.Submissions
                     join a in db.Assignments on s.AId equals a.Id
                     join ac in db.AssignmentCategories on a.Category equals ac.Id
                     join c in db.Classes on ac.Class equals c.Id
                     join co in db.Courses on c.OfferingOf equals co.Id
                     where s.UId == uid && a.Name == asgname && ac.Name == category && c.SemesterSeason == season && c.SemesterYear == year && co.Subject == subject && co.Crn == num
                     select s).FirstOrDefault();

                if (submission == null)
                {
                    Submissions newSubmission = new Submissions
                    {
                        UId = uid,
                        Score = 0,
                        Contents = contents,
                        Time = DateTime.Now,
                        AId = (from a in db.Assignments
                               join ac in db.AssignmentCategories on a.Category equals ac.Id
                               join c in db.Classes on ac.Class equals c.Id
                               join co in db.Courses on c.OfferingOf equals co.Id
                               where a.Name == asgname && ac.Name == category && c.SemesterSeason == season && c.SemesterYear == year && co.Subject == subject && co.Crn == num
                               select a.Id).First()
                    };

                    db.Submissions.Add(newSubmission);
                    int result = db.SaveChanges();
                    submitted = (result == 1);
                } else
                {
                    submission.Contents = contents;
                    submission.Time = DateTime.Now;
                    int result = db.SaveChanges();
                    submitted = (result == 1);
                }
            }
            return Json(new { success = submitted });
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
            Boolean enrolled = false;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                    from e in db.Enrollment
                    join c in db.Classes on e.ClassId equals c.Id
                    join co in db.Courses on c.OfferingOf equals co.Id
                    where e.UId == uid && co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year
                    select e;

                if (query.Count() > 0)
                {
                    enrolled = false;
                } else
                {
                    Enrollment newEnrollment = new Enrollment
                    {
                        UId = uid,
                        Grade = "--",
                        ClassId = (from c in db.Classes
                                   join co in db.Courses on c.OfferingOf equals co.Id
                                   where co.Subject == subject && co.Crn == num && c.SemesterSeason == season && c.SemesterYear == year
                                   select c.Id).First()
                    };

                    db.Enrollment.Add(newEnrollment);
                    int result = db.SaveChanges();
                    enrolled = (result == 1);
                }
            }
            return Json(new { success = enrolled });
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
            JsonResult json_query;
            using (Team31LMSContext db = new Team31LMSContext())
            {
                var query =
                    from e in db.Enrollment
                    where e.UId == uid && e.Grade != null && e.Grade != "--"
                    select GPAFeed(e.Grade);

                if (query.Count() == 0)
                {
                    json_query = Json(new { gpa = 0.0 });
                } else
                {
                    // Taking a straight average because all courses are 4 credit hours
                    json_query = Json(new { gpa = query.Average() });
                }
            }
                return json_query;
    }

   
    /// <summary>
    /// Takes in a letter grade and returns the GPA for that grade
    /// </summary>
    /// <param name="grade"></param>
    /// <returns></returns>
    private Double GPAFeed(string grade)
    {
        Double gpa = 0.0;
        switch (grade)
        {
            case "A+":
            case "A":
                gpa = 4.0;
                break;
            case "A-":
                gpa = 3.7;
                break;
            case "B+":
                gpa = 3.3;
                break;
            case "B":
                gpa = 3.0;
                break;
            case "B-":
                gpa = 2.7;
                break;
            case "C+":
                gpa = 2.3;
                break;
            case "C":
                gpa = 2.0;
                break;
            case "C-":
                gpa = 1.7;
                break;
            case "D+":
                gpa = 1.3;
                break;
            case "D":
                gpa = 1.0;
                break;
            case "D-":
                gpa = 0.7;
                break;
            case "E":
                gpa = 0.0;
                break;               

        }
        return gpa;
    }

    /*******End code to modify********/

  }
}