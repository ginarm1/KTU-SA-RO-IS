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
    public class RequirementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequirementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requirements
        public async Task<IActionResult> Index()
        {
            ViewData["GeneralReq"] = await _context.Requirements.Where(r => r.Is_general).ToListAsync();
            ViewData["AdditionalReq"] = await _context.Requirements.Where(r => !r.Is_general).ToListAsync();
            return View(await _context.Requirements.ToListAsync());
        }

        public async Task<IActionResult> GetAdditionalReq(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.Where(e => e.Id == id).FirstOrDefaultAsync();
            var requirements = await _context.Requirements.Where(r => r.Event == @event).ToListAsync();
            return (IActionResult)requirements;
        }

        // GET: Requirements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requirement = await _context.Requirements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requirement == null)
            {
                return NotFound();
            }

            return View(requirement);
        }

        // GET:
        [Route("Requirements/Create")]
        [Route("Requirements/Create/{eventId}")]
        public IActionResult Create(int? eventId)
        {
            if (eventId != null)
            {
                var @event = _context.Events.FirstOrDefault(e => e.Id == eventId);
                ViewData["eventTitle"] = @event.Title;
            }
            return View();
        }

        // POST: Requirements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Requirements/Create/{eventId}")]
        [HttpPost]
        [Route("Requirements/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Comment,Is_general,Is_fulfilled")] Requirement requirement, int? eventId)
        {
            var reqLabel = "Bendrinis reikalavimas: <b> ";

            if (ModelState.IsValid)
            {

                if (!User.IsInRole("orgCoord") && eventId == null)
                    requirement.Is_general = true;

                if (eventId != null)
                {
                    var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
                    reqLabel = "Specifinis reikalavimas: <b> ";
                    if (@event == null)
                    {
                        TempData["danger"] = "Nerestas renginys su paskirtu ID";
                        return Redirect("Requirements/Create/{eventId}");
                    }
                    requirement.Event = @event;
                }
                _context.Add(requirement);
                await _context.SaveChangesAsync();
                if (eventId != null)
                {
                    TempData["success"] = reqLabel + requirement.Name + "</b> sėkmingai sukurtas!";
                    return RedirectToAction("Details", "Events", new { Id = eventId });
                }
                else
                {
                    TempData["success"] = reqLabel + requirement.Name + "</b> sėkmingai sukurtas!";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["success"] = reqLabel + requirement.Name + " </b> sėkmingai sukurtas!";

            return View(requirement);
        }

        // GET: Requirements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requirement = await _context.Requirements
                .Include(e => e.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (requirement == null)
            {
                return NotFound();
            }
            return View(requirement);
        }

        // POST: Requirements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Comment,Is_general,Is_fulfilled")] Requirement requirement, int? EventId)
        {
            if (id != requirement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var reqLabel = "Bendrinis reikalavimas: <b> ";
                try
                {
                    if (EventId != null)
                    {
                        reqLabel = "Specifinis reikalavimas: <b> ";
                    }
                    _context.Update(requirement);
                    await _context.SaveChangesAsync();

                    TempData["success"] = reqLabel + requirement.Name + " </b> sėkmingai atnaujintas!";
                    if (EventId != null)
                        return RedirectToAction("Details", "Events", new { Id = EventId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequirementExists(requirement.Id))
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
            return View(requirement);
        }

        // GET: Requirements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requirement = await _context.Requirements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requirement == null)
            {
                return NotFound();
            }

            return View(requirement);
        }

        // POST: Requirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requirement = await _context.Requirements.FindAsync(id);
            _context.Requirements.Remove(requirement);
            await _context.SaveChangesAsync();

            TempData["success"] = "Bendrinis reikalavimas: <b> " + requirement.Name + " </b> sėkmingai pašalintas!";

            return RedirectToAction(nameof(Index));
        }

        private bool RequirementExists(int id)
        {
            return _context.Requirements.Any(e => e.Id == id);
        }
    }
}
