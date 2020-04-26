using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyForge
{
    public class ResultTournamentModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        [BindProperty]
        public string OrderProperty { get; set; }
        public IEnumerable<vLeaderboard> Leaderboard { get; set; }
        private readonly ApplicationDbContext _db;
        public ResultTournamentModel(ApplicationDbContext db)
        {
            //OrderProperty = "";
            _db = db;
        }

        public IActionResult OnGet(int? tournamentId, string orderBy)
        {   
            OrderProperty = orderBy;
            Tournament = (from r in _db.Tournament
                          where r.Id == tournamentId
                          select r).FirstOrDefault();
            if (Tournament == null)
            {
                return RedirectToPage("./NotFound");
            }
             
            Leaderboard = from r1 in _db.ELOTable 
                          join r2 in _db.Player on r1.PlayerId equals r2.KFId 
                          where (r1.TournamentId == tournamentId) && 
                             ((from p in _db.Tournament
                               where (p.Private == false ||
                                      p.Shops.Contains(User.Identity.Name)) select p).Count() > 0)
                         // orderby r1.ELOScore descending
                          select new vLeaderboard(){PlayerName = r2.Name, PlayerID = r2.KFId, TournamentId = r1.TournamentId, ELOScore = r1.ELOScore,
                                                    LoseGames = (from r in _db.History
                                                                 where (r.PlayerLoseId == r2.KFId) &&
                                                                         r.TournamentId == tournamentId
                                                                 select r).Count(),
                                                    WinGames = (from r in _db.History
                                                                 where (r.PlayerWinId == r2.KFId) &&
                                                                         r.TournamentId == tournamentId
                                                                 select r).Count()
                          };

            if(orderBy == "ELODESC")
                Leaderboard = Leaderboard.OrderByDescending(s => s.ELOScore);
            else if (orderBy == "ELOASC")
                Leaderboard = Leaderboard.OrderBy(s => s.ELOScore);
            else if(orderBy == "NAMEDESC")
                Leaderboard = Leaderboard.OrderByDescending(s => s.PlayerName);
            else if (orderBy == "NAMEASC")
                Leaderboard = Leaderboard.OrderBy(s => s.PlayerName);
            else if (orderBy == "GAMEDESC")
                Leaderboard = Leaderboard.OrderByDescending(s => s.WinGames + s.LoseGames);
            else if (orderBy == "GAMEASC")
                Leaderboard = Leaderboard.OrderBy(s => s.WinGames + s.LoseGames);
            else if (orderBy == "WINDESC")
                Leaderboard = Leaderboard.OrderByDescending(s => s.WinGames);
            else if (orderBy == "WINASC")
                Leaderboard = Leaderboard.OrderBy(s => s.WinGames);
            else if (orderBy == "LOSEDESC")
                Leaderboard = Leaderboard.OrderByDescending(s => s.LoseGames);
            else if (orderBy == "LOSEASC")
                Leaderboard = Leaderboard.OrderBy(s => s.LoseGames);
            else
                return RedirectToPage("./NotFound");
            
            return Page();
        }
    }
}