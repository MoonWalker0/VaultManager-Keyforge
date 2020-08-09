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
    public class ResultHubModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public string ErrorMsg { get; set; }
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Tournament Tournament { get; set; }
        public IEnumerable<SelectListItem> AvailableTournaments { get; set; }
        public ResultHubModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
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

            //Show available tournaments assigned to this shop
            AvailableTournaments = (from r in _db.Tournament
                                    where r.Shops.Contains(User.Identity.Name) &&
                                    r.DateFrom <= DateTime.UtcNow.Date &&
                                    r.DateTo >= DateTime.UtcNow.Date
                                    select new SelectListItem { Text = r.Name, Value = r.Id.ToString() }).ToList();

            if (AvailableTournaments.Count() == 0)
            {
                ErrorMsg = "Nie znaleziono turniejów. Utwórz nowy lub dołącz do istniejącego.";
                return Page();
            }
            return Page();
        }

        public IActionResult OnPostSingle()
        {
            return RedirectRoutine("Result"); 
        }
        public IActionResult OnPostMulti()
        {
            return RedirectRoutine("MultiResult");
        }
        public IActionResult OnPostGEM()
        {
            return RedirectRoutine("GEMResult");
        }

        private IActionResult RedirectRoutine(string path)
        {

            //Show available tournaments assigned to this shop
            AvailableTournaments = (from r in _db.Tournament
                                    where r.Shops.Contains(User.Identity.Name) &&
                                    r.DateFrom <= DateTime.UtcNow.Date &&
                                    r.DateTo >= DateTime.UtcNow.Date
                                    select new SelectListItem { Text = r.Name, Value = r.Id.ToString() }).ToList();

            if (Tournament.Name == "0" || Tournament.Name == null)
            {
                ErrorMsg = "Wybierz turniej";
                return Page();
            }
            return RedirectToPage("./" + path + "/", new { tournamentId = Convert.ToInt32(Tournament.Name) });
        }
    }
}