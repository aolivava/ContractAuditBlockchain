using System.Data.Entity;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ContractAuditBlockchain.Core;

namespace ContractAuditBlockchain.Domain
{
    internal class DomainInitialiser : CreateDatabaseIfNotExists<ApplicationEntities>
    {
        protected override void Seed(ApplicationEntities context)
        {
            var rm = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole, string, ApplicationUserRole>(context));

            rm.Create(new ApplicationRole(LogicConstants.Roles.Administrator));
            rm.Create(new ApplicationRole(LogicConstants.Roles.Client));
            
            var um = new ApplicationUserManager(context);
            var admin = CreateUser(um, "Admin", LogicConstants.Roles.Administrator);

            context.SaveChanges();
        }

        private ApplicationUser CreateUser(ApplicationUserManager um, string name, string roleName, string adminId = null)
        {
            string userId = Guid.NewGuid().ToString();
            adminId = adminId ?? userId;
            var user = new ApplicationUser()
            {
                UserName = name,
                Forename = name,
                Surname = name,
                Email = name + "@nowhere.com",
                AddedWhen = DateTime.UtcNow,
                AccessFailedCount = 0,
                Id = userId,
                Active = true
            };

            IdentityResult ir = um.Create(user, "Passw0rd1!");
            um.AddToRole(userId, roleName);
            return user;
        }
    }
}
