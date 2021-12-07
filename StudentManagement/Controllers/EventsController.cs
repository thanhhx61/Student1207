using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Models.Events;
using StudentManagement.Models.Students;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class EventsController : Controller
    {
        private static User user = new User();
        public static int? evtId;
        private readonly IUserService userService;
        private readonly IEventService eventService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly StudentManagementContext context;

        public EventsController(IUserService userService,
                                IEventService eventService,
                                IWebHostEnvironment webHostEnvironment,
                                StudentManagementContext context)
        {
            this.userService = userService;
            this.eventService = eventService;
            this.webHostEnvironment = webHostEnvironment;
            this.context = context;
        }
        [Route("/Events/Index/{userId}")]
        public async Task<IActionResult> Index(string userId, int? SchoolYearId, int? ListEvent, bool checkbox1, bool checkbox2, bool checkbox3, int status1 = 1, int status2 = 2, int status3 = 3)
        {
            var events = from m in context.Events select m;

            if (SchoolYearId != null)
            {
                events = events.Where(s => s.SchoolYearId == SchoolYearId);
            }
            if (ListEvent != null)
            {
                events = events.Where(s => s.ListEvent.ListEventId == ListEvent);
            }
            if(checkbox1 && checkbox2 & checkbox3)
            {
               events = events.Where(s => s.Status == (Enums.EventStatus?)status1 || s.Status == (Enums.EventStatus?)status2 || s.Status == (Enums.EventStatus?)status3);
            }else
            {
                if(checkbox1 && checkbox2 && !checkbox3)
                {
                    events = events.Where(s => s.Status == (Enums.EventStatus?)status1 || s.Status == (Enums.EventStatus?)status2);
                }
                if(checkbox1 && !checkbox2 && !checkbox3)
                {
                    events = events.Where(s => s.Status == (Enums.EventStatus?)status1);
                }    
                if (!checkbox1 && checkbox2 && checkbox3)
                {
                    events = events.Where(s => s.Status == (Enums.EventStatus?)status2 || s.Status == (Enums.EventStatus?)status3);
                }
                if (!checkbox1 && checkbox2 && !checkbox3)
                {
                    events = events.Where(s => s.Status == (Enums.EventStatus?)status2);
                }
                if (checkbox1 && !checkbox2 && checkbox3)
                {
                    events = events.Where(s => s.Status == (Enums.EventStatus?)status3 || s.Status == (Enums.EventStatus?)status1);
                }
                if (!checkbox1 && !checkbox2 && checkbox3)
                {
                    events = events.Where(s => s.Status == (Enums.EventStatus?)status3);
                }
            }    
            user = userService.Get(userId);
            var currentUserId = User.Claims.Select(x => x.Value).ToList()[0];
            var eventSearch = new SearchEvent
            {
                events = await events.Include(p => p.Messages).Where(p => p.UserId == userId).OrderByDescending(c => c.SchoolYearId).OrderByDescending(c => c.EventId).ToListAsync()
            };
            var readList = await context.BellNotifications.ToListAsync();
            for(var i = 0; i < eventSearch.events.Count;i++)
            {
                //kiểm tra có tin nhắn của mình không
               var messageContains = eventSearch.events[i].Messages.Where(x => x.UserId == currentUserId).ToList();              
                var remains = eventSearch.events[i].Messages.Where(m => !messageContains.Contains(m)).ToList();
                eventSearch.events[i].Messages = remains;

                //kiểm tra có tin nhắn đã đọc hay không
                var realReadList = new List<Message>();
                for(int j = 0; j < eventSearch.events[i].Messages.Count; j++)
                {
                    var message = eventSearch.events[i].Messages.ToList();

                    if (readList.Any(x => x.NotifiId == message[j].MessagesId && x.UserId == currentUserId))
                    {
                        realReadList.Add(message[j]);
                    }
                }
                eventSearch.events[i].Messages = eventSearch.events[i].Messages.Where(m => !realReadList.Contains(m)).ToList();
            }
            var schoolYear = eventService.GetSchoolYear(userId).SchoolYear.SchoolYearName;
            ViewBag.ListSchoolYearId = await context.SchoolYears.ToListAsync();
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            ViewData["SchoolYearId"] = new SelectList(context.SchoolYears, "SchoolYearId", "SchoolYearName");
            ViewBag.User = user;
            ViewBag.SchoolYear = schoolYear;
            return View(eventSearch);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateEvent create)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    create.UserId = user.Id;
                    var schoolYear = context.UserSchoolYears.Include(u => u.SchoolYear).Include(u => u.User).OrderByDescending(u => u.SchoolYearId).FirstOrDefault(m => m.UserId == user.Id).SchoolYear.SchoolYearId;
                    var events = new Event()
                    {
                        UserId = create.UserId,
                        Status = (Enums.EventStatus?)1,
                        Act = create.Act,
                        Activities = create.Activities,
                        PowerDev = create.PowerDev,
                        PowerExerted = create.PowerExerted,
                        SchoolYearId = schoolYear,
                        Think = create.Think,
                        ListEventId = create.ListEventId
                    };
                    if (eventService.Create(events))
                    {
                        return RedirectToAction("Index", "Events", new { userId = user.Id });
                    }
                }
                ViewBag.User = user;
                ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
                return View(create);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet]
        [Route("/Events/Edit/{eventId}")]
        public async Task<IActionResult> Edit(int eventId)
        {
            var currentUserId = User.Claims.Select(x => x.Value).ToList()[0];
            var events = eventService.Get(eventId);
            var edituser = new EditEvents()
            {
                EventId = events.EventId,
                Act = events.Act,
                Activities = events.Activities,
                PowerDev = events.PowerDev,
                PowerExerted = events.PowerExerted,
                Think = events.Think,
                UserId = events.UserId,
                ListEventId = events.ListEventId
            };
            var lstMessages = events.Messages.Where(x => x.UserId != currentUserId).ToList();
            foreach (var item in lstMessages)
            {
                var bellNoti = new BellNotification()
                {
                    DateCreated = DateTime.Now,
                    IsRead = true,
                    NotifiId = item.MessagesId,
                    UserId = currentUserId
                };
                context.Add(bellNoti);
                await context.SaveChangesAsync();
            }

            ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
            ViewBag.User = user;
            ViewBag.EvtId = eventId;
            return View(edituser);
        }
        [HttpPost]
        public IActionResult Edit(EditEvents model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var events = eventService.Get(model.EventId);
                    events.UserId = model.UserId;
                    events.PowerDev = model.PowerDev;
                    events.PowerExerted = model.PowerExerted;
                    events.Think = model.Think;
                    events.Act = model.Act;
                    events.ListEventId = model.ListEventId;
                    events.Activities = model.Activities;
                    if (eventService.Edit(events))
                    {
                        return RedirectToAction("Index", "Events", new { userId = model.UserId });
                    }
                }
                ViewBag.User = user;
                ViewData["ListEventId"] = new SelectList(context.ListEvents, "ListEventId", "ListEventName");
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Events/Delete/{eventId}")]
        public IActionResult Remove(int eventId)
        {
            if (eventService.Remove(eventId))
            {
                return RedirectToAction("Index", "Events", new { userId = user.Id });
            }
            return RedirectToAction("Index", "Detail", new { userId = user.Id });
        }
        [HttpGet]
        [Route("/Events/ChangeStatus23/{eventId}")]
        public IActionResult ChangeStatus23(int eventId)
        {
            ViewBag.EmployeeId = eventId;
            if (eventService.ChangeStatus23(eventId))
            {
                return RedirectToAction("Index", "Events", new { userId = user.Id });
            }
            return RedirectToAction("Index", "Events", new { userId = user.Id });
        }
        [HttpGet]
        [Route("/Events/ChangeStatus12/{eventId}")]
        public IActionResult ChangeStatus12(int eventId)
        {
            ViewBag.EmployeeId = eventId;
            if (eventService.ChangeStatus12(eventId))
            {
                return RedirectToAction("Index", "Events", new { userId = user.Id });
            }
            return RedirectToAction("Index", "Events", new { userId = user.Id });
        }
    }
}
