using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Hubs
{
    public class ChatHub : Hub
    {
        private readonly StudentManagementContext _context;
        public ChatHub(StudentManagementContext context)
        {
            _context = context;
        }
        public async Task SendMessage(Message message) => await Clients.All.SendAsync("SendMessages", message);

        public List<GroupInfo> lstUser = new List<GroupInfo>();
        public async Task ShowNotify(string content)
        {
            await Clients.Group("Hello").SendAsync("ShowNotify", content);

        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var groupInfos = _context.GroupInfos.Where(x => x.ConnectionId == Context.ConnectionId).ToList();
            _context.RemoveRange(groupInfos);
            await _context.SaveChangesAsync();

        }
        public override async Task OnConnectedAsync()
        {

            var userId = Context.User.Claims.Select(x => x.Value).ToList()[0];
            var userName = Context.User.Claims.Select(x => x.Value).ToList()[1];
            var role = Context.User.Claims.Select(x => x.Value).ToList()[4];
            var lstEvent = await _context.Events.ToListAsync();

            foreach (var item in lstEvent)
            {
                if (role != "Student")
                {
                    GroupInfo groupInfo = new GroupInfo()
                    {

                        ConnectionId = Context.ConnectionId,
                        Name = userName,
                        Role = role,
                        UserId = userId,
                        EventId = item.EventId,

                    };
                    var groupInfoExist = _context.Set<GroupInfo>().FirstOrDefault(x => x.EventId == item.EventId && x.UserId == userId);
                    if (groupInfoExist == null)
                    {
                        _context.Add(groupInfo);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        groupInfoExist.ConnectionId = groupInfo.ConnectionId;
                        _context.Update(groupInfoExist);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (item.UserId == userId)
                    {

                        GroupInfo groupInfo = new GroupInfo()
                        {

                            ConnectionId = Context.ConnectionId,
                            Name = userName,
                            Role = role,
                            UserId = userId,
                            EventId = item.EventId,

                        };
                        var groupInfoExist = _context.Set<GroupInfo>().FirstOrDefault(x => x.EventId == item.EventId && x.UserId == userId);
                        if (groupInfoExist == null)
                        {
                            _context.Add(groupInfo);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            groupInfoExist.ConnectionId = groupInfo.ConnectionId;
                            _context.Update(groupInfoExist);
                            await _context.SaveChangesAsync();
                        }

                    }
                }

            }
        }
    }
}
