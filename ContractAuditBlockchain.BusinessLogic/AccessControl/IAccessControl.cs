using ContractAuditBlockchain.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.BusinessLogic.AccessControl
{
    public interface IAccessControl
    {
        Task<SetPasswordResult> ChangePassword(string userid, string oldPassword, string newPassword);

        Task<ApplicationUser> CreateUser(ApplicationUser user, string url, IEnumerable<string> roles = null);

        Task<SetPasswordResult> ProcessSetPassword(string resetToken, string newPassword, bool isReset=false, string userName = null);

        Task SendResetPassword(string userId, string urlFormat);

        Task<ApplicationUser> UpdateUser(ApplicationUser user, IEnumerable<string> roles);
    }
}
