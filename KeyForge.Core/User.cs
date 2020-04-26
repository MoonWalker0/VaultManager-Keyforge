using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KeyForge.Core
{
    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(80)]
        public string Login { get; set; }
        [Required, StringLength(80)]
        public string Password { get; set; }
    }
}
