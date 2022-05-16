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
    [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
    public class SponsorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SponsorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sponsors
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            /*Pagination*/
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling(_context.Sponsors
                .Count() / (double)pageSize);

            ViewData["totalPages"] = totalPages;
            ViewData["pageIndex"] = pageIndex;

            return View(await _context.Sponsors
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .OrderBy(s => s.Title)
                .ToListAsync());
        }

        // GET: Sponsors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsor = await _context.Sponsors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sponsor == null)
            {
                return NotFound();
            }

            return View(sponsor);
        }

        // GET: Sponsors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sponsors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CompanyType,CompanyCode,CompanyVAT,Address,PhoneNr,Email,CompanyHeadName,CompanyHeadSurname")] Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sponsor);
                await _context.SaveChangesAsync();
                TempData["success"] = "Rėmėjas <b>" + sponsor.Title + "</b> sėkmingai sukurtas!";
                return RedirectToAction(nameof(Index));
            }
            return View(sponsor);
        }

        //// GET: Sponsors/CreateSponsorship
        //public IActionResult CreateSponsorship()
        //{
        //    return View();
        //}

        // POST: Sponsors/CreateSponsorship
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSponsorship([Bind("Id,Description,Quantity,CostTotal")]
            int? eventId, int? sponsorId, string description,int? quantity, double? costTotal)
        {

            if (eventId != null && description != null && quantity != null && costTotal != null)
            {
                Sponsorship sponsorship = new()
                {
                    Description = description,
                    Quantity = (int)quantity,
                    CostTotal = (double)costTotal,
                    Event = await _context.Events.FirstOrDefaultAsync(e => e.Id == (int)eventId),
                    Sponsor = await _context.Sponsors.FirstOrDefaultAsync(e => e.Id == (int)sponsorId)

                };
                _context.Add(sponsorship);
                await _context.SaveChangesAsync();
                TempData["success"] = "Vienas iš rėmimų sėkmingai sukurtas!";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
            else
            {
                TempData["danger"] = "Negeri duomenys apie vieną iš rėmėjų";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
        }

        // GET: Sponsors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsor = await _context.Sponsors.FindAsync(id);
            if (sponsor == null)
            {
                return NotFound();
            }
            return View(sponsor);
        }

        // POST: Sponsors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,CompanyType,CompanyCode,CompanyVAT,Address,PhoneNr,Email,CompanyHeadName,CompanyHeadSurname")] Sponsor sponsor)
        {
            if (id != sponsor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sponsor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SponsorExists(sponsor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["success"] = "Rėmėjas: <b>" + sponsor.Title + "</b> sėkmingai atnaujintas!";

                return RedirectToAction(nameof(Details), nameof(SponsorsController).Replace("Controller", ""), new { id = sponsor.Id.ToString()});
            }
            return View(sponsor);
        }

        // POST: Sponsors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sponsor = await _context.Sponsors.FindAsync(id);
            _context.Sponsors.Remove(sponsor);
            await _context.SaveChangesAsync();
            TempData["success"] = "Rėmėjas: <b>" + sponsor.Title + "</b> sėkmingai pašalintas!";
            return RedirectToAction(nameof(Index));
        }

        private bool SponsorExists(int id)
        {
            return _context.Sponsors.Any(e => e.Id == id);
        }

        //[Route("Sponsor/EditSponsorship/{id}")]
        public async Task<ActionResult> EditSponsorship(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsorship = await _context.Sponsorships
                .Include(e => e.Event)
                .Include(s => s.Sponsor)
                .FirstOrDefaultAsync(r => r.Id == id);

            ViewData["sponsors"] = await _context.Sponsors.ToListAsync();

            if (sponsorship == null)
            {
                return NotFound();
            }
            return View(sponsorship);
        }

        // POST: SponsorsController/EditSponsorship/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSponsorship(int id, [Bind("Id,Description,Quantity,CostTotal")] Sponsorship sponsorship, Event @event, int sponsorId)
        {
            if (id != sponsorship.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid || ModelState.ElementAt(2).Value.AttemptedValue == null)
            {
                try
                {
                    sponsorship.Sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.Id == sponsorId);
                    _context.Update(sponsorship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SponsorshipExists(sponsorship.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Rėmėjo: <b>" + sponsorship.Sponsor.Title + "</b> parama sėkmingai atnaujinta!";
                return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
            }
            return View(sponsorship);
        }

        // POST: Sponsors/Delete/5
        [HttpPost, ActionName("DeleteSponsorship")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSponsorshipConfirmed(int id)
        {
            var sponsorship = await _context.Sponsorships
                .Include(e => e.Event)
                .Include(e => e.Sponsor)
                .FirstOrDefaultAsync(s => s.Id == id);

            _context.Sponsorships.Remove(sponsorship);
            await _context.SaveChangesAsync();
            TempData["success"] = "Rėmėjo: <b>" + sponsorship.Sponsor.Title + "</b> parama, kurio ID: <b>" + sponsorship.Id + "</b> sėkmingai pašalinta!";
            return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = sponsorship.Event.Id.ToString() });
        }

        private bool SponsorshipExists(int id)
        {
            return _context.Sponsorships.Any(e => e.Id == id);
        }


    }
}
