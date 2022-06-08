using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<Dictionary<string, IdentityRole>> GetUsersRole()
        {
            var UsersName = await _userManager.Users.Select(u => new { u.Id, u.UserName }).ToListAsync();
            var Roles = await _roleManager.Roles.ToListAsync();

            var UsersRoles = await _context.UserRoles.ToListAsync();
            var UserRole = new Dictionary<string, IdentityRole>();

            foreach (var usr in UsersName)
            {
                foreach (var userRole in UsersRoles.Where(a => a.UserId.Equals(usr.Id)))
                {
                    var role = new IdentityRole();
                    role = Roles.Single(a => a.Id.Equals(userRole.RoleId));
                    UserRole.Add(usr.UserName, role);
                }
            }
            return UserRole;
        }

        // Get one user roles by his email
        public async Task<string> GetUserRole(string email)
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

            return roleName;
        }

        public void AddNewUserRole(ApplicationUser tempUser, string roleId)
        {
            var newUserRole = new IdentityUserRole<string>
            {
                UserId = tempUser.Id,
                RoleId = roleId
            };
            _context.UserRoles.Add(newUserRole);
        }
    }
}
