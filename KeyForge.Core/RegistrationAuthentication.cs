using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class RegistrationAuthentication
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string AuthId { get; set; } 
    }
}
