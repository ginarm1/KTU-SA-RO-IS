using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using KTU_SA_RO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "admin, orgCoord")]
        // GET: Users
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            UserService userService = new UserService(_context,_roleManager,_userManager);
            ViewData["roles"] = await userService.GetUsersRole();

            /*Pagination*/
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling(_context.Users
                .Count() / (double)pageSize);

            ViewData["totalPages"] = totalPages;
            ViewData["pageIndex"] = pageIndex;

            return View(await _context.Users
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync());
        }

        [Route("Users/UserRole/{userId}")]
        public async Task<IActionResult> UserRole(string userId)
        {
            UserService userService = new UserService(_context, _roleManager, _userManager);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            ViewData["userRoleName"] = await userService.GetUserRole(user.Email);
            var userRole = await _context.UserRoles.Where(u => u.UserId.Equals(user.Id)).FirstOrDefaultAsync();
            if (userRole == null)
            {
                ViewData["roles"] = await _roleManager.Roles.OrderBy(r => r.Name)
                    .Select(r => r.Name)
                    .ToListAsync();
            }
            else
            {
                ViewData["roles"] = await _roleManager.Roles.OrderBy(r => r.Name)
                    .Where(r => !r.Id.Equals(userRole.RoleId) && !r.Name.Equals("admin"))
                    .Select(r => r.Name)
                    .ToListAsync();
            }

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(string pickedRole,
            ApplicationUser user)
        {
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var tempUser = await _context.Users.FirstOrDefaultAsync(a => a.Email.Equals(user.Email));
                var userRole = await _context.UserRoles.Where(u => u.UserId.Equals(tempUser.Id)).FirstOrDefaultAsync();
                var role = await _context.Roles.Where(r => r.Name.Equals(pickedRole)).FirstOrDefaultAsync();
                // Delete old userRole and create new one
                if (role != null && userRole != null)
                {
                    AddNewUserRole(tempUser, role.Id);
                    _context.UserRoles.Remove(userRole);
                    TempData["success"] = "Naudotojo <b>" + tempUser.Name + " " + tempUser.Surname + "</b> sena rolė sėkmingai pakeista į <b> " + role.Name + "</b>!";
                    await _context.SaveChangesAsync();
                }
                if (userRole == null)
                {
                    AddNewUserRole(tempUser, role.Id);
                    TempData["success"] = "Naudotojo <b> " + tempUser.Name + " " + tempUser.Surname + "</b> rolė sėkmingai paskirta!";
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private void AddNewUserRole(ApplicationUser tempUser, string roleId)
        {
            var newUserRole = new IdentityUserRole<string>
            {
                UserId = tempUser.Id,
                RoleId = roleId
            };
            _context.UserRoles.Add(newUserRole);
        }


        //POST: Users/UserRole/{userName}
        //[Route("Users/DeleteUser/{userId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (userId == null)
            {
                return NotFound($"Unable to find user.");
            }

            var DeleteUser = await _userManager.FindByIdAsync(userId);
            if (DeleteUser == null)
            {
                TempData["danger"] = "Naudotojas su ID: " + userId + " nerastas";
                return RedirectToAction(nameof(Index));
            }

            //var userCoordEvents = await _context.Events
            //    .Where(e => e.CoordinatorName.Equals(DeleteUser.Name) && e.CoordinatorSurname.Equals(DeleteUser.Surname)).ToListAsync();
            var userEventTeams = await _context.EventTeamMembers
                .Where(e => e.UserId.Equals(userId)).ToListAsync();
            var userRequirements = await _context.Requirements
                .Where(e => e.User.Equals(DeleteUser)).ToListAsync();

            if (userEventTeams != null)
                _context.EventTeamMembers.RemoveRange(userEventTeams);
            if (userRequirements != null)
                _context.Requirements.RemoveRange(userRequirements);

            await _userManager.DeleteAsync(DeleteUser);

            var successMsg = "Naudotojas:" + DeleteUser.Name + " " + DeleteUser.Surname;

            if (userEventTeams != null && userRequirements != null)
                TempData["success"] = "<b>" + successMsg + "<b/> iš <u>renginio komandos</u> ir <u>specifiniais reikalavimais</u> sėkmingai pašalintas!";
            else if (userEventTeams != null)
                TempData["success"] = "<b>" + successMsg + "<b/> su <u>renginio komandos nariais</u> sėkmingai pašalintas!";
            else if (userRequirements != null)
                TempData["success"] = "<b>" + successMsg + "<b/> su <u>specifiniais reikalavimais</u> sėkmingai pašalintas!";
            else
                TempData["success"] = "<b>" + successMsg + "<b/> sėkmingai pašalintas";

            TempData["success"] = "Naudotojas <b>" + DeleteUser.Name + DeleteUser.Surname + "</b> sėkmingai pašalintas!";
            return RedirectToAction(nameof(Index));
        }
    }
}
