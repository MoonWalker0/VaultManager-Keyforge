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
    public class IndexTournamentModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        private readonly ApplicationDbContext _db;
        public IEnumerable<Tournament> Tournament { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public IndexTournamentModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            if(SearchTerm != null)
            {
                Tournament = from r in _db.Tournament
                             where //(r.Private == false //||
                                    //r.Shops.Contains(User.Identity.Name))
                                    //&& 
                                    r.Name.Contains(SearchTerm)
                                
                             select r;
            }
            else
            { 
                Tournament = from r in _db.Tournament
                             //where (r.Private == false //||
                                    //r.Shops.Contains(User.Identity.Name))
                             select r;
            }
        }
    }
}