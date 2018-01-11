using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Model.Account
{
    public class TwoFactorLoginModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "Min. 6 characters and Max. 7", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator Code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Remember this Machine")]
        public bool RememberMachine { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
