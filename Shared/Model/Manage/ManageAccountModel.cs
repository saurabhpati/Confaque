using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model.Manage
{
    public class ManageAccountModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsTwoFactorAuthEnabled { get; set; }
    }
}
