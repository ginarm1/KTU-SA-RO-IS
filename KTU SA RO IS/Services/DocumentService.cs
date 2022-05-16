using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using KTU_SA_RO.Data;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace KTU_SA_RO.Services
{
    public class DocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public DocumentService(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public static void DeleteBrackets(Text[] words, int i)
        {
            words[i - 1].Text = words[i - 1].Text.Replace("{",string.Empty);
            words[i + 1].Text = words[i + 1].Text.Replace("}", string.Empty);
        }

        public async static Task<bool> ChangeTextInBrackets(int? sponsorId, MemoryStream memoryStream, 
            Sponsorship sponsorship, string companyLegalType, ICollection<Sponsorship> sponsorshipsDetails , ApplicationDbContext context)
        {
            // Use the file name and path passed in as an argument to 
            // open an existing document.            
            using (WordprocessingDocument doc =
                WordprocessingDocument.Open(memoryStream, true))
            {
                var sponsor = sponsorship.Sponsor;
                var @event = sponsorship.Event;

                var body = doc.MainDocumentPart.Document.Body;
                var paras = body.Elements<Paragraph>();

                // Every paragrah
                var document = doc.MainDocumentPart.Document;
                var words = document.Descendants<Text>().ToArray();

                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Text.Equals("{todayDate}") || words[i].Text.Equals("todayDate"))
                    {
                        if (words[i].Text.Equals("todayDate"))
                            DeleteBrackets(words, i);

                        words[i].Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }
                    else if (words[i].Text.Equals("{companyName}") || words[i].Text.Equals("companyName"))
                    {
                        if(words[i].Text.Equals("companyName"))
                            DeleteBrackets(words, i);

                        words[i].Text = sponsor.Title;
                    }
                    else if (words[i].Text.Equals("{companyType}") || words[i].Text.Equals("companyType"))
                    {
                        if (words[i].Text.Equals("companyType"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsor.CompanyType;

                        //switch (companyType)
                        //{
                        //    case "Uždaroji akcinė bendrovė":
                        //        words[i].Text = "UAB " + sponsor.Title;
                        //        break;
                        //    case "Akcinė bendrovė":
                        //        words[i].Text = "AB " + sponsor.Title;
                        //        break;
                        //    case "Mažoji bendrija":
                        //        words[i].Text = "MB " + sponsor.Title;
                        //        break;
                        //    case "Asociacija":
                        //        words[i].Text = "" + sponsor.Title;
                        //        break;
                        //    case "Viešoji įstaiga":
                        //        words[i].Text = "VšĮ " + sponsor.Title;
                        //        break;
                        //    case "Individuali įmonė":
                        //        words[i].Text = "IĮ " + sponsor.Title;
                        //        break;
                        //    default:
                        //        break;
                        //}
                    }
                    else if (words[i].Text.Equals("{companyVat}") || words[i].Text.Equals("companyVat"))
                    {
                        if(words[i].Text.Equals("companyVat"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsor.CompanyVAT;
                    }
                    else if (words[i].Text.Equals("{companyCode}") || words[i].Text.Equals("companyCode"))
                    {
                        if(words[i].Text.Equals("companyCode"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsor.CompanyCode;
                    }
                    else if (words[i].Text.Equals("{companyLegalType}") || words[i].Text.Equals("companyLegalType"))
                    {
                        if (words[i].Text.Equals("companyLegalType"))
                            DeleteBrackets(words, i);
                        words[i].Text = companyLegalType;
                    }
                    else if (words[i].Text.Equals("{companyAddress}") || words[i].Text.Equals("companyAddress"))
                    {
                        if(words[i].Text.Equals("companyAddress"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsor.Address;
                    }
                    else if (words[i].Text.Equals("{companyHeadNameSurname}") || words[i].Text.Equals("companyHeadNameSurname"))
                    {
                        if(words[i].Text.Equals("companyHeadNameSurname"))
                            DeleteBrackets(words, i);
                        words[i].Text = " " + sponsor.CompanyHeadName + " " + sponsor.CompanyHeadSurname;
                    }
                    else if (words[i].Text.Equals("{companyPhoneNr}") || words[i].Text.Equals("companyPhoneNr"))
                    {
                        if (words[i].Text.Equals("companyPhoneNr"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsor.PhoneNr;
                    }
                    else if (words[i].Text.Equals("{companyEmail}") || words[i].Text.Equals("companyEmail"))
                    {
                        if (words[i].Text.Equals("companyEmail"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsor.Email;
                    }
                    else if (words[i].Text.Equals("{eventTitle}") || words[i].Text.Equals("eventTitle"))
                    {
                        if(words[i].Text.Equals("eventTitle"))
                            DeleteBrackets(words, i);

                        words[i].Text = @event.Title;
                    }
                    else if (words[i].Text.Equals("{sponsorshipDescription}") || words[i].Text.Equals("sponsorshipDescription"))
                    {
                        if(words[i].Text.Equals("sponsorshipDescription"))
                            DeleteBrackets(words, i);
                        SponsorshipContainsPart(words[i], sponsorshipsDetails, sponsorId, @event.Id);
                    }
                    else if (words[i].Text.Equals("{sponsorshipSingleCost}") || words[i].Text.Equals("sponsorshipSingleCost"))
                    {
                        if(words[i].Text.Equals("sponsorshipSingleCost"))
                            DeleteBrackets(words, i);
                        words[i].Text = decimal.Round((decimal)(sponsorship.CostTotal / sponsorship.Quantity),2).ToString();
                    }
                    else if (words[i].Text.Equals("{sponsorshipCostTotal}") || words[i].Text.Equals("sponsorshipCostTotal"))
                    {
                        if (words[i].Text.Equals("sponsorshipCostTotal"))
                            DeleteBrackets(words, i);
                        words[i].Text = sponsorship.CostTotal.ToString();
                    }
                }
                await context.SaveChangesAsync();
            }
            return true;
        }

        public static void SponsorshipContainsPart(Text word, ICollection<Sponsorship> sponsorshipsDetails, int? sponsorId, int? eventId)
        {
            word.Text = "";
            foreach (var sponsorship in sponsorshipsDetails)
            {
                if (sponsorship.Sponsor.Id == sponsorId && sponsorship.Event.Id == eventId)
                {
                    word.Text += " " + sponsorship.Description + ", kurių 1 vnt. vertė yra " + decimal.Round((decimal)(sponsorship.CostTotal / sponsorship.Quantity), 2).ToString()
                        + " EUR , bendra materialinių vertybių suma " + sponsorship.CostTotal + " EUR (suma žodžiais). || ";
                }
            }
        }
    }
}
