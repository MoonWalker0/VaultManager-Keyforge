using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class Attachments
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nazwa pliku")]
        public string Name { get; set; }
        [Required] 
        public string Data { get; set; }
        [Display(Name = "Długi opis - widoczny w podglądzie załącznika")]
        public string Description { get; set; }
        [Display(Name = "Krótki opis (max 100 znaków) - widoczny na głównej stronie załączników")]
        [StringLength(100, ErrorMessage = "Krótki opis nie może przekraczać 100 znaków.")]
        public string ShortDescription { get; set; }
        [Required]
        public int Author { get; set; }
        [Required]
        public long Size { get; set; }

    }
}
