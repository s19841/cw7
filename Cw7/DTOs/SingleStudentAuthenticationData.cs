using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw7.DTOs
{
    public class SingleStudentAuthenticationData
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SaltPasswordHash { get; set; }
        public string RefreshToken { get; set; }
        public string[] Roles { get; set; }
    }
}
