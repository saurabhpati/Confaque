using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confaque.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Account;

namespace Confaque.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ConfaqueUser> _userManager;

        public AccountController(UserManager<ConfaqueUser> userManager)
        {
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            IdentityResult result = await this._userManager.CreateAsync(new ConfaqueUser()
            {
                UserName = model.Email,
                Email = model.Email,

            }, model.Password);

            if (result.Succeeded)
            {
                return View("RegistrationConfirmation");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.TryAddModelError("error", error.Description);
            }

            return View(model);
        }
    }
}
