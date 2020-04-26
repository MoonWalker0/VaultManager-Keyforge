using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class Tournament
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Tytuł")]
        public string Name { get; set; } 
        public string Shops { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Data startu")]
        public DateTime DateFrom { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Data końca")]
        public DateTime DateTo { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Zaznacz jeżeli turniej jest prywatny. " +
                        "Inni organizatorzy mogą dołączyć do turnieju i zgłaszać wyniki jeżeli jest on publiczny.")]
        public bool Private { get; set; }
        public string Originator { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Wspołczynnik ELO (wyższy = bardziej dynamiczna tabela wyników)")]
        public int ELOFactor { get; set; }
    }
}
