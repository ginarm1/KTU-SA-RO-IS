using KTU_SA_RO.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Models
{
    public class SeedData
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public SeedData(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<bool> SeedAdminUser()
        {
            var user = new ApplicationUser
            {
                Id = "745cb9fa-0f03-44a8-b266-af578118b2c1",
                Name = "Gintaras",
                Surname = "Armonaitis",
                UserName = "gintaras.armonaitis@gmail.com",
                NormalizedUserName = "GINTARAS.ARMONAITIS@GMAIL.COM",
                Email = "gintaras.armonaitis@gmail.com",
                NormalizedEmail = "GINTARAS.ARMONAITIS@GMAIL.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var roleStore = new RoleStore<IdentityRole>(_context);

            if (!_context.Roles.Any(r => r.Name == "registered"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "registered", NormalizedName = "REGISTERED" });
            }
            if (!_context.Roles.Any(r => r.Name == "eventCoord"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "eventCoord", NormalizedName = "EVENTCOORD" });
            }
            if (!_context.Roles.Any(r => r.Name == "fsaOrgCoord"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "fsaOrgCoord", NormalizedName = "FSAORGCOORD" });
            }
            if (!_context.Roles.Any(r => r.Name == "fsaBussinesCoord"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "fsaBussinesCoord", NormalizedName = "FSABUSSINESCOORD" });
            }
            if (!_context.Roles.Any(r => r.Name == "fsaPrCoord"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "fsaPrCoord", NormalizedName = "FSAPRCOORD" });
            }
            if (!_context.Roles.Any(r => r.Name == "orgCoord"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "orgCoord", NormalizedName = "ORGCOORD" });
            }
            if (!_context.Roles.Any(r => r.Name == "admin"))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = "admin", NormalizedName = "ADMIN" });
            }

            if (!_context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<IdentityUser>();
                var hashed = password.HashPassword(user, "Slaptazodis123.");
                user.PasswordHash = hashed;
                var userStore = new UserStore<IdentityUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "ADMIN");
            }

            if (_context.EventTypes.Count() == 0)
            {
                _context.Add(new EventType { Name = "Vidinis" });
                _context.Add(new EventType { Name = "Masinis" });
                _context.Add(new EventType { Name = "Komercinis" });
                _context.Add(new EventType { Name = "Fakultetinis" });
                _context.Add(new EventType { Name = "Tarpastovybinis" });
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
