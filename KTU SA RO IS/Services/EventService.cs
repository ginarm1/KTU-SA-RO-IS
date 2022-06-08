using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace KTU_SA_RO.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Event> LastEventsStats(Event chosenEvent, int? chosenEventsCount, int removeLettersCount)
        {
            if (chosenEvent.Title.Length > 3)
            {
                string titleTrimed = chosenEvent.Title.Remove(chosenEvent.Title.Length - removeLettersCount);
                var lastEvents = new List<Event>();

                var les = _context.Events.Where(e => e.Title.Contains(titleTrimed) && !e.StartDate.Equals(chosenEvent.StartDate) && !e.EndDate.Equals(chosenEvent.EndDate)).OrderByDescending(e => e.Id).Select(e => e.Id).AsEnumerable();
                if (chosenEventsCount > les.Count())
                    chosenEventsCount = les.Count();

                for (int i = 0; i < chosenEventsCount; i++)
                {
                    var lastEvent = _context.Events
                        .Where(e => e.Title.Contains(titleTrimed) && !e.StartDate.Equals(chosenEvent.StartDate) && !e.EndDate.Equals(chosenEvent.EndDate) && les.ElementAtOrDefault(i) == e.Id)
                        .Include(et => et.EventTeamMembers)
                        .Include(rv => rv.Revenues)
                        .Include(c => c.Costs)
                        .Include(t => t.Ticketings)
                        .Include(s => s.Sponsorships)
                        .FirstOrDefault();
                    lastEvents.Add(lastEvent);
                }
                return lastEvents;
            }

            return null;
        }

        public string SetUserPosition(string roleName)
        {
            if (roleName.Equals("registered"))
                return "Registruotas naudotojas";
            if (roleName.Equals("eventCoord"))
                return "Renginio koordinatorius";
            if (roleName.Equals("fsaOrgCoord"))
                return "ORK koordinatorius";
            if (roleName.Equals("fsaBussinesCoord"))
                return "VIP koordinatorius";
            if (roleName.Equals("fsaPrCoord"))
                return "RSV koordinatorius";
            if (roleName.Equals("orgCoord"))
                return "CSA ORK koordinatorius";
            if (roleName.Equals("admin"))
                return "Administratorius";
            else
                return null;
        }
    }
}
