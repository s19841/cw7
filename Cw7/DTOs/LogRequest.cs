using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw7.DTOs
{
    public class LogRequest
    {
        [Required(ErrorMessage = "Enter login to log in!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Enter password to log in!")]
        public string Password { get; set; }
    }
}
