using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class Result
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Winner { get; set; }
        [Required]
        public string Loser { get; set; } 
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int TournamentID { get; set; }
    }
}
