using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers.ApiControllers
{
    [ApiController]
    [Route("Api/sponsors")]
    public class SponsorsControllerApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SponsorsControllerApi(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Sponsor>>> Index()
        {
            return Ok(await _context.Sponsors.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Sponsor>>> Get(int id)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(h => h.Id == id);
            if (sponsor == null)
                return BadRequest("Sponsor was not found");
            return Ok(sponsor);
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<Sponsor>> AddSponsor([FromBody] Sponsor sponsor)
        {
            _context.Sponsors.Add(sponsor);

            await _context.SaveChangesAsync();
            return Ok(await _context.Sponsors.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Sponsor>>> UpdateSponsor(Sponsor request)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(h => h.Id == request.Id);

            if (sponsor == null)
                return BadRequest("Sponsor was not found");

            sponsor.Title = request.Title;
            sponsor.CompanyType = request.CompanyType;
            sponsor.CompanyCode = request.CompanyCode;
            sponsor.CompanyVAT = request.CompanyVAT;
            sponsor.Address = request.Address;
            sponsor.PhoneNr = request.PhoneNr;
            sponsor.Email = request.Email;
            sponsor.CompanyHeadName = request.CompanyHeadName;
            sponsor.CompanyHeadSurname = request.CompanyHeadSurname;

            await _context.SaveChangesAsync();
            return Ok(await _context.Sponsors.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Sponsor>>> DeleteSponsor(int id)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(h => h.Id == id);

            if (sponsor == null)
                return BadRequest("Sponsor was not found");

            _context.Sponsors.Remove(sponsor);

            await _context.SaveChangesAsync();
            return Ok(await _context.Sponsors.ToListAsync());
        }
    }
}
