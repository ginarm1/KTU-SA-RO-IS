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
using Microsoft.AspNetCore.Authorization;

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

        // POST: EventTeamMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,orgCoord")]
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
                var userWithSamePosition = await _context.EventTeamMembers.Where(et => et.EventId == eventId && et.RoleName.Equals(userRole)).FirstOrDefaultAsync();
                // if wanted user has the same position 
                if ( _context.EventTeamMembers.Where(et => et.EventId == eventId && et.UserId.Equals(user.Id)
                    && userRole.Equals(SetUserRole(pickedPosition))).FirstOrDefault() != null || userWithSamePosition != null)
                {
                    TempData["danger"] = "Naudotojas su tokiais duomenimis ir pozicija jau egzistuoja komandoje";
                    return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = eventId.ToString() });
                }
                else if (!userRole.Equals(SetUserRole(pickedPosition)))
                {
                    TempData["danger"] = "Pasirinktas naudotojas neturi tokios rolės";
                    return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = eventId.ToString() });
                }

                var eventTeamMember = new EventTeamMember()
                {
                    EventId = (int)eventId,
                    RoleName = SetUserRole(pickedPosition),
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

        public string SetUserRole(string positionName)
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

        // POST: EventTeams/Delete/5
        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,orgCoord")]
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var eventTeamMember = await _context.EventTeamMembers.FindAsync(id);
            var eventId =  eventTeamMember.EventId;
            var @event = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(eventTeamMember.UserId);

            if (!user.Name.Equals(@event.CoordinatorName) && !user.Surname.Equals(@event.CoordinatorSurname))
            {
                _context.EventTeamMembers.Remove(eventTeamMember);
                await _context.SaveChangesAsync();
                TempData["success"] = "Narys iš komandos sėkmingai pašalintas";
            }
            else
            {
                TempData["danger"] = "Renginio koordinatorius negali būti pašalintas iš renginio. " +
                    "Pakeiskite renginio koordinatorių per renginio redagavimo puslapį";
            }

            return Redirect("~/Events/Details/" + eventId.ToString());
        }

        private bool EventTeamExists(int id)
        {
            return _context.EventTeamMembers.Any(e => e.Id == id);
        }
    }
}
