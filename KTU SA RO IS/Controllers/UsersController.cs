using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: Users
        public async Task<IActionResult> Index()
        {
            ViewData["roles"] = await GetUsersRole();
            return View(await _context.Users.ToListAsync());
        }

        // Get users roles
        private async Task<Dictionary<string, IdentityRole>> GetUsersRole()
        {
            var UsersName = await _userManager.Users.Select(u => new { u.Id, u.UserName }).ToListAsync();
            var Roles = await _roleManager.Roles.ToListAsync();

            var UsersRoles = await _context.UserRoles.ToListAsync();

            var UserRole = new Dictionary<string, IdentityRole>();
            //var roles = new Dictionary<ApplicationUser, List<IdentityRole>>();
            foreach (var usr in UsersName)
            {
                //var rolelist = new List<IdentityRole>();
                foreach (var userRole in UsersRoles.Where(a => a.UserId.Equals(usr.Id)))
                {
                    var role = new IdentityRole();
                    role = Roles.Single(a => a.Id.Equals(userRole.RoleId));
                    //rolelist.Add(role);
                    UserRole.Add(usr.UserName, role);
                }

            }
            return UserRole;
        }

        // Get one user roles by his email
        private async Task<string> GetUserRole(string email)
        {
            var User = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            var Roles = await _roleManager.Roles.ToListAsync();

            var userRole = await _context.UserRoles.Where(u => u.UserId.Equals(User.Id)).FirstOrDefaultAsync();

            //var roles = new Dictionary<ApplicationUser, List<IdentityRole>>();
            string roleName = null;
            if (userRole != null)
            {
                var role = Roles.Single(a => a.Id.Equals(userRole.RoleId));
                roleName = role.Name;
            }

            else
                TempData["danger"] = "Nepriskirta rolė naudotojui arba rolė nerasta";

            return roleName;
        }

        [Route("Users/Role/")]
        public async Task<IActionResult> UserRole(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            ViewData["userRoleName"] = await GetUserRole(email);
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
                    .Where(r => !r.Id.Equals(userRole.RoleId))
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


        // GET: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            if (userName == null)
            {
                return NotFound($"Unable to find user.");
            }

            var DeleteUser = await _userManager.FindByNameAsync(userName);
            if (DeleteUser == null)
            {
                TempData["danger"] = "Nerandamas naudotojas: " + DeleteUser.UserName + ".";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(DeleteUser);
            TempData["success"] = "Naudotojas <b>" + DeleteUser.Name + DeleteUser.Surname + "</b> sėkmingai pašalintas!";
            return RedirectToAction(nameof(Index));
        }
    }
}
