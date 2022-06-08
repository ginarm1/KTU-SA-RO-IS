using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using KTU_SA_RO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KTU_SA_RO.ApiControllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersControllerApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersControllerApi(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Get users
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ApplicationUser>>> Index(int pageIndex = 1)
        {
            /*Pagination*/
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling(_context.Users
                .Count() / (double)pageSize);

            return Ok(await _context.Users
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync());
        }

        [HttpGet("Users/UserRole/{userId}")]
        public async Task<IActionResult> UserRole(string userId)
        {
            UserService userService = new UserService(_context, _roleManager, _userManager);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            //ViewData["userRoleName"] = await userService.GetUserRole(user.Email);
            var userRole = await _context.UserRoles.Where(u => u.UserId.Equals(user.Id)).FirstOrDefaultAsync();
            if (userRole == null)
            {
                //ViewData["roles"] = await _roleManager.Roles.OrderBy(r => r.Name)
                //    .Select(r => r.Name)
                //    .ToListAsync();
            }
            else
            {
                //ViewData["roles"] = await _roleManager.Roles.OrderBy(r => r.Name)
                //    .Where(r => !r.Id.Equals(userRole.RoleId) && !r.Name.Equals("admin"))
                //    .Select(r => r.Name)
                //    .ToListAsync();
            }

            return Ok(user);
        }
    }
}
