using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Confaque.Data;
using Shared.Model.Manage;

namespace Confaque.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<ConfaqueUser> _userManager;

        public ManageController(UserManager<ConfaqueUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ConfaqueUser user = await this._userManager.GetUserAsync(this.User).ConfigureAwait(false);
            ManageAccountModel model = new ManageAccountModel()
            {
                Email = user.Email,
                Username = user.UserName,
                IsTwoFactorAuthEnabled = user.TwoFactorEnabled,
                IsEmailConfirmed = user.EmailConfirmed,
            };
            return View(model);
        }
    }
}