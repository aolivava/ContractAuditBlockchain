using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContractAuditBlockchain.Domain
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, IdentityUserClaim>
    {
        [MaxLength(255)]
        public string Forename { get; set; }
        [MaxLength(255)]
        public string Surname { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return Forename + " " + Surname;
            }
        }

        [Required]
        public bool Active { get; set; }
        public int? ClientId { get; set; }
        public DateTime? DatePasswordReset { get; set; }
        public DateTime? LastLoginUtc { get; set; }

        public DateTime? PasswordChangedDate { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public string MobilePhone { get; set; }
        public string PasswordResetKey { get; set; }
        /// <summary>
        /// If set, when processing password set link (from email) also
        /// unlock the account.
        /// </summary>
        public bool PasswordResetIsUnlock { get; set; }
        // Override to set attributes (rather than being varchar(max)
        [StringLength(100)]
        [Column(TypeName = "varchar")]
        public override string PhoneNumber { get { return base.PhoneNumber; }  set { base.PhoneNumber=value; } }

        public DateTime AddedWhen { get; set; }

        // ApplicationUser to Client is 0or1 to 1. Handled in fluent API
        public virtual Client Client { get; set; }
        // ApplicationUser to Admin is 0or1 to 1. Handled in fluent API
        public virtual Admin Admin { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static Expression<Func<ApplicationUser, string>> GetFullNameExpression()
        {
            return x => x.Forename + " " + x.Surname;
        }
    }
}
