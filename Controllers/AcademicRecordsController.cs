using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Models.DataAccess;
using Microsoft.AspNetCore.Http;

namespace Lab5.Controllers
{
    public class AcademicRecordsController : Controller
    {
        private readonly StudentRecordContext _context;

        public AcademicRecordsController(StudentRecordContext context)
        {
            _context = context;
        }

        // GET: AcademicRecords
        public async Task<IActionResult> Index(string? sort)
        {
            string order = "";
            if (HttpContext.Session.GetString("sort") == sort)
            {
                if (HttpContext.Session.GetString("order") == null || HttpContext.Session.GetString("order") == "desc")
                {
                    order = "asc";
                    HttpContext.Session.SetString("order", order);
                }
                else
                {
                    order = "desc";
                    HttpContext.Session.SetString("order", order);
                }
            }
            else
            {
                HttpContext.Session.SetString("sort", sort);
            }

            if (sort=="course")
            {
                IOrderedQueryable<AcademicRecord> list;
                if(order == "desc")
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderByDescending(a => a.CourseCode);
                }
                else
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(a => a.CourseCode);
                }
                return View(await list.ToListAsync());
            }
            else if (sort == "student")
            {
                IOrderedQueryable<AcademicRecord> list;
                if (order == "desc")
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderByDescending(a => a.StudentId);
                }
                else
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(a => a.StudentId);
                }
                return View(await list.ToListAsync());
            }
            else if (sort == "grade")
            {
                IOrderedQueryable<AcademicRecord> list;
                if (order == "desc")
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderByDescending(a => a.Grade);
                }
                else
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(a => a.Grade);
                }
                return View(await list.ToListAsync());
            }

            var studentRecordContext = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student);
            return View(await studentRecordContext.ToListAsync());
        }


        // GET: AcademicRecords/Create
        public IActionResult Create()
        {
            List<object> courses = new List<object>();
            foreach (var course in _context.Course)
            {
                courses.Add(new
                {
                    Code = course.Code,
                    Title = course.Code + " - " + course.Title
                });
            }

            ViewData["CourseCode"] = new SelectList(courses, "Code", "Title");

            List<object> students = new List<object>();
            foreach (var student in _context.Student)
            {
                students.Add(new
                {
                    Id = student.Id,
                    Name = student.Id + " - " + student.Name
                });
            }

            ViewData["StudentId"] = new SelectList(students, "Id", "Name");

            return View();
        }

        // POST: AcademicRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,StudentId,Grade")] AcademicRecord academicRecord)
        {
            var existing = _context.AcademicRecord.Where(a => a.CourseCode.Equals(academicRecord.CourseCode) && a.StudentId.Equals(academicRecord.StudentId)).FirstOrDefault();

            ViewData["Error"] = "";
            if (existing != null)
            {
                ViewData["Error"] = "The student already has an academic record for the course.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _context.Add(academicRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            List<object> courses = new List<object>();
            foreach (var course in _context.Course)
            {
                courses.Add(new
                {
                    Code = course.Code,
                    Title = course.Code + " - " + course.Title
                });
            }

            ViewData["CourseCode"] = new SelectList(courses, "Code", "Title");

            List<object> students = new List<object>();
            foreach (var student in _context.Student)
            {
                students.Add(new
                {
                    Id = student.Id,
                    Name = student.Id + " - " + student.Name
                });
            }

            ViewData["StudentId"] = new SelectList(students, "Id", "Name");
            return View(academicRecord);
        }

        // GET: AcademicRecords/Edit/5
        public async Task<IActionResult> Edit(string course, string student)
        {
            if (course == null || student == null)
            {
                return NotFound();
            }

            var academicRecord = _context.AcademicRecord.Where(a => a.CourseCode.Equals(course) && a.StudentId.Equals(student)).FirstOrDefault();
            if (academicRecord == null)
            {
                return NotFound();
            }
            var selectedCourse = _context.Course.Where(c => c.Code.Equals(course)).FirstOrDefault();
            var selectedStudent = _context.Student.Where(s => s.Id.Equals(student)).FirstOrDefault();
            ViewData["CourseCode"] = academicRecord.CourseCode;
            ViewData["CourseName"] = selectedCourse.Title;
            ViewData["StudentId"] = academicRecord.StudentId;
            ViewData["StudentName"] = selectedStudent.Name;
            return View(academicRecord);
        }

        // POST: AcademicRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CourseCode,StudentId,Grade")] AcademicRecord academicRecord)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicRecordExists(academicRecord.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseCode"] = new SelectList(_context.Course, "Code", "Code", academicRecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id", academicRecord.StudentId);
            return View(academicRecord);
        }

        private bool AcademicRecordExists(string id)
        {
            return _context.AcademicRecord.Any(e => e.StudentId == id);
        }

        [HttpGet]
        public async Task<IActionResult> EditAll(string? sort)
        {
            string order = "";
            if (HttpContext.Session.GetString("sort") == sort)
            {
                if (HttpContext.Session.GetString("order") == null || HttpContext.Session.GetString("order") == "desc")
                {
                    order = "asc";
                    HttpContext.Session.SetString("order", order);
                }
                else
                {
                    order = "desc";
                    HttpContext.Session.SetString("order", order);
                }
            }
            else
            {
                HttpContext.Session.SetString("sort", sort);
            }

            if (sort == "course")
            {
                IOrderedQueryable<AcademicRecord> list;
                if (order == "desc")
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderByDescending(a => a.CourseCode);
                }
                else
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(a => a.CourseCode);
                }
                return View(await list.ToListAsync());
            }
            else if (sort == "student")
            {
                IOrderedQueryable<AcademicRecord> list;
                if (order == "desc")
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderByDescending(a => a.StudentId);
                }
                else
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(a => a.StudentId);
                }
                return View(await list.ToListAsync());
            }
            else if (sort == "grade")
            {
                IOrderedQueryable<AcademicRecord> list;
                if (order == "desc")
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderByDescending(a => a.Grade);
                }
                else
                {
                    list = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student).OrderBy(a => a.Grade);
                }
                return View(await list.ToListAsync());
            }

            var studentRecordContext = _context.AcademicRecord.Include(a => a.CourseCodeNavigation).Include(a => a.Student);
            return View(await studentRecordContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(AcademicRecord[] academicRecords)
        {
            if (ModelState.IsValid)
            {
                foreach (AcademicRecord academicRecord in academicRecords)
                {
                    _context.Update(academicRecord);
                    await _context.SaveChangesAsync();
                }
            }

            return Redirect("/AcademicRecords");
        }
    }
}
