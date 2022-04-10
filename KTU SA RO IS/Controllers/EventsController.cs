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

namespace KTU_SA_RO.Controllers
{
    [Authorize(Roles = "admin,eventCoord")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
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
                    positions.Add(setUserPosition(role.ToString()));
            }
            ViewData["positions"] = positions;

            var @event = await _context.Events
                .Include(r => r.Requirements.Where(e => e.Event.Id == id))
                    .ThenInclude(u => u.User)
                .Include(et => et.EventTeamMembers)
                .FirstOrDefaultAsync(m => m.Id == id);

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
                        membersRole.Add(i,setUserPosition(eventTeamMember.RoleName));
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

            return View(@event);
        }

        public string setUserPosition(string roleName)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartDate,EndDate,Location,Description,Has_coordinator,CoordinatorName,CoordinatorSurname,Is_canceled,Is_public,Is_live,PlannedPeopleCount,PeopleCount")]
            Event @event, EventType eventType)
        {

            if (ModelState.IsValid)
            {
                @event.EventType = await _context.EventTypes.FirstOrDefaultAsync(et => et.Name.Equals(eventType.Name));

                ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.Name.Equals(@event.CoordinatorName) && u.Surname.Equals(@event.CoordinatorSurname));

                if (user == null)
                {
                    TempData["danger"] = "Naudotojas su tokiu vardu ir pavarde nebuvo rastas sistemoje";
                    return RedirectToAction(nameof(Create));
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();

                EventTeamMember team = new()
                {
                    EventId = @event.Id,
                    UserId = user.Id,
                    Is_event_coord = true
                };

                _context.Add(team);
                await _context.SaveChangesAsync();

                TempData["success"] = "Renginys <b> " + @event.Title + " </b> sėkmingai sukurtas!";

                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

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

                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    EventTeamMember team = await _context.EventTeamMembers.Where(t => t.EventId == @event.Id).FirstOrDefaultAsync();
                    team.UserId = user.Id;

                    _context.Update(team);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Renginys <b> " + @event.Title + " </b> sėkmingai atnaujintas!";
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
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var @event = await _context.Events
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (@event == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(@event);
        //}

        // POST: Events/Delete/5
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

            await _context.SaveChangesAsync();

            var successMsg = "Renginys:" + @event.Title;
            if (@event.EventTeamMembers != null && @event.Requirements != null)
            {
                TempData["success"] = "<b>" + successMsg + "<b/> su <u>renginio komandos nariais</u> ir <u>specifiniais reikalavimais</u> sėkmingai pašalintas";
            }
            else if (@event.EventTeamMembers != null)
            {
                TempData["success"] = "<b>" + successMsg + "<b/> su <u>renginio komandos nariais</u> sėkmingai pašalintas";
            }
            else if (@event.Requirements != null)
            {
                TempData["success"] = "<b>" + successMsg + "<b/> su <u>specifiniais reikalavimais</u> sėkmingai pašalintas";
            }
            else
            {
                TempData["success"] = "<b>" + successMsg + "<b/> sėkmingai pašalintas";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
