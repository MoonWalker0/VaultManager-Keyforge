using AForge.Video;
using AForge.Video.DirectShow;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging; 
using System;
using System.Runtime.InteropServices;
using System.Timers;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;
using ZXing;
using System.Collections;
using ZXing.QrCode.Internal;
using ZXing.Common;
using System.Text.RegularExpressions;
using ZXing.Windows.Compatibility;
using Microsoft.AspNetCore.Identity;

namespace KeyForge
{
    public class ResultModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public string ScannedPlayer { get; set; }
        [BindProperty]
        public string ErrorMsg { get; set; } 
        [BindProperty]
        public Tournament Tournament { get; set; }
        [BindProperty]
        public Player PlayerWin { get; set; }
        [BindProperty]
        public Player PlayerLose { get; set; }
        [BindProperty]
        public Core.Result Result { get; set; }
        private readonly ApplicationDbContext _db; 
        public IEnumerable<SelectListItem> AvailablePlayers { get; set; }
        [BindProperty]
        public bool correctlyScanned { get; set; }

        public ResultModel(ApplicationDbContext db, SignInManager<IdentityUser> signInManager)
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

            ScannedPlayer = "init";
            correctlyScanned = false;                       
            PlayerWin = new Player();
            PlayerLose = new Player();

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
                                select new SelectListItem{Text = r1.Name, Value = r1.KFId.ToString()}).ToList();
                        
            return Page();
        }

        public IActionResult OnPostDefault()
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

            if (PlayerWin.Name == PlayerLose.Name && PlayerWin.KFId == PlayerLose.KFId)
            {
                ErrorMsg = "Podani gracze muszą być różni.";
                return Page();
            } 
            else if(PlayerWin.Name == null || PlayerLose.Name == null)
            {
                ErrorMsg = "Należy podać obu graczy.";
                return Page();
            }

            //Add player to DB if not already there
            bool isPlayerPresent = (from r in _db.Player
                                    where r.KFId == PlayerWin.KFId
                                    select r).Count() > 0;
            if(!isPlayerPresent)
            {
                _db.Player.Add(PlayerWin);
            }//TODO UPDATE PLAYER'S NAME ON READ 

            isPlayerPresent = (from r in _db.Player
                               where r.KFId == PlayerLose.KFId
                               select r).Count() > 0;
            if (!isPlayerPresent)
            {
                _db.Player.Add(PlayerLose);
            }

            //Add player to tournament results if not already there and give him 1000 pts
            isPlayerPresent = (from r in _db.ELOTable
                               where r.PlayerId == PlayerWin.KFId &&
                                     r.TournamentId == Tournament.Id
                               select r).Count() > 0;
            if (!isPlayerPresent)
            {
                var temp = new ELOTable()
                {
                    ELOScore = 1000,
                    PlayerId = PlayerWin.KFId,
                    TournamentId = Tournament.Id
                };
                _db.ELOTable.Add(temp);
            }

            isPlayerPresent = (from r in _db.ELOTable
                               where r.PlayerId == PlayerLose.KFId &&
                                     r.TournamentId == Tournament.Id
                               select r).Count() > 0;
            if (!isPlayerPresent)
            {
                var temp = new ELOTable()
                {
                    ELOScore = 1000,
                    PlayerId = PlayerLose.KFId,
                    TournamentId = Tournament.Id
                };
                _db.ELOTable.Add(temp);
            }
            _db.SaveChanges();

            //Calculate ELO score for given result
            ELOTable playerWinner = (from r in _db.ELOTable
                                     where r.PlayerId == PlayerWin.KFId &&
                                           r.TournamentId == Tournament.Id
                                     select r).FirstOrDefault();
            ELOTable playerLoser  = (from r in _db.ELOTable
                                     where r.PlayerId == PlayerLose.KFId &&
                                           r.TournamentId == Tournament.Id
                                     select r).FirstOrDefault();
            int scoreBeforeWinner = playerWinner.ELOScore;
            int scoreBeforeLoser = playerLoser.ELOScore;
            ELOCalculation.CalculateELO(ref playerWinner, ref playerLoser, Tournament.ELOFactor);

            //Add match to the recent action history
            var tempHistory = new History()
            {
                PlayerWinId = PlayerWin.KFId,
                PlayerLoseId = PlayerLose.KFId,
                TournamentId = Tournament.Id,
                WinIncrease = playerWinner.ELOScore - scoreBeforeWinner,
                LoseDecrease = playerLoser.ELOScore - scoreBeforeLoser,
                WinFinalELO = playerWinner.ELOScore,
                LoseFinalELO = playerLoser.ELOScore,
                Date = DateTime.Now
            };
            _db.History.Add(tempHistory);
            _db.SaveChanges();

            TempData["Message"] = "Pomyślnie dodano wynik!";
            return RedirectToPage("./ResultTournament/", new { tournamentId = Tournament.Id, orderBy = "ELODESC" });
        }

        public IActionResult OnPostQRData()
        { 
            MemoryStream stream = new MemoryStream();
            string postFrame, postScanned;

            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    var objFrame = JsonConvert.DeserializeObject<Postdata>(requestBody);
                    if (objFrame != null)
                    {
                        postFrame = objFrame.Frame;
                        postScanned = objFrame.Scanned;

                        var base64Data = Regex.Match(postFrame, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                        var binData = Convert.FromBase64String(base64Data);

                        Bitmap Image;
                        using (var imageStream = new MemoryStream(binData))
                        {
                            Image = new Bitmap(imageStream); 
                        }

                        using (Image)
                        {
                            LuminanceSource source;
                            source = new BitmapLuminanceSource(Image);
                            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                            ZXing.Result result = new MultiFormatReader().decode(bitmap);
                            if (result != null)
                            {
                                var objQR = JsonConvert.DeserializeObject<QRData>(result.Text);// "{ \"f\": \"PF Jedrzej\", \"l\": \"Peziak\", \"un\": \"AndyMaster\", \"id\": 4113648}");
                               
                                var tempPlayer = new Player();
                                tempPlayer.KFId = objQR.id;
                                tempPlayer.Name = objQR.f + " \"" + objQR.un + "\" " + objQR.l;

                                PlayerWin.KFId = objFrame.WinnerID;
                                PlayerWin.Name = objFrame.WinnerName;
                                PlayerLose.KFId = objFrame.LoserID;
                                PlayerLose.Name = objFrame.LoserName;

                                if (postScanned == "Winner")
                                { 
                                    PlayerWin = tempPlayer; 
                                }
                                else
                                {
                                    PlayerLose = tempPlayer; 
                                }

                                return new JsonResult(new { winnerName = PlayerWin.Name, winnerID = PlayerWin.KFId,
                                                            loserName = PlayerLose.Name, loserID = PlayerLose.KFId
                                });  
                            }
                        } 
                    }
                }
            }
            return Page();
        }
    }
}
