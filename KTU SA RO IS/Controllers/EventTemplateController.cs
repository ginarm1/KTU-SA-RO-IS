using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KTU_SA_RO.Controllers
{
    public class EventTemplateController : Controller
    {
        private readonly IFileProvider fileProvider;

        public EventTemplateController(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            var model = new EventTemplate();
            foreach (var item in this.fileProvider.GetDirectoryContents(""))
            {
                model = new EventTemplate { Name = item.Name, Path = item.PhysicalPath };
            }
            model.Path = model.Path + "\\documents\\eventTemplate\\Renginio-šablonas.xlsx";
            model.Name = "Renginio-šablonas";


            FileInfo file = new FileInfo(model.Path);
            if (file.Exists)
                return View(model);
            else
                model = null;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["name_danger"] = "Nepasirinktas dokumentas";
                return RedirectToAction("Index");
            }
            //return Content("file not selected");



            if (file.FileName == null)
            {
                TempData["name_danger"] = "Dokumento vardo nėra";
                return RedirectToAction("Index");
            }
            //return Content("filename not present");

            if (!file.FileName.Contains(".xlsx"))
            {
                TempData["file_type_danger"] = "Netinkamas dokumento tipas! Dokumentas privalo būti xlsx tipo";
                return RedirectToAction("Index");
            }
            else if (file.FileName != "Renginio-šablonas.xlsx")
            {
                TempData["name_danger"] = "Dokumento pavadinimas privalo turėti pavadinimą: Renginio-šablonas";
                return RedirectToAction("Index");
            }

            var path = Path.Combine("Renginio_šablonas.xlsx",
                        Directory.GetCurrentDirectory(),
                        "wwwroot/lib/documents/eventTemplate",
                        file.FileName);

            if (GetContentType(path) == null)
            {
                return RedirectToAction("Index");
            }


            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            TempData["success"] = "Dokumentas sėkmingai patalpintas";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            if (filename != "Renginio-šablonas")
            {
                TempData["name_danger"] = "Dokumento pavadinimas privalo turėti pavadinimą: Renginio-šablonas";
                return View();
            }

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot/lib/documents/eventTemplate",
                            filename + ".xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (!ext.Equals(types.First().Key))
            {
                TempData["file_type_danger"] = "Netinkamas dokumento tipas! Dokumentas privalo būti xlsx tipo";
                return null;
            }
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                //{".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats"},  
                //  officedocument.spreadsheetml.sheet
            };
        }

        public IActionResult Delete(string path)
        {
            if (path == null)
            {
                return NotFound();
            }
            FileInfo file = new FileInfo(path);
            var model = new EventTemplate();

            file.Delete();

            TempData["success"] = "Renginio šablonas sėkmingai pašalintas";
            return RedirectToAction("Index");
        }
    }
}
