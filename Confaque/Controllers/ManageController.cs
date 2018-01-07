using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Confaque.Data;
using Shared.Model.Manage;
using System.Text;
using System.Text.Encodings.Web;

namespace Confaque.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<ConfaqueUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(UserManager<ConfaqueUser> userManager, UrlEncoder urlEncoder)
        {
            this._userManager = userManager;
            this._urlEncoder = urlEncoder;
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> ToggleTwoFactorAuthentication()
        {
            ConfaqueUser user = await this._userManager.GetUserAsync(this.User).ConfigureAwait(false);
            user.TwoFactorEnabled = !user.TwoFactorEnabled;
            return RedirectToAction(user.TwoFactorEnabled ? "EnableAuthenticator" : "Index");
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            ConfaqueUser user = await this._userManager.GetUserAsync(this.User).ConfigureAwait(false);
            string key = await this._userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);

            if (string.IsNullOrEmpty(key))
            {
                IdentityResult result = await this._userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    key = await this._userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
                }
            }

            AuthenicatorModel model = new AuthenicatorModel()
            {
                SharedKey = this.FormatKey(key),
                AuthenticatorUri = this.GenerateQRCodeUri(user.Email, key)
            };

            return View(model);
        }

        private string FormatKey(string key)
        {
            var result = new StringBuilder();
            int currentPosition = 0;

            while (currentPosition + 4 < key.Length)
            {
                result.Append(key.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < key.Length)
            {
                result.Append(key.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQRCodeUri(string email, string key) => string.Format(
            AuthenicatorUriFormat,
            this._urlEncoder.Encode("Confaque"),
            this._urlEncoder.Encode(email),
            key);
        
    }


}