using KTU_SA_RO.Models;
using System.Collections.Generic;

namespace KTU_SA_RO.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Compares similar events to this by removing title letters and picking chosen events count
        /// </summary>
        public List<Event> LastEventsStats(Event chosenEvent, int? chosenEventsCount, int removeLettersCount);
        /// <summary>
        /// Change enumerable role name to client readable role name
        /// </summary>
        public string SetUserPosition(string roleName);
    }
}
