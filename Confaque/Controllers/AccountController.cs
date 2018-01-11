using Confaque.Data;
using Confaque.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Account;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Confaque.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ConfaqueUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ConfaqueUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<ConfaqueUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ConfaqueUser> signInManager,
            IEmailService emailService)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            ConfaqueUser user = new ConfaqueUser()
            {
                UserName = model.Email,
                Email = model.Email,
                BirthDate = model.BirthDate
            };

            // User manager creates the user.
            IdentityResult result = await this._userManager.CreateAsync(user, model.Password);

            // Role manager creates the organizer role if it is not created already.
            // This is needed because the role has to exist before a user can be added to the role.
            if (!await this._roleManager.RoleExistsAsync("Organizer").ConfigureAwait(false))
            {
                await this._roleManager.CreateAsync(new IdentityRole("Organizer")).ConfigureAwait(false);
            }

            // Same is done for the speaker what is done for the organizer above.
            if (!await this._roleManager.RoleExistsAsync("Speaker").ConfigureAwait(false))
            {
                await this._roleManager.CreateAsync(new IdentityRole("Speaker")).ConfigureAwait(false);
            }

            // Adds user to the role and the claim.
            await this._userManager.AddToRoleAsync(user, model.Role).ConfigureAwait(false);
            await this._userManager.AddClaimAsync(user, new Claim("technology", model.Technology)).ConfigureAwait(false);

            if (result.Succeeded)
            {
                string code = await this._userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, Request.Scheme);
                await this._emailService.SendEmailAsync(user.Email, "Confirm Account", $"Please confirm your account by clicking this link {callbackUrl}").ConfigureAwait(false);
                await this._signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);

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
                    return this.RedirectToAction(nameof(TwoFactorLogin), new { returnUrl, loginModel.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.sss");
            }

            return View(loginModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return NotFound();
            }

            ConfaqueUser user = await this._userManager.FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await this._userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
            return View(result != null && result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorLogin(bool rememberMe, string returnUrl = null)
        {
            ConfaqueUser user = await this._signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);

            if (user == null)
            {
                throw new ApplicationException("User is null");
            }

            TwoFactorLoginModel model = new TwoFactorLoginModel()
            {
                RememberMe = rememberMe
            };

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactorLogin(TwoFactorLoginModel model, string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            if (model == null)
            {
                throw new ApplicationException("Model is null");
            }

            string code = model.TwoFactorCode;
            Microsoft.AspNetCore.Identity.SignInResult result = await this._signInManager.TwoFactorAuthenticatorSignInAsync(
                model.TwoFactorCode,
                model.RememberMe,
                model.RememberMachine).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return await this.RedirectToLocal(returnUrl).ConfigureAwait(false);
            }
            else
            {
                this.ModelState.AddModelError("model.Code", "Invalid Code");
                return View();
            }
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
