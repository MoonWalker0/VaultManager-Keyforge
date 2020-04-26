using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KeyForge
{
    public class EditTournamentModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public string ErrorMsg { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        private readonly ApplicationDbContext _db;
        public EditTournamentModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _signInManager = signInManager;
        }
        public IActionResult OnGet(int? tournamentId)
        {
            if(!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("./NotFound");
            }

            Tournament = (from r in _db.Tournament
                          where r.Id == tournamentId
                          select r).FirstOrDefault();
            if (Tournament == null)
            {
                return RedirectToPage("./NotFound");
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            var tempTournament = (from r in _db.Tournament
                                  where r.Id == Tournament.Id 
                                  select r).FirstOrDefault();

            if (!ModelState.IsValid)
            { 
                return Page();
            }

            //Check if tournament with given name already exists
            bool isPresent = (from r in _db.Tournament
                              where r.Name == Tournament.Name
                                    && r.Id != Tournament.Id
                              select r).Count() > 0;
            if (isPresent)
            {
                ErrorMsg = "Turniej o takiej nazwie już istnieje";
                return Page();
            }

            //Check if TO date is not after AFTER date
            if (DateTime.Compare(Tournament.DateTo, Tournament.DateFrom) < 0)
            {
                ErrorMsg = "Data startu nie może być późniejsza niż data końca!";
                return Page();
            }

            //Check if AFTER date is earlier than today
            if (DateTime.Compare(DateTime.UtcNow.Date, Tournament.DateTo) > 0)
            {
                ErrorMsg = "Data końca nie może być wcześniejsza niż aktualna!";
                return Page();
            }
             
            //Select elements to be updated
            tempTournament.Name = Tournament.Name;
            tempTournament.DateFrom = Tournament.DateFrom;
            tempTournament.DateTo = Tournament.DateTo;
            tempTournament.Description = Tournament.Description;
            tempTournament.Private = Tournament.Private;
            tempTournament.ELOFactor = Tournament.ELOFactor;
            _db.SaveChanges();

            TempData["Message"] = "Zmiany zapisane!";
            return RedirectToPage("./DetailTournament", new { tournamentId = Tournament.Id });
        }
    }
}