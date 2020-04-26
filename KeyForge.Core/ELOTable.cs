using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class ELOTable
    {
        public int Id { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public int TournamentId { get; set; }
        [Required]
        public int ELOScore { get; set; }
    }
}
