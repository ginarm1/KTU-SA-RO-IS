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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.OrderByDescending(e => e.Id).ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(r => r.Requirements.Where(e => e.Event.Id == id))
                    .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

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

                EventTeam team = new()
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

                    EventTeam team = await _context.EventTeams.Where(t => t.EventId == @event.Id).FirstOrDefaultAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
