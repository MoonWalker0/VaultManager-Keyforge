using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyForge
{
    public class PlayerStatsModel : PageModel
    {
        [BindProperty]
        public int tournamentHidden { get; set; }
        [BindProperty]
        public int playerHidden { get; set; }
        [BindProperty]
        public Player Player { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        [BindProperty]
        public int Wins { get; set; }
        [BindProperty]
        public int Loses { get; set; }

        private readonly ApplicationDbContext _db; 
        public PlayerStatsModel(ApplicationDbContext db)
        {
            _db = db; 
        } 
        public IActionResult OnGet(int playerId, int tournamentId)
        {
            tournamentHidden = tournamentId;
            playerHidden = playerId;

            Player = (from r in _db.Player
                      where r.KFId == playerId
                      select r).FirstOrDefault();

            Tournament = (from r in _db.Tournament
                          where r.Id == tournamentId
                          select r).FirstOrDefault();

            if (Player == null || Tournament == null)
            {
                return RedirectToPage("./NotFound");
            }

            Wins = (from r in _db.History
                    where (r.PlayerWinId == playerId) &&
                            r.TournamentId == tournamentId
                    select r).Count();

            Loses = (from r in _db.History
                    where (r.PlayerLoseId == playerId) &&
                            r.TournamentId == tournamentId
                    select r).Count();

            return Page(); 
        }
        public IActionResult OnGetData(int playerId, int tournamentId)
        { 
            var playerData = (from r in _db.History
                              where (r.PlayerWinId == playerId ||
                                      r.PlayerLoseId == playerId) &&
                                      r.TournamentId == tournamentId
                              select new { r.PlayerWinId, r.PlayerLoseId, r.Date, r.WinFinalELO, r.LoseFinalELO });

            List<int> xAxis = new List<int>();
            List<DateTime> yAxis = new List<DateTime>();

            var listPlayerData = playerData.ToList();
            for (int i = 0; i < playerData.Count(); i++)
            {
                if (listPlayerData[i].PlayerWinId == playerId)
                {
                    xAxis.Add(listPlayerData[i].WinFinalELO);
                }
                else
                {
                    xAxis.Add(listPlayerData[i].LoseFinalELO);
                }
                yAxis.Add(listPlayerData[i].Date);
            }

            return new JsonResult(new { xAxis = xAxis, yAxis = yAxis });
        }

    }
}