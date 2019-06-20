using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace ContractAuditBlockchain.Domain
{
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole() : base()
        {
            Id = Guid.NewGuid().ToString();
        }
        public ApplicationRole(string roleName) : this()
        {
            Name = roleName;
        }
    }
}
