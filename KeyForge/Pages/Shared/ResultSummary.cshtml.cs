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
    public class ResultSummaryModel : PageModel
    {
        public Tournament Tournament { get; set; }
        [BindProperty(SupportsGet = true)]
        public Player PlayerWin { get; set; }
        [BindProperty]
        public Player PlayerLose { get; set; }
        private readonly ApplicationDbContext _db;
        public IEnumerable<SelectListItem> AvailablePlayers { get; set; }
        public ResultSummaryModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult OnGet(string tournamentName, int? winnerId, string winnerName, int? loserId, string loserName)
        { 
            Tournament = new Tournament();
            PlayerWin = new Player();
            PlayerLose = new Player(); 

            if (tournamentName != null && tournamentName != "''")
            {
                Tournament.Name = tournamentName;
                Tournament.Id = (from r in _db.Tournament
                                 where r.Name == tournamentName
                                 select r.Id).FirstOrDefault();
            }
            else if (winnerId != null && winnerId != 0)
            {
                PlayerWin.Id = winnerId ?? 0;
            }
            else if (winnerName != null && winnerName != "''")
            {
                PlayerWin.Name = winnerName;
            }
            else if (loserId != null && loserId != 0)
            {
                PlayerWin.Id = loserId ?? 0;
            }
            else if (loserName != null && loserName != "''")
            {
                PlayerWin.Name = loserName;
            }
            return Page();
        }
    }
}