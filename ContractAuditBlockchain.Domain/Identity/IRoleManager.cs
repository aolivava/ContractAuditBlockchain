using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.Domain
{
    public interface IApplicationRoleManager : IRoleManager<ApplicationRole, string>
    {
    }

    public interface IRoleManager<TRole, TKey>
        where TRole : class, IRole<TKey>
        where TKey : IEquatable<TKey>
    {
        IQueryable<TRole> Roles { get; }
        IIdentityValidator<TRole> RoleValidator { get; set; }

        Task<IdentityResult> CreateAsync(TRole role);
        Task<IdentityResult> DeleteAsync(TRole role);
        void Dispose();
        Task<TRole> FindByIdAsync(TKey roleId);
        Task<TRole> FindByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> UpdateAsync(TRole role);
    }
}
