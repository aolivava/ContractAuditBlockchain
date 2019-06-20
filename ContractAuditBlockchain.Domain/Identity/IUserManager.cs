using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace ContractAuditBlockchain.Domain
{
    public interface IApplicationUserManager : IUserManager<ApplicationUser, string>
    {
    }

    public interface IUserManager<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        IClaimsIdentityFactory<TUser, TKey> ClaimsIdentityFactory { get; set; }
        TimeSpan DefaultAccountLockoutTimeSpan { get; set; }
        IIdentityMessageService EmailService { get; set; }
        int MaxFailedAccessAttemptsBeforeLockout { get; set; }
        IPasswordHasher PasswordHasher { get; set; }
        IIdentityValidator<string> PasswordValidator { get; set; }
        IIdentityMessageService SmsService { get; set; }
        bool SupportsQueryableUsers { get; }
        bool SupportsUserClaim { get; }
        bool SupportsUserEmail { get; }
        bool SupportsUserLockout { get; }
        bool SupportsUserLogin { get; }
        bool SupportsUserPassword { get; }
        bool SupportsUserPhoneNumber { get; }
        bool SupportsUserRole { get; }
        bool SupportsUserSecurityStamp { get; }
        bool SupportsUserTwoFactor { get; }
        IDictionary<string, IUserTokenProvider<TUser, TKey>> TwoFactorProviders { get; }
        bool UserLockoutEnabledByDefault { get; set; }
        IQueryable<TUser> Users { get; }
        IUserTokenProvider<TUser, TKey> UserTokenProvider { get; set; }
        IIdentityValidator<TUser> UserValidator { get; set; }

        Task<IdentityResult> AccessFailedAsync(TKey userId);
        Task<IdentityResult> AddClaimAsync(TKey userId, Claim claim);
        Task<IdentityResult> AddLoginAsync(TKey userId, UserLoginInfo login);
        Task<IdentityResult> AddPasswordAsync(TKey userId, string password);
        Task<IdentityResult> AddToRoleAsync(TKey userId, string role);
        Task<IdentityResult> AddToRolesAsync(TKey userId, params string[] roles);
        Task<IdentityResult> ChangePasswordAsync(TKey userId, string currentPassword, string newPassword);
        Task<IdentityResult> ChangePhoneNumberAsync(TKey userId, string phoneNumber, string token);
        Task<bool> CheckPasswordAsync(TUser user, string password);
        Task<IdentityResult> ConfirmEmailAsync(TKey userId, string token);
        Task<IdentityResult> CreateAsync(TUser user);
        Task<IdentityResult> CreateAsync(TUser user, string password);
        Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType);
        Task<IdentityResult> DeleteAsync(TUser user);
        void Dispose();
        Task<TUser> FindAsync(UserLoginInfo login);
        Task<TUser> FindAsync(string userName, string password);
        Task<TUser> FindByEmailAsync(string email);
        Task<TUser> FindByIdAsync(TKey userId);
        Task<TUser> FindByNameAsync(string userName);
        Task<string> GenerateChangePhoneNumberTokenAsync(TKey userId, string phoneNumber);
        Task<string> GenerateEmailConfirmationTokenAsync(TKey userId);
        Task<string> GeneratePasswordResetTokenAsync(TKey userId);
        Task<string> GenerateTwoFactorTokenAsync(TKey userId, string twoFactorProvider);
        Task<string> GenerateUserTokenAsync(string purpose, TKey userId);
        Task<int> GetAccessFailedCountAsync(TKey userId);
        Task<IList<Claim>> GetClaimsAsync(TKey userId);
        Task<string> GetEmailAsync(TKey userId);
        Task<bool> GetLockoutEnabledAsync(TKey userId);
        Task<DateTimeOffset> GetLockoutEndDateAsync(TKey userId);
        Task<IList<UserLoginInfo>> GetLoginsAsync(TKey userId);
        Task<string> GetPhoneNumberAsync(TKey userId);
        Task<IList<string>> GetRolesAsync(TKey userId);
        Task<string> GetSecurityStampAsync(TKey userId);
        Task<bool> GetTwoFactorEnabledAsync(TKey userId);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(TKey userId);
        Task<bool> HasPasswordAsync(TKey userId);
        Task<bool> IsEmailConfirmedAsync(TKey userId);
        Task<bool> IsInRoleAsync(TKey userId, string role);
        Task<bool> IsLockedOutAsync(TKey userId);
        Task<bool> IsPhoneNumberConfirmedAsync(TKey userId);
        Task<IdentityResult> NotifyTwoFactorTokenAsync(TKey userId, string twoFactorProvider, string token);
        void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<TUser, TKey> provider);
        Task<IdentityResult> RemoveClaimAsync(TKey userId, Claim claim);
        Task<IdentityResult> RemoveFromRoleAsync(TKey userId, string role);
        Task<IdentityResult> RemoveFromRolesAsync(TKey userId, params string[] roles);
        Task<IdentityResult> RemoveLoginAsync(TKey userId, UserLoginInfo login);
        Task<IdentityResult> RemovePasswordAsync(TKey userId);
        Task<IdentityResult> ResetAccessFailedCountAsync(TKey userId);
        Task<IdentityResult> ResetPasswordAsync(TKey userId, string token, string newPassword);
        Task SendEmailAsync(TKey userId, string subject, string body);
        Task SendSmsAsync(TKey userId, string message);
        Task<IdentityResult> SetEmailAsync(TKey userId, string email);
        Task<IdentityResult> SetLockoutEnabledAsync(TKey userId, bool enabled);
        Task<IdentityResult> SetLockoutEndDateAsync(TKey userId, DateTimeOffset lockoutEnd);
        Task<IdentityResult> SetPhoneNumberAsync(TKey userId, string phoneNumber);
        Task<IdentityResult> SetTwoFactorEnabledAsync(TKey userId, bool enabled);
        Task<IdentityResult> UpdateAsync(TUser user);
        Task<IdentityResult> UpdateSecurityStampAsync(TKey userId);
        Task<bool> VerifyChangePhoneNumberTokenAsync(TKey userId, string token, string phoneNumber);
        Task<bool> VerifyTwoFactorTokenAsync(TKey userId, string twoFactorProvider, string token);
        Task<bool> VerifyUserTokenAsync(TKey userId, string purpose, string token);
    }
}
