using System;
using Microsoft.AspNetCore.Identity;

namespace Confaque.Data
{
    public class ConfaqueUser : IdentityUser
    {
        public DateTime BirthDate { get; set; }
    }
}
