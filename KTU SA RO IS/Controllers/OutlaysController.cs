using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers
{
    public class OutlaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OutlaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Event/Details/5/Revenue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Route("Event/Details/{eventId}/Revenue/Create")]
        [Route("Outlay/CreateRevenue")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRevenue(/*[Bind("Id,Title,Earned,InvoiceNr,Date,Comment")]*/
            int? eventId, string sponsorTitle, string title, double earned, string invoiceNr, DateTime date, string comment)
        {
            if (eventId != null && title != null && invoiceNr != null)
            {
                Revenue revenue = new()
                {
                    Title = title,
                    Earned = earned,
                    InvoiceNr = invoiceNr,
                    Date = date,
                    Comment = comment
                };
                revenue.Event = await _context.Events.FirstOrDefaultAsync(e => e.Id == (int)eventId);
                _context.Add(revenue);
                await _context.SaveChangesAsync();
                TempData["success"] = "Viena iš pajamų: <b>" + revenue.Title + "</b> sėkmingai sukurta!";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
            else
            {
                TempData["danger"] = "Negeri duomenys apie vieną iš pajamų";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
        }

        // GET: OutlaysController/Edit/5
        //[Route("Outlay/EditRevenue/{id}")]
        public async Task<ActionResult> EditRevenue(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revenue = await _context.Revenues
                .Include(e => e.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (revenue == null)
            {
                return NotFound();
            }
            return View(revenue);
        }

        // POST: OutlaysController/EditRevenue/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRevenue(int id, [Bind("Id,Title,Earned,InvoiceNr,Date,Comment")] Revenue revenue, Event @event)
        {
            if (id != revenue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid || ModelState.ElementAt(2).Value.AttemptedValue == null)
            {
                try
                {
                    _context.Update(revenue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RevenueExists(revenue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Vienas iš pajamų: <b>" + revenue.Title + "</b> sėkmingai atnaujintas!";
                return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
            }
            return View(revenue);
        }

        // POST: OutlaysController//DeleteRevenue/5
        [HttpPost, ActionName("DeleteRevenue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRevenueConfirmed(int id, Event @event)
        {
            var revenue = await _context.Revenues.FindAsync(id);
            _context.Revenues.Remove(revenue);
            await _context.SaveChangesAsync();
            TempData["success"] = "Vienas iš pajamų: <b>" + revenue.Title + "</b> sėkmingai pašalintas!";
            return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
        }

        private bool RevenueExists(int id)
        {
            return _context.Revenues.Any(e => e.Id == id);
        }

        // POST: Event/Details/5/Cost/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Outlay/CreateCost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCost(/*[Bind("Id,Title,Earned,InvoiceNr,Date,Comment")]*/
            int? eventId, string title, double price, string invoiceNr, DateTime date, string comment)
        {
            if (eventId != null && title != null && invoiceNr != null)
            {
                Cost cost = new()
                {
                    Title = title,
                    Price = price,
                    InvoiceNr = invoiceNr,
                    Date = date,
                    Comment = comment
                };
                cost.Event = await _context.Events.FirstOrDefaultAsync(e => e.Id == (int)eventId);
                _context.Add(cost);
                await _context.SaveChangesAsync();
                TempData["success"] = "Viena iš išlaidų: <b>" + cost.Title + "</b> sėkmingai sukurta!";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
            else
            {
                TempData["danger"] = "Negeri duomenys apie vieną iš išlaidų";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
        }

        //[Route("Outlay/EditCost/{id}")]
        public async Task<ActionResult> EditCost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.Costs
                .Include(e => e.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (cost == null)
            {
                return NotFound();
            }
            return View(cost);
        }

        // POST: OutlaysController/EditCost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCost(int id, [Bind("Id,Title,Price,InvoiceNr,Date,Comment")] Cost cost, Event @event)
        {
            if (id != cost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid || ModelState.ElementAt(2).Value.AttemptedValue == null)
            {
                try
                {
                    _context.Update(cost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CostExists(cost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Vienas iš išlaidų: <b>" + cost.Title + "</b> sėkmingai atnaujintas!";
                return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
            }
            return View(cost);
        }
        private bool CostExists(int id)
        {
            return _context.Revenues.Any(e => e.Id == id);
        }

        // POST: OutlaysController//DeleteCost/5
        [HttpPost, ActionName("DeleteCost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCostConfirmed(int id, Event @event)
        {
            var cost = await _context.Costs.FindAsync(id);
            _context.Costs.Remove(cost);
            await _context.SaveChangesAsync();
            TempData["success"] = "Vienas iš išlaidų: <b>" + cost.Title + "</b> sėkmingai pašalintas!";
            return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
        }

        // POST: Event/Details/5/Ticketing/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Outlay/CreateTicketing")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicketing(/*[Bind("Id,Price,Quantity")] */
            int? eventId, double price, int? quantity)
        {
            if (eventId != null && quantity != null)
            {
                Ticketing ticketing = new()
                {
                    Price = price,
                    Quantity = (int)quantity
                };
                ticketing.Event = await _context.Events.FirstOrDefaultAsync(e => e.Id == (int)eventId);
                _context.Add(ticketing);
                await _context.SaveChangesAsync();
                TempData["success"] = "Viena iš bilietacijų sėkmingai sukurta!";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
            else
            {
                TempData["danger"] = "Negeri duomenys apie vieną iš bilietacijų";
                return Redirect("~/Events/Details/" + eventId.ToString());
            }
        }

        //[Route("Outlay/EditTicketing/{id}")]
        public async Task<ActionResult> EditTicketing(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketing = await _context.Ticketings
                .Include(e => e.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (ticketing == null)
            {
                return NotFound();
            }
            return View(ticketing);
        }

        // POST: OutlaysController/EditCost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicketing(int id, [Bind("Id,Price,Quantity")] Ticketing ticketing, Event @event)
        {
            if (id != ticketing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid || ModelState.ElementAt(1).Value.AttemptedValue == null)
            {
                try
                {
                    _context.Update(ticketing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketingtExists(ticketing.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Vienas iš bilietacijų sėkmingai atnaujintas!";
                return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
            }
            return View(ticketing);
        }
        private bool TicketingtExists(int id)
        {
            return _context.Revenues.Any(e => e.Id == id);
        }

        // POST: OutlaysController//DeleteRevenue/5
        [HttpPost, ActionName("DeleteTicketing")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTicketingConfirmed(int id, Event @event)
        {
            var ticketing = await _context.Ticketings.FindAsync(id);
            _context.Ticketings.Remove(ticketing);
            await _context.SaveChangesAsync();
            TempData["success"] = "Viena iš bilietacijos sėkmingai pašalintas!";
            return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
        }
    }
}

