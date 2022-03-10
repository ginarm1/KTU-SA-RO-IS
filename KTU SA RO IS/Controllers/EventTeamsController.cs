using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KTU_SA_RO.Data;
using KTU_SA_RO.Models;

namespace KTU_SA_RO.Controllers
{
    public class EventTeamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventTeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EventTeams
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EventTeams.Include(e => e.Events);
            return View(await applicationDbContext.OrderByDescending(e => e.Events.Id).ToListAsync());
        }

        // GET: EventTeams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTeam = await _context.EventTeams
                .Include(e => e.Events)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventTeam == null)
            {
                return NotFound();
            }

            return View(eventTeam);
        }

        // GET: EventTeams/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "CoordinatorName");
            return View();
        }

        // POST: EventTeams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventId,UserId,Is_event_coord")] EventTeam eventTeam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventTeam);
                await _context.SaveChangesAsync();

                TempData["success"] = "Komanda <b> " + eventTeam.Id + "</b> sėkmingai sukurta!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "CoordinatorName", eventTeam.EventId);
            return View(eventTeam);
        }

        // GET: EventTeams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTeam = await _context.EventTeams.FindAsync(id);
            if (eventTeam == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "CoordinatorName", eventTeam.EventId);
            return View(eventTeam);
        }

        // POST: EventTeams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,UserId,Is_event_coord")] EventTeam eventTeam)
        {
            if (id != eventTeam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventTeam);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Komanda <b> " + eventTeam.Id + "</b> sėkmingai atnaujinta!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventTeamExists(eventTeam.Id))
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
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "CoordinatorName", eventTeam.EventId);
            return View(eventTeam);
        }

        // GET: EventTeams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTeam = await _context.EventTeams
                .Include(e => e.Events)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventTeam == null)
            {
                return NotFound();
            }

            return View(eventTeam);
        }

        // POST: EventTeams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventTeam = await _context.EventTeams.FindAsync(id);
            _context.EventTeams.Remove(eventTeam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventTeamExists(int id)
        {
            return _context.EventTeams.Any(e => e.Id == id);
        }
    }
}
