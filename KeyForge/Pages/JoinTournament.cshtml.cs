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
    public class JoinTournamentModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        private readonly ApplicationDbContext _db;
        public JoinTournamentModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _signInManager = signInManager;
        }
        public IActionResult OnGet(int tournamentId)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("./NotFound");
            }

            Tournament = (from r in _db.Tournament
                          where r.Id == tournamentId
                                && r.Private == false
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

            tempTournament.Shops = Tournament.Shops + ";" + User.Identity.Name;
            _db.SaveChanges();

            TempData["Message"] = "Dołączono do turnieju!";
            return RedirectToPage("./IndexTournament");
        }
    }
}