using Confaque.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Confaque.Service
{
    public class ConfaqueUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ConfaqueUser>
    {
        public ConfaqueUserClaimsPrincipalFactory(UserManager<ConfaqueUser> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ConfaqueUser user)
        {
            return await Task.Run(async () =>
            {
                ClaimsPrincipal claimsPrincipal = await base.CreateAsync(user).ConfigureAwait(false);
                ClaimsIdentity claimsIdentity = claimsPrincipal.Identities.First();
                claimsIdentity.AddClaim(new Claim("birthdate", user.BirthDate.ToString("MM/dd/yyy")));
                return claimsPrincipal;
            }).ConfigureAwait(false);
        }
    }
}
