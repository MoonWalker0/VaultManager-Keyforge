using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyForge
{
    public class AttachmentDeleteModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public Attachments Attachment { get; set; }
        private readonly ApplicationDbContext _db;
        public AttachmentDeleteModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _signInManager = signInManager;
        }
        public IActionResult OnGet(int attachmentId)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("./NotFound");
            }

            Attachment = (from r in _db.Attachments
                          where r.Id == attachmentId
                          select r).FirstOrDefault();
            if (Attachment == null)
            {
                return RedirectToPage("./NotFound");
            }

            var tempEmail = (from r1 in _db.Attachments
                             join r2 in _db.Company on r1.Author equals r2.Id
                             where r1.Id == Attachment.Id
                             select r2.Email).FirstOrDefault();

            //Check if author
            if (tempEmail != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            Attachment.Data = "data:application/pdf;base64," + Attachment.Data;

            return Page(); 
        }

        public IActionResult OnPost()
        {
            var tempTournament = (from r in _db.Attachments
                                  where r.Id == Attachment.Id
                                  select r).FirstOrDefault();

            var tempAssignment = (from r in _db.AttachmentAssignment
                                  where r.AttachmentId == Attachment.Id
                                  select r).FirstOrDefault();

            var tempEmail = (from r1 in _db.Attachments
                             join r2 in _db.Company on r1.Author equals r2.Id
                             where r1.Id == Attachment.Id
                             select r2.Email).FirstOrDefault();

            //Check if author
            if (!_signInManager.IsSignedIn(User) || tempEmail != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            _db.Remove(tempTournament);
            _db.Remove(tempAssignment);
            _db.SaveChanges();

            TempData["Message"] = "Usunięto plik!";
            return RedirectToPage("./AttachmentHub");
        }
    }
}