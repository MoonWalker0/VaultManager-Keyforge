using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class vLeaderboard
    { 
        [Required]
        public string PlayerName { get; set; }
        [Required]
        public int PlayerID { get; set; }
        [Required]
        public int TournamentId { get; set; }
        [Required]
        public int ELOScore { get; set; } 
        [Required]
        public int WinGames { get; set; }
        [Required]
        public int LoseGames { get; set; }
    }
}
