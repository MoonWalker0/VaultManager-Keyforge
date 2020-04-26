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
    public class DetailTournamentModel : PageModel
    {
        [BindProperty]
        public string Places { get; set; }
        [BindProperty]
        public int PlacesCount { get; set; }
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        private readonly ApplicationDbContext _db;
        public DetailTournamentModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult OnGet(int tournamentId)
        {
            Tournament = (from r in _db.Tournament
                          where r.Id == tournamentId
                          select r).FirstOrDefault();
            if (Tournament == null)
            {
                return RedirectToPage("./NotFound");
            }

            //Get all addresses
            var emails = Tournament.Shops.Split(';');
            PlacesCount = emails.Count();

            Places = "";
            foreach(string email in emails)
            {
                var tempCompany= (from r in _db.Company
                                  where r.Email == email
                                  select r).FirstOrDefault();

                Places += "\"" + tempCompany.Name + "\" - " + tempCompany.Address + "\n";
            }
             
            return Page();
        }
    }
}