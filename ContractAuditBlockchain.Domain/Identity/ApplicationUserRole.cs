using Microsoft.AspNet.Identity.EntityFramework;

namespace ContractAuditBlockchain.Domain
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public int ID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
