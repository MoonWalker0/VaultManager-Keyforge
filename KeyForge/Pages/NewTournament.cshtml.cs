using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Identity;

namespace KeyForge
{
    public class NewTournamentModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public string ErrorMsg { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        [TempData]
        public string Message { get; set; }
        private readonly ApplicationDbContext _db;
        public NewTournamentModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
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
            return Page();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Check if tournament with given name already exists
            bool isPresent = (from r in _db.Tournament
                              where r.Name == Tournament.Name
                              select r).Count() > 0;
            if(isPresent)
            { 
                ErrorMsg = "Turniej o takiej nazwie już istnieje";
                return Page();
            }

            //Assign to current user
            Tournament.Shops = User.Identity.Name;
            Tournament.Originator = User.Identity.Name;

            //Check if TO date is not after AFTER date
            if (DateTime.Compare(Tournament.DateTo, Tournament.DateFrom) < 0)
            {
                ErrorMsg = "Data startu nie może być późniejsza niż data końca!"; 
                return Page();
            }
            
            //Check if AFTER date is earlier than today
            if(DateTime.Compare(DateTime.UtcNow.Date, Tournament.DateTo) > 0)
            {
                ErrorMsg = "Data końca nie może być wcześniejsza niż aktualna!"; 
                return Page();
            }

            _db.Tournament.Add(Tournament);
            _db.SaveChanges();
            TempData["Message"] = "Turniej pomyślnie dodany!";
            return RedirectToPage("./IndexTournament");
        }
    }
}