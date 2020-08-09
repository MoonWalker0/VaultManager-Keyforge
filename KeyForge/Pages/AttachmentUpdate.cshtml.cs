using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KeyForge
{
    public class AttachmentUpdateModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public string ErrorMsg { get; set; }
        [BindProperty]
        public Attachments Attachment { get; set; }
        [BindProperty]
        public string TournamentID { get; set; }
        public IEnumerable<SelectListItem> AvailableTournaments { get; set; }
        private readonly ApplicationDbContext _db;
        public AttachmentUpdateModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _signInManager = signInManager;
        }
        public IActionResult OnGet(int? attachmentId)
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

            var tempAssignedTournament = (from r in _db.AttachmentAssignment
                                          where r.AttachmentId == attachmentId
                                          select r.TournamentId).FirstOrDefault();

            AvailableTournaments = (from r in _db.Tournament
                                    where r.Originator == User.Identities.FirstOrDefault().Name
                                    select new SelectListItem { Text = r.Name, Value = r.Id.ToString(), Selected = tempAssignedTournament == r.Id })
                                    .ToList();

            var tempEmail = (from r1 in _db.Attachments
                             join r2 in _db.Company on r1.Author equals r2.Id
                             where r1.Id == Attachment.Id
                             select r2.Email).FirstOrDefault();

            //Check if author
            if (tempEmail != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            return Page();
        }
        public IActionResult OnPost()
        {
            var tempAttachment = (from r in _db.Attachments
                                  where r.Id == Attachment.Id
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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Select elements to be updated
            tempAttachment.Name = Attachment.Name;
            tempAttachment.ShortDescription = Attachment.ShortDescription; 
            tempAttachment.Description = Attachment.Description; 
            _db.SaveChanges();

            //Attach to the tournament
            var tempAssignment = (from r in _db.AttachmentAssignment
                                  where r.AttachmentId == tempAttachment.Id select r).FirstOrDefault();
            tempAssignment.TournamentId = Convert.ToInt32(TournamentID);
            _db.SaveChanges();

            TempData["Message"] = "Zmiany zapisane!";
            return RedirectToPage("./AttachmentView", new { attachmentId = Attachment.Id });
        }
    }
}