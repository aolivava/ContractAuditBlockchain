using Microsoft.AspNet.Identity.EntityFramework;

namespace ContractAuditBlockchain.Domain
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public int ID { get; set; }
    }
}
