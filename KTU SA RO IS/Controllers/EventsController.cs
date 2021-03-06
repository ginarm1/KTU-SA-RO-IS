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
using KTU_SA_RO.Services;

namespace KTU_SA_RO.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
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
            ViewData["users"] = await _context.Users.ToListAsync();

            return View(await _context.Events
                .Include(tm => tm.EventTeamMembers)
                .OrderByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync());
        }

        // GET: Filtered events
        [HttpGet]
        public async Task<IActionResult> Index(string title,int pageIndex = 1)
        {
            /*Filtering*/
            /*Filter by event title*/
            var context_events = _context.Events.AsQueryable();
            if (title != null)
            {
                context_events = context_events.Where(e => e.Title.Equals(title) || e.Title.Contains(title));
                ViewData["pickedEventTitle"] = title;
            }

            /*Pagination*/
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling(context_events
                .Count() / (double)pageSize);
            ViewData["totalPages"] = totalPages;
            ViewData["pageIndex"] = pageIndex;
            ViewData["users"] = await _context.Users.ToListAsync();

            return View(await context_events
                .Include(tm => tm.EventTeamMembers)
                .OrderByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync());
        }

        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
        // GET: Events/UserEvents/{userId}
        [Route("Events/UserEvents/{userId}")]
        public async Task<IActionResult> UserEvents(string userId, string title, int pageIndex = 1)
        {
            var userTeams = await _context.EventTeamMembers.Where(u => u.UserId.Equals(userId)).ToListAsync();

            var events = new List<Event>();
            if (title == null)
                events = await _context.Events.ToListAsync();
            else
                events = await _context.Events.Where(e => e.Title.Equals(title) || e.Title.Contains(title)).ToListAsync();
            var userEvents = new List<Event>();
            var users = await _context.Users.ToListAsync();

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

            ViewData["userEvents"] = userEvents.OrderByDescending(e => e.StartDate).ToList();
            ViewData["users"] = users;
            ViewData["userId"] = userId;

            if (userEvents.Count == 0)
                TempData["danger"] = "Jūsų renginių nebuvo rasta";

            return View(nameof(Index));
        }

        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id, int? lastEventsCount)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positions = new List<string>();
            EventService eventService = new EventService(_context);

            // convert user roles to positions
            foreach (var role in Enum.GetValues(typeof(Role)))
            {
                if(!role.Equals("admin"))
                    positions.Add(eventService.SetUserPosition(role.ToString()));
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

            if (lastEventsCount != null)
                ViewData["lastEvents"] = eventService.LastEventsStats(@event, lastEventsCount, 4);
            else
                ViewData["lastEvents"] = eventService.LastEventsStats(@event, 3, 4);
            
            ViewData["sponsors"] = await _context.Sponsors.ToListAsync();
            ViewData["eventSponsors"] = @event.Sponsorships.Select(s => s.Sponsor).Distinct().ToList();

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
                        membersRole.Add(i, eventService.SetUserPosition(eventTeamMember.RoleName));
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

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(currentUserId));
            var currentRole = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault();

            if (currentRole.Equals("admin") || currentRole.Equals("eventCoord") || currentRole.Equals("fsaOrgCoord"))
            {
                var users = await _context.Users.OrderBy(u => u.Name).ToListAsync();
                var usersNameSurname = new List<string>();

                foreach (var user in users)
                {
                    var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    var role = await _roleManager.Roles.FirstOrDefaultAsync(a => a.Name.Equals(userRole));
                    usersNameSurname.Add(user.Name + " " + user.Surname + " || " + eventService.SetUserPosition(role.Name)); 
                    ViewData["usersNameSurname"] = usersNameSurname;
                }
            }


            ViewData["eventTeam"] = eventTeam;
            ViewData["membersRole"] = membersRole;
            ViewData["revenues"] = @event.Revenues.Where(r => r.Event == @event).ToList();
            ViewData["costs"] = @event.Costs.Where(r => r.Event == @event).ToList();
            ViewData["ticketings"] = @event.Ticketings.Where(r => r.Event == @event).ToList();
            ViewData["userBelongsToEvent"] = await _context.EventTeamMembers.FirstOrDefaultAsync(e => e.UserId.Equals(currentUserId) && e.EventId == @event.Id);
            

            return View(@event);
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
                        EventTeamMember newEventCoord = await _context.EventTeamMembers.Where(t => 
                               (t.UserId == user.Id && t.RoleName.Equals("admin") )
                            || (t.UserId == user.Id && t.RoleName.Equals("fsaBussinesCoord") ) 
                            || (t.UserId == user.Id && t.RoleName.Equals("fsaPrCoord")) 
                            || (t.UserId == user.Id && t.RoleName.Equals("orgCoord"))).FirstOrDefaultAsync();
                        if (newEventCoord != null)
                        {
                            TempData["danger"] = "Netinkama norimo naujo koordinatoriaus rolė. Privalo būti: Renginio koordinatorius arba FSA ORK koordinatorius";
                            return RedirectToAction(nameof(EventsController.Edit), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
                        }

                        var oldEventCoord = new ApplicationUser();
                        if (teamEventCoord == null)
                        {
                            EventTeamMember teamFsaOrgCoord = await _context.EventTeamMembers.Where(t => t.EventId == @event.Id && t.RoleName.Equals("fsaOrgCoord")).FirstOrDefaultAsync();
                            if (teamFsaOrgCoord == null)
                                teamFsaOrgCoord = await _context.EventTeamMembers.Where(t => t.EventId == @event.Id && t.RoleName.Equals("admin")).FirstOrDefaultAsync();
                            else
                            {
                                EventTeamMember teamFsaOrgCoord2 = new()
                                {
                                    EventId = @event.Id,
                                    UserId = oldEventCoord.Id,
                                    RoleName = teamFsaOrgCoord.RoleName,
                                    Is_event_coord = false
                                };
                                _context.Add(teamFsaOrgCoord2);
                            }
                            oldEventCoord = await _context.Users.FirstOrDefaultAsync(u => u.Id == teamFsaOrgCoord.UserId);
                            
                            teamFsaOrgCoord.UserId = user.Id;
                            teamFsaOrgCoord.RoleName = "eventCoord";
                            _context.Update(teamFsaOrgCoord);

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
