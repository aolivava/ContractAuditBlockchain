using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ContractAuditBlockchain.Domain
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole, string>, IApplicationRoleManager
    {
        public ApplicationRoleManager(IApplicationDbContext db)
            : base(new RoleStore<ApplicationRole, string, ApplicationUserRole>(db as DbContext))
        {

        }
    }
}
