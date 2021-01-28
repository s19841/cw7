using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw7.DTOs
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "RefreshToken is mandatory to actualize JWT!")]
        public string RefreshToken { get; set; }
    }
}
