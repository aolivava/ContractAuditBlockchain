using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContractAuditBlockchain.Domain
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser, string>, IApplicationUserManager
    {
        public ApplicationUserManager(IApplicationDbContext context)
            : base(GetApplicationStore(context))
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 10,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(50 * 365); // ~50 Years - effectively indefinite.
            this.MaxFailedAccessAttemptsBeforeLockout = 3;
        }

        public override async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            return await base.FindAsync(userName, password);
        }

        private static IUserStore<ApplicationUser, string> GetApplicationStore(IApplicationDbContext context)
        {
            return new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, IdentityUserClaim>(context as DbContext);
        }
        public override async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType)
        {
            ClaimsIdentity identity = await base.CreateIdentityAsync(user, authenticationType);

            if (!string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            }

            return identity;
        }
    }
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

}
