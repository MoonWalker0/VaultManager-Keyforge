using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class AttachmentAssignment
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int TournamentId { get; set; }
        [Required]
        public int AttachmentId { get; set; }
    }
}
