using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Hubs;
using StudentManagement.Models.Comments;

namespace StudentManagement.Controllers
{
    public class MessagesController : Controller
    {
        private readonly StudentManagementContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _signalrHub;
        private readonly SignInManager<User> _signInManager;
        private readonly IToastNotification _toastNotification;
        public static int EvtId;

        public MessagesController(StudentManagementContext context,
                                  UserManager<User> userManager,
                                  IHubContext<ChatHub> signalrHub,
                                  SignInManager<User> signInManager,
                                  IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManager;
            _signalrHub = signalrHub;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
        }
        [HttpGet("/Messages/CallHashTag/")]
        public async Task<IActionResult> CallHashTag()
        {
            var hasttag = await _context.Skills.Select(m => m.SkillName).ToListAsync();
            return Ok(hasttag);
        }
        // GET: Messages
        [Route("/Messages/Index/{evtId}")]
        public async Task<IActionResult> Index(int evtId)
        {

            var studentManagementContext = _context.Messages.Include(m => m.Event).Include(m => m.MessagesSkills).Include(m => m.User).Where(p => p.EventId == evtId).ToListAsync();
            var listMessages = new ViewMessages()
            {
                Messages = await studentManagementContext
            };
            return View(listMessages);
        }
        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Event)
                .Include(m => m.MessagesSkills)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MessagesId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }
        // POST: Messages/Create
        [HttpPost]
        public async Task<IActionResult> Create(int EventId, string content, List<int> skillId)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(u => u.UserName.Equals(User.Identity.Name)).First();
                //Chi role doctor cmt # đc
                if (User.IsInRole("Doctor"))
                {
                    List<Skill> skills = _context.Skills.ToList();
                    foreach (var s in skills)
                    {
                        if (content.Contains(s.SkillName))
                        {
                            skillId.Add(s.SkillId);
                        }
                    };
                }
                Message message = new Message();
                message.Timestamp = DateTime.Now;
                message.Content = content;
                message.UserId = user.Id;
                message.EventId = EventId;
                _context.Add(message);
                await _context.SaveChangesAsync();
                //Add vào bảng phụ
                if (skillId != null)
                {
                    foreach (var id in skillId)
                    {
                        
                        MessagesSkill messagesSkill = new MessagesSkill();
                        messagesSkill.SkillId = id;
                        messagesSkill.MessagesId = message.MessagesId;
                        _context.Add(messagesSkill);
                        _context.SaveChanges();
                    }
                }
                var groupInfo = await _context.GroupInfos.ToListAsync();
                foreach (var item in groupInfo)
                {
                    if (item.EventId == EventId && item.UserId != user.Id)
                    {
                        await _signalrHub.Clients.Client(item.ConnectionId).SendAsync("ShowNotify", content);

                    }

                }
               await _signalrHub.Clients.All.SendAsync("SendMessages");
                return Ok(1);
            }
            return Ok(0);
        }

        //GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return Ok(message);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string content, List<int> skillId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Chi role doctor cmt # đc
                    if (User.IsInRole("Doctor"))
                    {
                        List<Skill> skills = _context.Skills.ToList();
                        foreach (var s in skills)
                        {
                            if (content.Contains(s.SkillName))
                            {
                                skillId.Add(s.SkillId);
                            }
                        };
                    }
                    Message mess = await _context.Messages.FindAsync(id);     
                    mess.Content = content;
                    _context.Update(mess);
                    await _context.SaveChangesAsync();
                    var messskill = _context.MessagesSkills.Where(s => s.MessagesId == id).ToList();
                    //Xóa dữ liệu bảng MessSkill 
                    if (messskill != null)
                    {
                        foreach (var i in messskill)
                        {
                            _context.MessagesSkills.Remove(i);
                        }
                    }
                    //Add vào lại bảng phụ
                    if (skillId != null)
                    {
                        foreach (var item in skillId)
                        {
                            MessagesSkill messagesSkill = new MessagesSkill();
                            messagesSkill.SkillId = item;
                            messagesSkill.MessagesId = mess.MessagesId;
                            _context.Add(messagesSkill);
                        }
                        _context.SaveChanges();
                    }

                    await _signalrHub.Clients.All.SendAsync("SendMessages");

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;

                }
                return Ok(1);
            }
            return Ok(0);
        }
        // POST: Messages/Delete/5
        [HttpGet]
        [Route("/Messages/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var message = await _context.Messages.FindAsync(id);
            var messskill = _context.MessagesSkills.Where(s => s.MessagesId == id).ToList();
            var bell = _context.BellNotifications.Where(s => s.NotifiId == id).ToList();
            //Xóa dữ liệu bảng MessSkill 
            if (messskill != null)
            {
                foreach (var i in messskill)
                {
                    _context.MessagesSkills.Remove(i);
                    _context.SaveChanges();
                }
            }
            //Xóa dữ liệu Bell
            if (bell != null)
            {
                foreach (var item in bell)
                {
                    _context.BellNotifications.Remove(item);
                    _context.SaveChanges();
                }
            }
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("SendMessages");
            return Ok();
        }
    }
}
