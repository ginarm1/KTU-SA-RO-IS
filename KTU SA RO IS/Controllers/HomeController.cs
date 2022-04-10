using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["userId"] = userId;

            var events = await _context.Events
                .Include(et => et.EventType)
                .Include(et => et.EventTeamMembers)
                .ToListAsync();

            var eventTeamMembers = await _context.EventTeamMembers
                .ToListAsync();
            var users = await _context.Users.ToListAsync();
            var repList = new Dictionary<int, string>();
            var eventIds = new List<int>();

            foreach (var eventTeamMember in eventTeamMembers)
            {
                if (eventTeamMember.UserId != null && eventTeamMember.Is_event_coord)
                {
                    eventIds.Add(eventTeamMember.EventId);
                    var user = users.FirstOrDefault(u => u.Id.Equals(eventTeamMember.UserId));

                    switch (user.Representative.ToString())
                    {
                        case "infosa":
                            repList.Add(eventTeamMember.EventId, "#03afd7");
                            break;
                        case "csa":
                            repList.Add(eventTeamMember.EventId, "#2B2B2B");
                            break;
                        case "vivat":
                            repList.Add(eventTeamMember.EventId, "#ea6c32");
                            break;
                        case "indi":
                            repList.Add(eventTeamMember.EventId, "#332c75");
                            break;
                        case "vfsa":
                            repList.Add(eventTeamMember.EventId, "#3b3c5a");
                            break;
                        case "esa":
                            repList.Add(eventTeamMember.EventId, "#27395b");
                            break;
                        case "shm":
                            repList.Add(eventTeamMember.EventId, "#78274b");
                            break;
                        case "statius":
                            repList.Add(eventTeamMember.EventId, "#1a5d33");
                            break;
                        case "fumsa":
                            repList.Add(eventTeamMember.EventId, "#ea3c3b");
                            break;

                        default:
                            break;
                    }
                }
            }
            ViewData["represantatives"] = repList;
            ViewData["eventIds"] = eventIds;

            return View(await _context.Events
                .Include(et => et.EventType)
                .ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
