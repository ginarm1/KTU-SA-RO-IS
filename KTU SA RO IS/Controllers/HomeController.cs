using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var a = await _context.Events
                .Include(et => et.EventType)
                .Include(et => et.EventTeam)
                    .ThenInclude(u => u.Users)
                .ToListAsync();

            var userRepresent = await _context.Users.ToListAsync();
            var repList = new Dictionary<int,string>();
            var eventIds = new List<int>();

            foreach (var Event in a)
            {
                foreach (var ur in userRepresent)
                {
                    if (Event.EventTeam.UserId.Equals(ur.Id) && Event.EventTeam.Is_event_coord)
                    {
                        eventIds.Add(Event.EventTeam.EventId);

                        switch (ur.Representative.ToString())
                        {
                            case "infosa":
                                repList.Add(Event.EventTeam.EventId, "#03afd7");
                                break;
                            case "csa":
                                repList.Add(Event.EventTeam.EventId, "#2B2B2B");
                                break;
                            case "vivat":
                                repList.Add(Event.EventTeam.EventId, "#ea6c32");
                                break;
                            case "indi":
                                repList.Add(Event.EventTeam.EventId, "#332c75");
                                break;
                            case "vfsa":
                                repList.Add(Event.EventTeam.EventId, "#3b3c5a");
                                break;
                            case "esa":
                                repList.Add(Event.EventTeam.EventId, "#27395b");
                                break;
                            case "shm":
                                repList.Add(Event.EventTeam.EventId, "#78274b");
                                break;
                            case "statius":
                                repList.Add(Event.EventTeam.EventId, "#1a5d33");
                                break;
                            case "fumsa":
                                repList.Add(Event.EventTeam.EventId, "#ea3c3b");
                                break;

                            default:
                                break;
                        }
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

        //[Route("getCalendarEvents")]
        //public IActionResult Calendar()
        //{
        //    var events = _context.Events.Select( e => new
        //    {
        //        id = e.Id,
        //        title = e.Title,
        //        description = e.Description,
        //        start = e.StartDate.ToString("yyyy-MM-dd"),
        //        end = e.EndDate.ToString("yyyy-MM-dd"),
        //    }).ToList();
        //    return new JsonResult(events);

        //}
    }
}
