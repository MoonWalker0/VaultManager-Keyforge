using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KeyForge
{
    public class AttachmentHubModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        private readonly ApplicationDbContext _db;
        public IEnumerable<vAttachments> Attachments { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTournamentID { get; set; }
        public IEnumerable<SelectListItem> AvailableTournaments { get; set; }
        public AttachmentHubModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? tournamentId)
        {  
            if(SearchTournamentID == null)
            {
                //Call from other site
                if (tournamentId != null)
                {
                    SearchTournamentID = tournamentId.ToString();
                }
                else
                {
                    SearchTournamentID = "0";
                }
            }

            AvailableTournaments = (from r in _db.Tournament 
                                    select new SelectListItem { Text = r.Name, 
                                                                Value = r.Id.ToString(), 
                                                                Selected = Convert.ToInt32(SearchTournamentID) == r.Id }).ToList();
             
            if (SearchTerm != null)
            {
                Attachments = from r1 in _db.Attachments
                              join r2 in _db.Company on r1.Author equals r2.Id
                              join r3 in _db.AttachmentAssignment on r1.Id equals r3.AttachmentId 
                              where r1.Name.Contains(SearchTerm) &&
                                    r3.TournamentId == Convert.ToInt32(SearchTournamentID) 
                              select new vAttachments()
                              {
                                  Id = r1.Id,
                                  Author = r2.Name,
                                  Data = r1.Data,
                                  Description = r1.Description,
                                  Name = r1.Name,
                                  ShortDescription = r1.ShortDescription,
                                  Email = r2.Email,
                                  Tournament = r3.TournamentId == 0 ? "-" : (from r4 in _db.Tournament
                                                                             where r4.Id == r3.TournamentId
                                                                             select r4.Name).FirstOrDefault().ToString()
                              };
            }
            else
            {
                Attachments = from r1 in _db.Attachments
                              join r2 in _db.Company on r1.Author equals r2.Id
                              join r3 in _db.AttachmentAssignment on r1.Id equals r3.AttachmentId
                              where r3.TournamentId == Convert.ToInt32(SearchTournamentID)
                              select new vAttachments()
                              {
                                  Id = r1.Id,
                                  Author = r2.Name,
                                  Data = r1.Data,
                                  Description = r1.Description,
                                  Name = r1.Name,
                                  ShortDescription = r1.ShortDescription,
                                  Email = r2.Email,
                                  Tournament = r3.TournamentId == 0 ? "-" : (from r4 in _db.Tournament
                                                                             where r4.Id == r3.TournamentId
                                                                             select r4.Name).FirstOrDefault().ToString()
                              };
            }
             
        }
    }
}