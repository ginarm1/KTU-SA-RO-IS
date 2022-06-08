using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.ApiControllers
{
    [ApiController]
    [Route("Api/requirements")]
    public class RequirementsControllerApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RequirementsControllerApi(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Requirement>>> Index()
        {
            return Ok(await _context.Requirements.ToListAsync());
        }

        /// <summary>
        /// Additional requirements are for particular event
        /// </summary>
        [HttpGet("additional/{eventId}")]
        public async Task<ActionResult<List<Requirement>>> GetAdditionalReq(int eventId)
        {
            var @event = await _context.Events.Where(e => e.Id == eventId).FirstOrDefaultAsync();
            var requirements = await _context.Requirements.Where(r => r.Event == @event).ToListAsync();

            if (requirements.Count == 0)
                return BadRequest("Additional requirements were not found");

            return Ok(requirements);
        }

        /// <summary>
        /// Generic requirements are for all events
        /// </summary>
        [HttpGet("generic/{id}")]
        public async Task<IActionResult> GetGenericReq(int? id)
        {
            var requirement = await _context.Requirements
                .Include(e => e.Event)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (requirement == null)
                return BadRequest("Generic requirement was not found");

            return Ok(requirement);
        }
    }
}
