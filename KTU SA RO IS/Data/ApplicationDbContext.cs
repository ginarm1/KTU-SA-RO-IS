using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<EventTeamMember> EventTeamMembers { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Sponsorship> Sponsorships { get; set; }
    }
}
