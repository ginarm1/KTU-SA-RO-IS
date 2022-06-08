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
using System.Security.Claims;
using System.Threading.Tasks;

namespace KTU_SA_RO.ApiControllers
{
    [ApiController]
    [Route("Api/events")]
    public class EventsControllerApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsControllerApi(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Event>>> Index(int pageIndex = 1)
        {
            int pageSize = 10;
            var totalPages = (int)Math.Ceiling(_context.Events
                .Count() / (double)pageSize);

            return Ok(await _context.Events
                .Include(tm => tm.EventTeamMembers)
                .OrderByDescending(e => e.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync());
        }

        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
        // GET: Events/UserEvents/{userId}
        [HttpGet("userEvents/{userId}")]
        public async Task<ActionResult<List<Event>>> UserEvents(string userId, string title, int pageIndex = 1)
        {
            var userTeams = await _context.EventTeamMembers.Where(u => u.UserId.Equals(userId)).ToListAsync();

            if(userTeams.Count == 0)
                return BadRequest("Jūsų renginių komandų nebuvo rasta arba naudotojas su tokiu ID nebuvo rastas");

            var events = new List<Event>();
            if (title == null)
                events = await _context.Events.ToListAsync();
            else
                events = await _context.Events.Where(e => e.Title.Equals(title) || e.Title.Contains(title)).ToListAsync();

            var userEvents = new List<Event>();
            var users = await _context.Users.ToListAsync();

            foreach (var @event in events)
            {
                foreach (var userTeam in userTeams)
                {
                    if (@event.Id.Equals(userTeam.EventId))
                    {
                        userEvents.Add(@event);
                    }
                }
            }

            if (userEvents.Count == 0)
                return BadRequest("Jūsų renginių nebuvo rasta");

            return Ok(userEvents);
        }

        [Authorize(Roles = "admin,eventCoord,fsaOrgCoord,fsaBussinesCoord,fsaPrCoord,orgCoord")]
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Event>>> Details(int? id, int? lastEventsCount)
        {
            if (id == null)
                return BadRequest("Renginys pagal ID nebuvo rastas");

            var positions = new List<string>();
            EventService eventService = new EventService(_context);

            // convert user roles to positions
            foreach (var role in Enum.GetValues(typeof(Role)))
            {
                if (!role.Equals("admin"))
                    positions.Add(eventService.SetUserPosition(role.ToString()));
            }

            var @event = await _context.Events
                .Include(r => r.Requirements.Where(e => e.Event.Id == id))
                    .ThenInclude(u => u.User)
                .Include(et => et.EventTeamMembers)
                .Include(rv => rv.Revenues)
                .Include(c => c.Costs)
                .Include(t => t.Ticketings)
                .Include(s => s.Sponsorships)
                    .ThenInclude(a => a.Sponsor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if(@event == null)
                return BadRequest("Renginys pagal ID nebuvo rastas");

            //if (lastEventsCount != null)
            //    ViewData["lastEvents"] = eventService.LastEventsStats(@event, lastEventsCount, 4);
            //else
            //    ViewData["lastEvents"] = eventService.LastEventsStats(@event, 3, 4);

            //ViewData["sponsors"] = await _context.Sponsors.ToListAsync();
            //ViewData["eventSponsors"] = @event.Sponsorships.Select(s => s.Sponsor).Distinct().ToList();

            var eventTeam = new Dictionary<int, ApplicationUser>();
            var membersRole = new Dictionary<int, string>();
            int i = 0;

            foreach (var eventTeamMember in @event.EventTeamMembers)
            {
                i = eventTeamMember.Id;
                if (eventTeamMember.EventId == id)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id.Equals(eventTeamMember.UserId));
                    if (user != null)
                    {
                        eventTeam.Add(i, user);
                        membersRole.Add(i, eventService.SetUserPosition(eventTeamMember.RoleName));
                    }
                    else
                        return BadRequest("Naudotojas nebuvo rastas");
                }
            }

            if (@event == null)
                return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
                return BadRequest("Prisijungusio naudotojo ID nebuvo rastas");

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(currentUserId));
            var currentRole = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault();

            if (currentRole.Equals("admin") || currentRole.Equals("eventCoord") || currentRole.Equals("fsaOrgCoord"))
            {
                var users = await _context.Users.OrderBy(u => u.Name).ToListAsync();
                var usersNameSurname = new List<string>();

                foreach (var user in users)
                {
                    var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    var role = await _roleManager.Roles.FirstOrDefaultAsync(a => a.Name.Equals(userRole));
                    usersNameSurname.Add(user.Name + " " + user.Surname + " || " + eventService.SetUserPosition(role.Name));
                    //ViewData["usersNameSurname"] = usersNameSurname;
                }
            }

            //ViewData["eventTeam"] = eventTeam;
            //ViewData["membersRole"] = membersRole;
            //ViewData["revenues"] = @event.Revenues.Where(r => r.Event == @event).ToList();
            //ViewData["costs"] = @event.Costs.Where(r => r.Event == @event).ToList();
            //ViewData["ticketings"] = @event.Ticketings.Where(r => r.Event == @event).ToList();
            //ViewData["userBelongsToEvent"] = await _context.EventTeamMembers.FirstOrDefaultAsync(e => e.UserId.Equals(currentUserId) && e.EventId == @event.Id);

            return Ok(@event);
        }
    }
}
