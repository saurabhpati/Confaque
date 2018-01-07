using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Manage
{
    public class AuthenicatorModel
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(7, ErrorMessage="Min. 6 digits and Max. 7", MinimumLength = 6)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        [ReadOnly(true)]
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}
