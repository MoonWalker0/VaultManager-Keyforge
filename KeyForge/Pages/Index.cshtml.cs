using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyForge.Core;
using KeyForge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyForge.Pages
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        public IEnumerable<vHistory> History { get; set; }
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {

            History = (from r1 in _db.History
                      join r2 in _db.Player on r1.PlayerWinId equals r2.KFId
                      join r3 in _db.Player on r1.PlayerLoseId equals r3.KFId
                      join r4 in _db.Tournament on r1.TournamentId equals r4.Id
                      orderby r1.Date descending
                      select new vHistory()
                      {
                          PlayerWin = r2.Name,
                          PlayerLose = r3.Name,
                          Tournament = r4.Name,
                          WinIncrease = r1.WinIncrease,
                          LoseDecrease = r1.LoseDecrease,
                          WinFinalELO = r1.WinFinalELO,
                          LoseFinalELO = r1.LoseFinalELO,
                          Date = r1.Date
                      }).Take(10);

        }
    }
}
