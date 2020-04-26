using System;
using System.ComponentModel.DataAnnotations;

namespace KeyForge.Core
{
    public class Player
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int KFId { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane."), StringLength(80)]
        [Display(Name = "Nazwa gracza")]
        public string Name { get; set; }  
    }
}
