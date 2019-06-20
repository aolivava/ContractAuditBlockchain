using ContractAuditBlockchain.Core.NLog;
using ContractAuditBlockchain.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.BusinessLogic.AccessControl
{
    public class AccessControlLogic : IAccessControl
    {
        private const string Logger = nameof(AccessControlLogic);

        private Core.Config.IApplicationConfig settings;
        private IApplicationUserManager UsrManager;
        private IApplicationDbContext dbApplicationContext { get; set; }
        private IRepository<ApplicationUser, string> users;

        public AccessControlLogic(Core.Config.IApplicationConfig settings,
                                  IApplicationUserManager userManager, 
                                  IApplicationDbContext dbApplicationContext,
                                  IRepository<ApplicationUser, string> users)
        {
            this.UsrManager = userManager;
            this.dbApplicationContext = dbApplicationContext;
            this.users = users;
            this.settings = settings;
        }

        public async Task<SetPasswordResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            if (String.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }
            if (String.IsNullOrWhiteSpace(oldPassword)) { throw new ArgumentNullException(nameof(oldPassword)); }
            if (String.IsNullOrWhiteSpace(newPassword)) { throw new ArgumentNullException(nameof(newPassword)); }

            var identRes = await UsrManager.PasswordValidator.ValidateAsync(newPassword);
            if (!identRes.Succeeded)
            {
                var msgs = (new [] {"Password failed checks:"}).Concat(identRes.Errors).ToArray();
                return new SetPasswordResult { Success = false, Errors = msgs };
            }
            // Check not reusing current password.
            ApplicationUser user = await UsrManager.FindByIdAsync(userId);
            var reusingPassword = await UsrManager.FindAsync(user.UserName, newPassword) != null;
            if (reusingPassword)
            {
                return new SetPasswordResult { Success = false, Errors = new[] { "New password must be different than the old one." } };
            }

            identRes = await UsrManager.ChangePasswordAsync(userId, oldPassword, newPassword);
            if (!identRes.Succeeded)
            {
                var msgs = (new [] {"Password change failed:"}).Concat(identRes.Errors).ToArray();
                return new SetPasswordResult { Success = false, Errors = msgs };
            }
            user.DatePasswordReset = DateTime.UtcNow;
            await UsrManager.UpdateAsync(user);
            
            return new SetPasswordResult { Success = true };
        }


        public async Task<ApplicationUser> CreateUser(ApplicationUser user, string url, IEnumerable<string> roles)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            if (!String.IsNullOrWhiteSpace(user.Id)) { new ArgumentException($"Cannot specify {nameof(user.Id)} when creating a user", nameof(user)); }

            try
            {
                var existingUser = await UsrManager.FindByNameAsync(user.UserName);
                if (existingUser != null)
                {
                    var errorMessage = $"The username '{user.UserName}' is already taken";
                    throw new AccessControlChangeException(errorMessage, new List<string>() { errorMessage });
                }

                user.Id = Guid.NewGuid().ToString();
                user.LockoutEndDateUtc = null;

                var createUserResult = await UsrManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    throw new AccessControlChangeException("Unable to create user", createUserResult.Errors);
                }

                if (roles != null && roles.Count() > 0)
                {
                    var roleResult = await UsrManager.AddToRolesAsync(user.Id, roles.ToArray());
                    if (!roleResult.Succeeded)
                    {
                        throw new AccessControlChangeException("Unable to assign roles", roleResult.Errors);
                    }
                }

                await SendResetPassword(user.Id, url);

                return user;

            }
            catch (AccessControlChangeException e)
            {
                // This will dump the errors across lines in the log.
                var es = String.Join("\n", e.Errors);
                LogHelper.Exception(Logger, $"{nameof(CreateUser)} errors from identity:\n{es}", e);
                throw;
            }
            catch (Exception e)
            {
                LogHelper.Exception(Logger, $"{nameof(CreateUser)} failed", e);
                throw;
            }
        }

        public async Task<SetPasswordResult> ProcessSetPassword(string resetToken, string newPassword,bool isExpiration=false,string userName = null)
        {
            var identRes = await UsrManager.PasswordValidator.ValidateAsync(newPassword);
            if (!identRes.Succeeded)
            {
                var msgs = (new [] {"Password failed checks:"}).Concat(identRes.Errors).ToArray();
                return new SetPasswordResult { Success = false, Errors = msgs };
            }


            ApplicationUser user;
            if (!isExpiration)
            {
                if ((String.IsNullOrWhiteSpace(newPassword) ||
                     (user = await users.SearchFor(u => u.PasswordResetKey == resetToken).SingleOrDefaultAsync()) ==
                     null))
                {
                    return new SetPasswordResult {Success = false, Errors = new[] {"Password reset invalid"}};
                }
            }
            else
            {
                if ((String.IsNullOrWhiteSpace(newPassword) ||
                     (user = await users.SearchFor(u => u.UserName == userName).FirstOrDefaultAsync()) ==
                     null))
                {
                    return new SetPasswordResult { Success = false, Errors = new[] { "Password reset invalid" } };
                }
            }

            var reusingPassword = await UsrManager.FindAsync(user.UserName, newPassword) != null; // Check not reusing current password.
            if (reusingPassword) 
            {
                return new SetPasswordResult { Success = false, Errors = new[] { "New password must be different than the old one." } };
            }

            // If this is a reset then will unlock...
            if (user.PasswordResetIsUnlock)
            {
                user.LockoutEndDateUtc = null;
                user.PasswordResetIsUnlock = false;
            }
            else if (await UsrManager.IsLockedOutAsync(user.Id))
            {
                return new SetPasswordResult { Success = false, Errors = new[] { "Account is locked out. Please contact your administrator" } };
            }

            user.PasswordResetKey = null;
            user.DatePasswordReset = user.PasswordChangedDate = DateTime.UtcNow;
            
            await dbApplicationContext.SaveChangesAsync();
            await UsrManager.RemovePasswordAsync(user.Id);
            identRes = await UsrManager.AddPasswordAsync(user.Id, newPassword);
            if (!identRes.Succeeded)
            {
                var msgs = (new [] {"Unable to set password:"}).Concat(identRes.Errors).ToArray();
                return new SetPasswordResult { Success = false, Errors = msgs };
            }
            return new SetPasswordResult { Success = true };
        }

        public async Task SendResetPassword(string userId, string urlFormat)
        {
            if (String.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }
            if (String.IsNullOrWhiteSpace(urlFormat)) { throw new ArgumentNullException(nameof(urlFormat)); }

            var user = await users.SearchFor(u => u.Id == userId).SingleOrDefaultAsync();
            if (user == null) { throw new ArgumentException($"No such user {userId}", nameof(userId)); }

            // Use CRNG for crypto key, but could still get a collision (database is case insenstive
            // and case in base64 is significant).
            string resetKey;
            var crng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int resetKeyByteLength = 50; // To give ~30 char reset key
            var buf = new byte [resetKeyByteLength];
            int n = 0;
            do
            {
                crng.GetBytes(buf);
                // URL safe base64 without padding.
                resetKey = Convert.ToBase64String(buf).Replace("/", "-").Replace("+", "_").Replace("=", "");
                LogHelper.Trace(Logger, $"{nameof(SendResetPassword)}: reset key gen #{n++}: \"{resetKey}\"");
            }
            while (await users.SearchFor(u => u.PasswordResetKey == resetKey).AnyAsync());

            user.DatePasswordReset = DateTime.UtcNow;
            user.PasswordResetKey = resetKey;
            user.PasswordResetIsUnlock = true;

            // Save this change immediately, so email only sent if reset key etc is set.
            await dbApplicationContext.SaveChangesAsync();

            var url = String.Format(urlFormat, resetKey);
            var msgBody = $"Dear {user.FullName}, <br/> you can access this <a href='{url}'>link</a> to reset your password.";

            var emailer = new Core.Email.EmailHelper()
            {
                From = settings.EmailFromAddress,
                FromName = settings.EmailFromAddress,
                To = new string[] { user.Email },
                ToName = new string[] { user.Email },
                Subject = "Reset password",
                Body = msgBody
            };

            await emailer.SendMailAsync();
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser user, IEnumerable<string> roles)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            if (String.IsNullOrWhiteSpace(user.Id)) { throw new ArgumentException($"A user being updated must have an existing id", nameof(user)); }

            try
            {
                var existingUser = await UsrManager.FindByNameAsync(user.UserName);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    var errorMessage = $"The username '{user.UserName}' is already taken";
                    throw new AccessControlChangeException(errorMessage, new List<string>() { errorMessage });
                }

                var updtUserResult = await UsrManager.UpdateAsync(user);
                if (!updtUserResult.Succeeded)
                {
                    throw new AccessControlChangeException("Unable to update user", updtUserResult.Errors);
                }

                // Changed roles...
                var currentRoles = user.Roles.Select(r => r.Role.Name).ToArray();
                var toRemove = currentRoles.Where(cr => !roles.Contains(cr)).ToArray();
                var rolesRemoveResult = await UsrManager.RemoveFromRolesAsync(user.Id, toRemove);
                if (!rolesRemoveResult.Succeeded)
                {
                    throw new AccessControlChangeException("Unable to remove roles", rolesRemoveResult.Errors);
                }
                var toAdd = roles.Where(cr => !currentRoles.Contains(cr)).ToArray();
                var rolesAddedResult = await UsrManager.AddToRolesAsync(user.Id, toAdd);
                if (!rolesAddedResult.Succeeded)
                {
                    throw new AccessControlChangeException("Unable to add roles", rolesAddedResult.Errors);
                }
                return user;
            }
            catch (AccessControlChangeException e)
            {
                // This will dump the errors across lines in the log.
                var es = String.Join("\n", e.Errors);
                LogHelper.Exception(Logger, $"{nameof(UpdateUser)}: errors from identity:\n{es}", e);
                throw;
            }
            catch (Exception e)
            {
                LogHelper.Exception(Logger, $"{nameof(UpdateUser)} failed", e);
                throw;
            }

        }
    }

    public class SetPasswordResult
    {
        public IList<string> Errors { get; set; }
        public bool Success { get; set; }
    }
}
