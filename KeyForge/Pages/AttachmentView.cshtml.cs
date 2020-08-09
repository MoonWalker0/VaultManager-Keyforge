using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyForge
{
    public class AttachmentViewModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public Attachments Attachment { get; set; }
        private readonly ApplicationDbContext _db;
        public AttachmentViewModel(ApplicationDbContext db)
        {
            _db = db; 
        }
        public IActionResult OnGet(int attachmentId)
        {
            Attachment = (from r in _db.Attachments
                          where r.Id == attachmentId 
                          select r).FirstOrDefault();
            if (Attachment == null)
            {
                return RedirectToPage("./NotFound");
            }

            Attachment.Data = "data:application/pdf;base64," + Attachment.Data;

            return Page();
        }
    }
}