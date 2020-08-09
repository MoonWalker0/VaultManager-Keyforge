using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KeyForge
{
    public class GEMResultModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public string ErrorMsg { get; set; }
        [BindProperty]
        public IFormFile FileUpload { get; set; }
        [BindProperty]
        public Tournament Tournament { get; set; }
        [BindProperty]
        public List<WinLose> winLoseList { get; set; } = new List<WinLose>();
        [BindProperty]
        public IDictionary<int?, string> playersToDB { get; set; } = new Dictionary<int?, string>();
        public GEMResultModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
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

            return Page();
        }
        public IActionResult OnPostFile(List<IFormFile> files)
        {
            Tournament = (from r in _db.Tournament
                          where r.Id == Tournament.Id
                          select r).FirstOrDefault();

            //Check if author
            if (!_signInManager.IsSignedIn(User) || Tournament.Originator != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            if (files.Count == 0)
            {
                ErrorMsg = "Nie załadowano pliku.";
                return Page();
            }
            else if (files.Count > 1)
            {
                ErrorMsg = "Załadowano więcej niż jeden plik.";
                return Page();
            }

            IFormFile file = files.First();

            //No more than 1 Mb
            if (file.Length > 1000000)
            {
                ErrorMsg = "Załadowano plik jest za duży, jego rozmiar nie może przekraczać 1 MB.";
                return Page();
            }

            //Check for Final Result
            if (file.FileName != "Final Results.json")
            {
                ErrorMsg = "Brak wymaganego pliku 'Final Results.json'.";
                return Page();
            }

            //Iterate through round's file           
            IDictionary<int?, int?> players = new Dictionary<int?, int?>();
            playersToDB.Clear();
            winLoseList.Clear();

            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                string fileData = reader.ReadToEnd();

                GEMModel jsonFile = new GEMModel();
                try
                {
                    jsonFile = JsonConvert.DeserializeObject<GEMModel>(fileData.ToString().Replace(":#", ""));
                }
                catch (Exception ex)
                {
                    ErrorMsg = "Niepoprawny format pliku 'Final Results.json'.";
                    return Page();
                }

                //Build list containing pairs GEM ID - KF ID
                foreach (var participant in jsonFile.data.entityGroupMap.Participant.entities)
                {
                    players.Add(participant.pk, participant.ffg_id);
                    playersToDB.Add(participant.ffg_id,
                                    participant.first_name + " \"" + participant.username + "\" " + participant.last_name);
                }

                //Build win-lose list 
                foreach (var pair in jsonFile.data.entityGroupMap.MatchParticipant.entities.GroupBy(x => x.match_pk))
                {
                    if (pair.Count() == 2)
                    {
                        var tempFirst = pair.First();
                        var tempSecond = pair.Last();
                        if (tempFirst.points_earned == 1)
                        {
                            winLoseList.Add(new WinLose()
                            {
                                winner = players[tempFirst.participant_pk],
                                loser = players[tempSecond.participant_pk]
                            });
                        }
                        else
                        {
                            winLoseList.Add(new WinLose()
                            {
                                winner = players[tempSecond.participant_pk],
                                loser = players[tempFirst.participant_pk]
                            });
                        }
                    }
                }
            }

            //Verify if each player is available in the database
            //If not - add them!
            foreach (var player in playersToDB)
            {
                //Not present in Player DB
                if ((from r in _db.Player
                     where r.KFId == player.Key
                     select r).Count() == 0)
                {
                    //Add to Player DB
                    _db.Player.Add(new Player() { KFId = player.Key ?? 0, Name = player.Value });
                    //Add to current Tournament's DB
                    _db.ELOTable.Add(new ELOTable() { PlayerId = player.Key ?? 0, ELOScore = 1000, TournamentId = Tournament.Id });
                    _db.SaveChanges();
                }
                //Not present in the Tournament DB
                else if ((from r in _db.ELOTable
                          where r.PlayerId == player.Key && r.TournamentId == Tournament.Id
                          select r).Count() == 0)
                {
                    //Add to current Tournament's DB
                    _db.ELOTable.Add(new ELOTable() { PlayerId = player.Key ?? 0, ELOScore = 1000, TournamentId = Tournament.Id });
                    _db.SaveChanges();
                }
            }

            return Page();
        }

        public IActionResult OnPostConfirm(List<WinLose> winLoseList)
        {
            Tournament = (from r in _db.Tournament
                          where r.Id == Tournament.Id
                          select r).FirstOrDefault();

            //Check if author
            if (!_signInManager.IsSignedIn(User) || Tournament.Originator != User.Identities.FirstOrDefault().Name)
            {
                return RedirectToPage("./NotFound");
            }

            //Start uploading
            foreach (var pair in winLoseList)
            {
                //Calculate ELO score for given result
                ELOTable playerWinner = (from r in _db.ELOTable
                                         where r.PlayerId == pair.winner &&
                                               r.TournamentId == Tournament.Id
                                         select r).FirstOrDefault();
                ELOTable playerLoser = (from r in _db.ELOTable
                                        where r.PlayerId == pair.loser &&
                                              r.TournamentId == Tournament.Id
                                        select r).FirstOrDefault();
                int scoreBeforeWinner = playerWinner.ELOScore;
                int scoreBeforeLoser = playerLoser.ELOScore;
                ELOCalculation.CalculateELO(ref playerWinner, ref playerLoser, Tournament.ELOFactor);

                //Add match to the recent action history
                var tempHistory = new History()
                {
                    PlayerWinId = pair.winner ?? 0,
                    PlayerLoseId = pair.loser ?? 0,
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