using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace KTU_SA_RO.Controllers
{
    public class EventTeamMembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventTeamMembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EventTeams
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EventTeamMembers;
            return View(await applicationDbContext.OrderByDescending(e => e.EventId).ToListAsync());
        }

        // GET: EventTeams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTeam = await _context.EventTeamMembers
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

        // POST: EventTeamMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,EventId,UserId,Is_event_coord")] EventTeamMember eventTeamMember, */
            int? eventId, string userName, string userSurname, string userEmail, string pickedPosition)
        {
            if (eventId != null && userName != null && userSurname != null)
            {
                var user = new ApplicationUser();

                if (userEmail != null)
                    user = await _context.Users.Where(u => u.Email.Equals(userEmail)).FirstOrDefaultAsync();
                else
                    user = await _context.Users.Where(u => u.Name.Equals(userName) && u.Surname.Equals(userSurname)).FirstOrDefaultAsync();

                if (user == null)
                {
                    TempData["danger"] = "Naudotojas nerastas";
                    return RedirectToAction(nameof(EventsController.Details),nameof(EventsController).Replace("Controller",""), new { id = eventId.ToString()});
                }
                var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                var a = setUserRole(pickedPosition);
                // if wanted user has the same position 
                if ( _context.EventTeamMembers.Where(et => et.EventId == eventId && et.UserId.Equals(user.Id) 
                    && userRole.Equals(setUserRole(pickedPosition))).FirstOrDefault() != null)
                {
                    TempData["danger"] = "Naudotojas su tokiais duomenimis ir pozicija jau egzistuoja komandoje";
                    return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = eventId.ToString() });
                }
                if (_context.EventTeamMembers.Where(et =>  et.UserId.Equals(user.Id)
                    && userRole.Equals(setUserRole(pickedPosition))).FirstOrDefault() == null)
                {
                    TempData["danger"] = "Pasirinktas naudotojas neturi tokios rolės";
                    return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = eventId.ToString() });
                }
                // if event coordinator already exists
                if (_context.EventTeamMembers.Where(et => et.EventId == eventId && et.UserId.Equals(user.Id)
                    && et.RoleName.Equals("eventCoord")).FirstOrDefault() != null)
                {
                    TempData["danger"] = "Renginio koordinatorius gali būti tik vienas";
                    return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = eventId.ToString() });
                }

                var eventTeamMember = new EventTeamMember()
                {
                    EventId = (int)eventId,
                    RoleName = setUserRole(pickedPosition),
                    UserId = user.Id
                };

                _context.Add(eventTeamMember);
                await _context.SaveChangesAsync();

                TempData["success"] = "Narys <b> " + user.Name + " " + user.Surname + 
                    "</b> sėkmingai pridėtas į komandą! Jo pozicija: <b>" + pickedPosition + "</b>";
                return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = eventId.ToString() });
            }
            return NotFound();
        }

        public string setUserRole(string positionName)
        {
            if (positionName.Equals("Registruotas naudotojas"))
                return "registered";
            if (positionName.Equals("Renginio koordinatorius"))
                return "eventCoord";
            if (positionName.Equals("ORK koordinatorius"))
                return "fsaOrgCoord";
            if (positionName.Equals("VIP koordinatorius"))
                return "fsaBussinesCoord";
            if (positionName.Equals("RSV koordinatorius"))
                return "fsaPrCoord";
            if (positionName.Equals("CSA ORK koordinatorius"))
                return "orgCoord";
            if (positionName.Equals("Administratorius"))
                return "admin";
            else
                return null;
        }

        // GET: EventTeams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventTeamMember = await _context.EventTeamMembers.FindAsync(id);
            if (eventTeamMember == null)
            {
                return NotFound();
            }
            // Pakeisti Id
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "CoordinatorName", eventTeamMember.Id);
            return View(eventTeamMember);
        }

        // POST: EventTeams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,UserId,Is_event_coord")] EventTeamMember eventTeamMember)
        {
            if (id != eventTeamMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventTeamMember);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Komanda <b> " + eventTeamMember.Id + "</b> sėkmingai atnaujinta!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventTeamExists(eventTeamMember.Id))
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
            // Pakeisti Id
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "CoordinatorName", eventTeamMember.Id);
            return View(eventTeamMember);
        }

        // GET: EventTeams/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var eventTeamMember = await _context.EventTeamMembers
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (eventTeamMember == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(eventTeamMember);
        //}

        // POST: EventTeams/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var eventTeamMember = await _context.EventTeamMembers.FindAsync(id);
            var eventId =  _context.EventTeamMembers.FirstOrDefault(et => et.Id == id).EventId.ToString();
            _context.EventTeamMembers.Remove(eventTeamMember);
            await _context.SaveChangesAsync();
            TempData["success"] = "Narys iš komandos sėkmingai pašalintas";
            return Redirect("~/Events/Details/" + eventId);
        }

        private bool EventTeamExists(int id)
        {
            return _context.EventTeamMembers.Any(e => e.Id == id);
        }
    }
}
