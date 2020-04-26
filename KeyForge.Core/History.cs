using System;
using System.Collections.Generic;
using System.Text;

namespace KeyForge.Core
{
    public class History
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int PlayerWinId { get; set; }
        public int PlayerLoseId { get; set; }
        public int WinIncrease { get; set; }
        public int LoseDecrease { get; set; }
        public int WinFinalELO { get; set; }
        public int LoseFinalELO { get; set; }
        public DateTime Date { get; set; }
    }
}
