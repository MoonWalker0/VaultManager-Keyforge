using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility; 

namespace KeyForge
{
    public class RegisterPlayerModel : PageModel
    {
        [BindProperty]
        public Player ScannedPlayer { get; set; }
        public IEnumerable<SelectListItem> AvailableTournaments { get; set; }
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Tournament Tournament { get; set; }
        [BindProperty]
        public string ErrorMsg { get; set; }
        public RegisterPlayerModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult OnGet()
        { 
            AvailableTournaments = (from r in _db.Tournament
                                    where
                                    //r.Shops.Contains(User.Identity.Name) &&
                                    r.DateFrom <= DateTime.UtcNow.Date &&
                                    r.DateTo >= DateTime.UtcNow.Date
                                    select new SelectListItem { Text = r.Name, Value = r.Id.ToString() }).ToList();

            ScannedPlayer = new Player();
            Tournament = new Tournament();

            if (AvailableTournaments.Count() == 0)
            {
                ErrorMsg = "Nie znaleziono aktywnych turniejów.";
                return Page();
            }

            return Page();
        }
        public IActionResult OnPostDefault()
        {
        
            AvailableTournaments = (from r in _db.Tournament
                                    where
                                    //r.Shops.Contains(User.Identity.Name) &&
                                    r.DateFrom <= DateTime.UtcNow.Date &&
                                    r.DateTo >= DateTime.UtcNow.Date
                                    select new SelectListItem { Text = r.Name, Value = r.Id.ToString() }).ToList();

            if (Tournament.Name == "" || Tournament.Name == null || ScannedPlayer.KFId == 0)
            {
                ErrorMsg = "Zeskanuj kod QR i wybierz turniej.";
                return Page();
            }

            //Add player to DB if not already there
            bool isPlayerPresent = (from r in _db.Player
                                    where r.KFId == ScannedPlayer.KFId
                                    select r).Count() > 0;
            if (!isPlayerPresent)
            {
                _db.Player.Add(ScannedPlayer);
            }
            else
            {
                var tempPlayer = (from r in _db.Player
                                 where r.KFId == ScannedPlayer.KFId
                                 select r).FirstOrDefault();

                if(tempPlayer.Name != ScannedPlayer.Name)
                {
                    tempPlayer.Name = ScannedPlayer.Name; 
                    _db.SaveChanges();
                }
            }
             
            //Add player to tournament results if not already there and give him 1000 pts
            isPlayerPresent = (from r in _db.ELOTable
                               where r.PlayerId == ScannedPlayer.KFId &&
                                     r.TournamentId == Convert.ToInt32(Tournament.Name)
                               select r).Count() > 0;
            if (!isPlayerPresent)
            {
                var temp = new ELOTable()
                {
                    ELOScore = 1000,
                    PlayerId = ScannedPlayer.KFId,
                    TournamentId = Convert.ToInt32(Tournament.Name)
                };
                _db.ELOTable.Add(temp);
            }
            else
            {
                ErrorMsg = "Ten gracz jest już zarejestrowany na ten turniej.";
                return Page();
            }
             
            _db.SaveChanges(); 

            TempData["Message"] = "Pomyślnie zarejestrowano!";
            return RedirectToPage("./ResultTournament/", new { tournamentId = Convert.ToInt32(Tournament.Name), orderBy = "ELODESC" });
             
        }

        public IActionResult OnPostQRData()
        {
            MemoryStream stream = new MemoryStream();
            string postFrame;

            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    var objFrame = JsonConvert.DeserializeObject<PostdataLesser>(requestBody);
                    if (objFrame != null)
                    {
                        postFrame = objFrame.Frame; 

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
                                
                                ScannedPlayer.KFId = objQR.id;
                                ScannedPlayer.Name = objQR.f + " \"" + objQR.un + "\" " + objQR.l;  

                                return new JsonResult(new
                                {
                                    playerName = ScannedPlayer.Name,
                                    playerID = ScannedPlayer.KFId, 
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