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
    public class MultiResultModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public string ErrorMsg { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; } 
        [BindProperty]
        public List<int> WinnerIDs { get; set; }
        [BindProperty]
        public List<int> LoserIDs { get; set; } 
        public IEnumerable<SelectListItem> AvailablePlayers { get; set; }
        public MultiResultModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
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
                          select r).FirstOrDefault();

            //Check if author
            if (Tournament.Originator != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            AvailablePlayers = (from r2 in _db.ELOTable
                                join r1 in _db.Player on r2.TournamentId equals tournamentId
                                where r2.PlayerId == r1.KFId 
                                orderby r1.Name ascending
                                select new SelectListItem { Text = r1.Name, Value = r1.KFId.ToString() }).ToList();
             
            return Page();
        }

        public IActionResult OnPost()
        {
            Tournament = (from r in _db.Tournament
                          where r.Id == Tournament.Id
                          select r).FirstOrDefault();

            //Check if author
            if (!_signInManager.IsSignedIn(User) || Tournament.Originator != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            AvailablePlayers = (from r2 in _db.ELOTable
                                join r1 in _db.Player on r2.TournamentId equals Tournament.Id
                                where r2.PlayerId == r1.KFId
                                orderby r1.Name ascending
                                select new SelectListItem { Text = r1.Name, Value = r1.KFId.ToString() }).ToList();

            //Verify input
            if (WinnerIDs.Count == 0 || LoserIDs.Count == 0)
            { 
                ErrorMsg = "Brak danych.";
                return Page();
            }

            if (WinnerIDs.Count != LoserIDs.Count)
            {
                ErrorMsg = "Pary zwycięzca-pokonany muszą być kompletne.";
                return Page();
            }

            for (int i = 0; i < WinnerIDs.Count; i++)
            {
                if(WinnerIDs[i] == LoserIDs[i])
                {  
                    ErrorMsg = "Podani gracze w jednej parze muszą być różni.";
                    return Page();
                }
            }

            //Start uploading
            for(int i = 0; i < WinnerIDs.Count; i++)
            {
                //Calculate ELO score for given result
                ELOTable playerWinner = (from r in _db.ELOTable
                                         where r.PlayerId == WinnerIDs[i] &&
                                               r.TournamentId == Tournament.Id
                                         select r).FirstOrDefault();
                ELOTable playerLoser = (from r in _db.ELOTable
                                        where r.PlayerId == LoserIDs[i] &&
                                              r.TournamentId == Tournament.Id
                                        select r).FirstOrDefault();
                int scoreBeforeWinner = playerWinner.ELOScore;
                int scoreBeforeLoser = playerLoser.ELOScore;
                ELOCalculation.CalculateELO(ref playerWinner, ref playerLoser, Tournament.ELOFactor);

                //Add match to the recent action history
                var tempHistory = new History()
                {
                    PlayerWinId = WinnerIDs[i],
                    PlayerLoseId = LoserIDs[i],
                    TournamentId = Tournament.Id,
                    WinIncrease = playerWinner.ELOScore - scoreBeforeWinner,
                    LoseDecrease = playerLoser.ELOScore - scoreBeforeLoser,
                    WinFinalELO = playerWinner.ELOScore,
                    LoseFinalELO = playerLoser.ELOScore,
                    Date = DateTime.Now
                };
                _db.History.Add(tempHistory);
                _db.SaveChanges();
            }
            
            TempData["Message"] = "Pomyślnie dodano wyniki!";
            return RedirectToPage("./ResultTournament/", new { tournamentId = Tournament.Id, orderBy = "ELODESC" });
        }
    }
}