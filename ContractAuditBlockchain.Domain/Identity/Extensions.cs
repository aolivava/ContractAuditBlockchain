using System.Security.Claims;
using System.Security.Principal;

namespace ContractAuditBlockchain.Domain.Identity
{
    public static class Extensions
    {
        public static string GetFromClaim(this IIdentity identity, string type)
        {
            string result = "";

            var cIdentity = identity as ClaimsIdentity;

            if (cIdentity != null)
            {
                Claim claim = cIdentity.FindFirst(type);
                if (claim != null)
                {
                    result = claim.Value;
                }
            }

            return result;
        }

        public static string GetFromClaim(this IPrincipal principal, string type)
        {
            string result = "";

            if (principal != null)
            {
                var cPrincipal = principal as ClaimsPrincipal;
                if (cPrincipal != null)
                {
                    Claim claim = cPrincipal.FindFirst(type);
                    if (claim != null)
                    {
                        result = claim.Value;
                    }
                }
            }

            return result;
        }

        public static string GetEmail(this IIdentity identity)
        {
            return identity.GetFromClaim(ClaimTypes.Email);
        }

        public static string GetEmail(this IPrincipal principal)
        {
            return principal.GetFromClaim(ClaimTypes.Email);
        }
    }
}
