using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KeyForge
{
    public class AttachmentAddModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Attachments Attachments { get; set; }
        [BindProperty]
        public string ErrorMsg { get; set; }
        [BindProperty]
        public IFormFile FileUpload { get; set; }
        [BindProperty]
        public string TournamentID { get; set; }
        public IEnumerable<SelectListItem> AvailableTournaments { get; set; }
        public AttachmentAddModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _signInManager = signInManager;
        }
        public IActionResult OnGet()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("./NotFound");
            }

            AvailableTournaments = (from r in _db.Tournament
                                    where r.Originator == User.Identities.FirstOrDefault().Name
                                    select new SelectListItem { Text = r.Name, Value = r.Id.ToString() })
                                    .ToList();

            Attachments = new Attachments();

            return Page();
        }

        public IActionResult OnPostFile(List<IFormFile> files)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("./NotFound");
            }

            if (files.Count == 0)
            {
                ErrorMsg = "Nie załadowano pliku.";
                return Page();
            }
            else if (files.Count > 1)
            {
                ErrorMsg = "Załadowano więcej niż jeden plik.";
                return Page();
            }
            else if (Attachments.ShortDescription.Length > 100)
            {
                ErrorMsg = "Krótki opis nie może przekraczać 100 znaków.";
                return Page();
            }

            IFormFile file = files.First();

            //Only pdfs
            if (!file.FileName.Contains("pdf"))
            {
                ErrorMsg = "Załączony plik nie jest plikiem pdf.";
                return Page();
            }

            //No more than 10 Mb
            if (file.Length > 10000000)
            {
                ErrorMsg = "Załadowano plik jest za duży, jego rozmiar nie może przekraczać 1 MB.";
                return Page();
            }

            //No more than 50Mb per user 
            int companyID = (from r in _db.Company
                             where r.Email == User.Identities.First().Name
                             select r.Id).FirstOrDefault();
            long sizeTotal = _db.Attachments.Where(t => t.Author == companyID).Sum(i => i.Size);
            if (sizeTotal > 5 * 10000000)
            {
                ErrorMsg = "Przekroczono limit 50MB na użytkownika. Usuń istniejący plik, aby odblokować miejsce.";
                return Page();
            }

            string base64file = null;
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                base64file = Convert.ToBase64String(stream.ToArray());
            }

            var newAttachment = new Attachments()
            {
                Author = companyID,
                Data = base64file,
                Name = file.FileName,
                ShortDescription = Attachments.ShortDescription,
                Description = Attachments.Description,
                Size = file.Length
            };

            _db.Add(newAttachment);
            _db.SaveChanges();

            //Attach to the tournament
            int tempAttachmentId = (from r in _db.Attachments
                                    where r.Id == newAttachment.Id
                                    select r.Id).FirstOrDefault();
            _db.Add(new AttachmentAssignment()
                {
                    TournamentId = TournamentID == null ? 0 : Convert.ToInt32(TournamentID),
                    AttachmentId = tempAttachmentId
                }
            );
            _db.SaveChanges();
             
            TempData["Message"] = "Plik pomyślnie dodany!";
            return RedirectToPage("./AttachmentHub");
        }
    }
}