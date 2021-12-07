using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Models;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly StudentManagementContext context;
        private readonly SignInManager<User> _signInManager;
        private static User user = new User();

        public StudentController(IUserService userService,
                                 UserManager<User> userManager,
                                 StudentManagementContext context,
                                 SignInManager<User> signInManager)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.context = context;
            _signInManager = signInManager;
        }
        public IActionResult Index(string StudentName, int? SchoolYearId, int? EventId, string SkillName)
        {
            var stu = from m in context.Users select m;
            if (!string.IsNullOrEmpty(StudentName))
            {
                stu = stu.Where(s => s.UserName!.Contains(StudentName));
            }
             if (SchoolYearId != null)
            {
                stu = stu.Where(a => a.UserSchoolYears.OrderByDescending(c => c.SchoolYearId).First().SchoolYear.SchoolYearId == SchoolYearId);
            }

            if (EventId != null)
            {
                stu = stu.Where(a => a.Events.OrderByDescending(c => c.EventId).First().ListEvent.ListEventId == EventId);
            }
            var users = new Student();
            if (!User.IsInRole("Family"))
            {
                users.Users = stu.Where(s => s.StudentCode != null).OrderBy(s => s.StudentCode).Include(p => p.Events).Include(p => p.Messages).Include(p => p.UserSchoolYears).ToList();
            }
            else
            {
                users.Users = stu.Where(s => s.StudentCode != null).Where(s => s.ParentId == "003").OrderBy(s => s.StudentCode).Include(p => p.Events).Include(p => p.Messages).Include(p => p.UserSchoolYears).ToList();
            };
            ViewBag.listSkills = userService.getListSkill();
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            ViewData["SchoolYearId"] = new SelectList(context.SchoolYears, "SchoolYearId", "SchoolYearName");
            
            return View(users);
        }
    }
}
