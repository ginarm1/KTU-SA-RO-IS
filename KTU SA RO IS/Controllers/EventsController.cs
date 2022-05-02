using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace KTU_SA_RO.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            /*Pagination*/
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling(_context.Events
                .Count() / (double)pageSize);

            ViewData["totalPages"] = totalPages;
            ViewData["pageIndex"] = pageIndex;

            return View(await _context.Events
                .OrderByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync());
        }
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
        // GET: Events/UserEvents/{userId}
        [Route("Events/UserEvents/{userId}")]
        public async Task<IActionResult> UserEvents(string userId, int pageIndex = 1)
        {
            var userTeams = await _context.EventTeamMembers.Where(u => u.UserId.Equals(userId)).ToListAsync();
            var events = await _context.Events.ToListAsync();

            var userEvents = new List<Event>();

            foreach (var @event in events)
            {
                foreach (var userTeam in userTeams)
                {
                    if (@event.Id.Equals(userTeam.EventId))
                    {
                        userEvents.Add(@event);
                    }
                }

            }

            ViewData["userEvents"] = userEvents;

            return View(nameof(Index));
        }
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positions = new List<string>();

            // convert user roles to positions
            foreach (var role in Enum.GetValues(typeof(Role)))
            {
                if(!role.Equals("admin"))
                    positions.Add(SetUserPosition(role.ToString()));
            }
            ViewData["positions"] = positions;

            var @event = await _context.Events
                .Include(r => r.Requirements.Where(e => e.Event.Id == id))
                    .ThenInclude(u => u.User)
                .Include(et => et.EventTeamMembers)
                .Include(rv => rv.Revenues)
                .Include(c => c.Costs)
                .Include(t => t.Ticketings)
                .Include(s => s.Sponsorships)
                    .ThenInclude(a => a.Sponsor)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewData["lastEvents"] = LastEvents(@event, 3, 4);
            ViewData["sponsors"] = await _context.Sponsors.ToListAsync();
            ViewData["eventSponsors"] = @event.Sponsorships.Select(s => s.Sponsor).Distinct().ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventTeam = new Dictionary<int,ApplicationUser>();
            var membersRole = new Dictionary<int,string>();
            int i = 0;

            foreach (var eventTeamMember in @event.EventTeamMembers)
            {
                i = eventTeamMember.Id;
                if (eventTeamMember.EventId == id)
                {
                    var user = _context.Users.FirstOrDefault( u => u.Id.Equals(eventTeamMember.UserId));
                    if (user != null)
                    {
                        eventTeam.Add(i,user);
                        membersRole.Add(i,SetUserPosition(eventTeamMember.RoleName));
                    }
                    else
                    {
                        TempData["danger"] = "Naudotojas nerastas";
                        return View();
                    }
                }
            }

            if (@event == null)
            {
                return NotFound();
            }

            ViewData["eventTeam"] = eventTeam;
            ViewData["membersRole"] = membersRole;
            ViewData["revenues"] = @event.Revenues.Where(r => r.Event == @event).ToList();
            ViewData["costs"] = @event.Costs.Where(r => r.Event == @event).ToList();
            ViewData["ticketings"] = @event.Ticketings.Where(r => r.Event == @event).ToList();
            ViewData["userBelongsToEvent"] = await _context.EventTeamMembers.FirstOrDefaultAsync(e => e.UserId.Equals(userId) && e.EventId == @event.Id);

            return View(@event);
        }

        public List<Event> LastEvents(Event chosenEvent,int chosenEventsCount , int removeLettersCount)
        {
            if (chosenEvent.Title.Length > 3)
            {
                string titleTrimed = chosenEvent.Title.Remove(chosenEvent.Title.Length - removeLettersCount);
                var lastEvents = new List<Event>();

                var les = _context.Events.Where(e => e.Title.Contains(titleTrimed) && !e.StartDate.Equals(chosenEvent.StartDate) && !e.EndDate.Equals(chosenEvent.EndDate)).OrderByDescending(e => e.Id).Select(e => e.Id).AsEnumerable();
                if (chosenEventsCount > les.Count())
                    chosenEventsCount = les.Count();

                for (int i = 0; i < chosenEventsCount; i++)
                {
                    var lastEvent = _context.Events
                        .Where(e => e.Title.Contains(titleTrimed) && !e.StartDate.Equals(chosenEvent.StartDate) && !e.EndDate.Equals(chosenEvent.EndDate) && les.ElementAtOrDefault(i) == e.Id)
                        .Include(et => et.EventTeamMembers)
                        .Include(rv => rv.Revenues)
                        .Include(c => c.Costs)
                        .Include(t => t.Ticketings)
                        .Include(s => s.Sponsorships)
                        .FirstOrDefault();
                    lastEvents.Add(lastEvent);
                }
                return lastEvents;
            }

            return null;
        }

        public string SetUserPosition(string roleName)
        {
            if (roleName.Equals("registered"))
                return "Registruotas naudotojas";
            if (roleName.Equals("eventCoord"))
                return "Renginio koordinatorius";
            if (roleName.Equals("fsaOrgCoord"))
                return "ORK koordinatorius";
            if (roleName.Equals("fsaBussinesCoord"))
                return "VIP koordinatorius";
            if (roleName.Equals("fsaPrCoord"))
                return "RSV koordinatorius"; 
            if (roleName.Equals("orgCoord"))
                return "CSA ORK koordinatorius";
            if (roleName.Equals("admin"))
                return "Administratorius";
            else
                return null;
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["eventTypes"] = _context.EventTypes.ToList();
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartDate,EndDate,Location,Description,Has_coordinator,CoordinatorName,CoordinatorSurname,Is_canceled,Is_public,Is_live,PlannedPeopleCount,PeopleCount")]
            Event @event, string eventTypeName)
        {

            if (ModelState.IsValid)
            {
                @event.EventType = await _context.EventTypes.FirstOrDefaultAsync(et => et.Name.Equals(eventTypeName));

                ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.Name.Equals(@event.CoordinatorName) && u.Surname.Equals(@event.CoordinatorSurname));

                if (user == null)
                {
                    TempData["danger"] = "Naudotojas su tokiu vardu ir pavarde nebuvo rastas sistemoje";
                    return RedirectToAction(nameof(Create));
                }

                var role = await _userManager.GetRolesAsync(user);
                if (role == null)
                {
                    TempData["danger"] = "Renginio koordinatoriaus rolė nebuvo rasta";
                    return RedirectToAction(nameof(Create));
                }
                _context.Add(@event);
                await _context.SaveChangesAsync();

                var is_coord = true;
                if (role.Equals("orgCoord"))
                {
                    is_coord = false;
                }
                
                EventTeamMember team = new()
                {
                    EventId = @event.Id,
                    UserId = user.Id,
                    RoleName = role.FirstOrDefault(),
                    Is_event_coord = is_coord
                };

                _context.Add(team);
                await _context.SaveChangesAsync();

                TempData["success"] = "Renginys <b> " + @event.Title + " </b> sėkmingai sukurtas!";

                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord")]
        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            ViewData["eventTypes"] = _context.EventTypes.ToList();

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,StartDate,EndDate,Location,Description,Has_coordinator,CoordinatorName,CoordinatorSurname,Is_canceled,Is_public,Is_live,PlannedPeopleCount,PeopleCount")]
            Event @event, EventType eventType)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @event.EventType = await _context.EventTypes.FirstOrDefaultAsync(et => et.Name.Equals(eventType.Name));

                    ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.Name.Equals(@event.CoordinatorName) && u.Surname.Equals(@event.CoordinatorSurname));

                    if (user == null)
                    {
                        TempData["danger"] = "Naudotojas su tokiu vardu ir pavarde nebuvo rastas";
                        return RedirectToAction(nameof(EventsController.Edit), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
                    }

                    EventTeamMember teamMember = await _context.EventTeamMembers.Where(t => t.EventId == @event.Id && t.UserId == user.Id).FirstOrDefaultAsync();
                    if (teamMember == null)
                    {
                        EventTeamMember teamEventCoord = await _context.EventTeamMembers.Where(t => t.EventId == @event.Id && t.RoleName.Equals("eventCoord")).FirstOrDefaultAsync();
                        var oldEventCoord = new ApplicationUser();
                        if (teamEventCoord == null)
                        {
                            EventTeamMember teamFsaOrgCoord = await _context.EventTeamMembers.Where(t => t.EventId == @event.Id && t.RoleName.Equals("fsaOrgCoord")).FirstOrDefaultAsync();
                            oldEventCoord = await _context.Users.FirstOrDefaultAsync(u => u.Id == teamFsaOrgCoord.UserId);
                            EventTeamMember teamFsaOrgCoord2 = new()
                            {
                                EventId = @event.Id,
                                UserId = oldEventCoord.Id,
                                RoleName = teamFsaOrgCoord.RoleName,
                                Is_event_coord = false
                            };
                            
                            teamFsaOrgCoord.UserId = user.Id;
                            teamFsaOrgCoord.RoleName = "eventCoord";
                            _context.Update(teamFsaOrgCoord);
                            _context.Add(teamFsaOrgCoord2);
                            TempData["success"] = "FSA ORK koordinatorius <b>" + oldEventCoord.Name + " " + oldEventCoord.Surname +
                                                    "</b> sėkmingai paskyrė išrinktą renginio koordinatorių <b>" + user.Name + " " + user.Surname + "</b> . ";
                        }
                        else
                        {
                            oldEventCoord = await _context.Users.FirstOrDefaultAsync(u => u.Id == teamEventCoord.UserId);
                            teamEventCoord.UserId = user.Id;                           
                            _context.Update(teamEventCoord);
                            TempData["success"] = "Senas renginio koordinatorius <b>" + oldEventCoord.Name + " " + oldEventCoord.Surname + 
                                                    "</b> sėkmingai pakeistas į <b>" + user.Name + " " + user.Surname + "</b> . ";
                        }                        

                        @event.CoordinatorName = user.Name;
                        @event.CoordinatorSurname = user.Surname;
                    }


                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    TempData["success"] += "Renginys <b> " + @event.Title + " </b> sėkmingai atnaujintas!";
                    return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(et => et.EventTeamMembers)
                .Include(r => r.Requirements)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event != null)
                _context.Events.Remove(@event);
            if(@event.EventTeamMembers != null)
                _context.EventTeamMembers.RemoveRange(@event.EventTeamMembers);
            if(@event.Requirements != null)
                _context.Requirements.RemoveRange(@event.Requirements);
            if (@event.Sponsorships != null)
                _context.Sponsorships.RemoveRange(@event.Sponsorships);
            if (@event.Revenues != null)
                _context.Revenues.RemoveRange(@event.Revenues);
            if (@event.Ticketings != null)
                _context.Ticketings.RemoveRange(@event.Ticketings);

            await _context.SaveChangesAsync();

            var successMsg = "Renginys:" + @event.Title;
            if (@event.EventTeamMembers != null && @event.Requirements != null)
            {
                TempData["success"] = "<b>" + @event.Title + "</b> su <u>renginio komandos nariais</u> ir <u>specifiniais reikalavimais</u> sėkmingai pašalintas!";
            }
            else if (@event.EventTeamMembers != null)
            {
                TempData["success"] = "<b>" + @event.Title + "</b> su <u>renginio komandos nariais</u> sėkmingai pašalintas!";
            }
            else if (@event.Requirements != null)
            {
                TempData["success"] = "<b>" + @event.Title + "</b> su <u>specifiniais reikalavimais</u> sėkmingai pašalintas!";
            }
            else
            {
                TempData["success"] = "<b>" + @event.Title + "</b> sėkmingai pašalintas!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
