using KTU_SA_RO.Data;
using KTU_SA_RO.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public DocumentsController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        [HttpPost("/Documents/GetSponsorshipAgreement")]
        public async Task<IActionResult> GetSponsorshipAgreement(int? sponsorId, int? eventId, string companyLegalType)
        {
            if (sponsorId == null || eventId == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(sp => sp.Sponsorships)
                    .ThenInclude(s => s.Sponsor)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            var sponsorshipsDetails = @event.Sponsorships;
            var sponsorship = await _context.Sponsorships.Include(e => e.Event).Include(s => s.Sponsor).FirstOrDefaultAsync(s => s.Event.Id == eventId && s.Sponsor.Id == sponsorId);

            var fileName = "Paramos-sutarties-šablonas.docx";

            DocumentService documentService = new DocumentService(_context, _appEnvironment);
            string templateFileName = _appEnvironment.WebRootPath + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar + "documents" 
                + Path.DirectorySeparatorChar + "sponsorshipAgreetment" + Path.DirectorySeparatorChar + fileName;
            byte[] byteArray = System.IO.File.ReadAllBytes(templateFileName);

            MemoryStream memorysStream = new MemoryStream();

            memorysStream.Write(byteArray, 0, byteArray.Length);

            if (!await DocumentService.ChangeTextInBrackets(sponsorId, memorysStream, sponsorship, companyLegalType, sponsorshipsDetails , _context))
            {
                TempData["danger"] = "";
                return RedirectToAction(nameof(EventsController.Details), nameof(EventsController).Replace("Controller", ""), new { id = @event.Id.ToString() });
            }

            byte[] document = memorysStream.ToArray();

            //https://stackoverflow.com/a/30893427
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(document, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"); // for .docx
        }
    }
}
