using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DBO.Data.Models;

namespace DBO.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetClaimValue(this IPrincipal principal, string claimName)
        {
            var claims = principal.Identity as ClaimsIdentity;
            //var claimsValues = claims.FindAll(claimName);
            var claimValue = claims?.FindFirst(claimName)?.Value ?? string.Empty;
            
            return claimValue;
        }

        public static string GetClaimValue(this IPrincipal principal, Predicate<Claim> predicate)
        {
            var claims = principal.Identity as ClaimsIdentity;
            return claims?.FindFirst(predicate)?.Value ?? string.Empty;
        }

        public static void AddClaimsForUser(this ClaimsIdentity identity, ApplicationUser user)
        {
            var claims = identity.FindAll(x => true).ToList();
            foreach (var userClaim in GetUserClaims(user))
            {
                var old = claims.FirstOrDefault(c => c.Type == userClaim.Type);
                if (old != null)
                {
                    identity.RemoveClaim(old);
                }

                identity.AddClaim(userClaim);
            }
        }

        public static void AddClaimsForUser(this ClaimsIdentity identity, ApplicationUser user, string role)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            identity.AddClaimsForUser(user);
        }

        public static async Task AddDboClaims(this ApplicationUserManager userManager, ApplicationUser user)
        {
            var claims = await userManager.GetClaimsAsync(user.Id);
            foreach (var userClaim in GetUserClaims(user))
            {
                var old = claims.FirstOrDefault(c => c.Type == userClaim.Type);
                if (old != null)
                {
                    await userManager.RemoveClaimAsync(user.Id, old);
                }
                
                await userManager.AddClaimAsync(user.Id, userClaim);
            }
        }

        private static IEnumerable<Claim> GetUserClaims(ApplicationUser user)
        {
            yield return new Claim(Common.Constants.CompanyIdClaim, user.CompanyId?.ToString() ?? "-1");
            yield return new Claim(Common.Constants.UserIdClaim, user.Id);
            yield return new Claim(ClaimTypes.NameIdentifier, user.Id);
            yield return new Claim(ClaimTypes.Name, user.UserName);
        }
    }
}