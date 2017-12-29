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
        private readonly SignInManager<ConfaqueUser> _signInManager;

        public AccountController(UserManager<ConfaqueUser> userManager, SignInManager<ConfaqueUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
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

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            return await Task.Run(() =>
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result =
                    await this._signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return await this.RedirectToLocal(returnUrl).ConfigureAwait(false);
                }

                if (result.RequiresTwoFactor)
                {
                    // Do nothing for now.
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.sss");
            }

            return View(loginModel);
        }

        private async Task<IActionResult> RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return await Task.FromResult<IActionResult>(Redirect(returnUrl)).ConfigureAwait(false);
            }

            return await Task.FromResult<IActionResult>(RedirectToAction("Index", "Conference")).ConfigureAwait(false);
        }
    }
}
