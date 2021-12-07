using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Models.Accounts;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly StudentManagementContext _context;
        private readonly SignInManager<User> signInManager;
        private static User user = new User();

        public AccountController(IUserService userService, UserManager<User> userManager, StudentManagementContext context, SignInManager<User> signInManager)
        {
            this.userService = userService;
            this.userManager = userManager;
            _context = context;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var result = await userService.Login(login);
                if (result.Success && result.Roles.Length > 0)
                {
                    var user = _context.Users.Where(u => u.UserName.Equals(login.Username)).First();
                    if (result.Roles.Contains("Student"))
                    {
                        return RedirectToAction("Index", "Events", new { userId = user.Id });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Student");
                    }
                }
                ViewBag.Error = result.Message;
                return View();
            }
            return View();
        }
        public IActionResult Signout()
        {
            userService.Sighout();
            return RedirectToAction("Login", "Account");
        }
    }

}
