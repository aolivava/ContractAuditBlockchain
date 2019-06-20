using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ContractAuditBlockchain.ClientApp.Models;
using ContractAuditBlockchain.Core;
using ContractAuditBlockchain.Core.NLog;
using ContractAuditBlockchain.Domain;
using ContractAuditBlockchain.BusinessLogic.AccessControl;

namespace ContractAuditBlockchain.ClientApp.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private string Logger = nameof(AccountController);

        private IApplicationUserManager UserManager { get; set; }
        private IApplicationDbContext dbApplicationUser { get; set; }
        private IRepository<ApplicationUser, string> users;
        private IAccessControl accessControl;

        public AccountController(IApplicationUserManager userManager, 
                                 IApplicationDbContext dbApplicationContext,
                                 IRepository<ApplicationUser, string> users,
                                 IAccessControl accessControl)
        {
            this.UserManager = userManager;
            this.dbApplicationUser = dbApplicationContext;
            this.users = users;
            this.accessControl = accessControl;
        }

        string RememberMeCookie
        {
            get
            {
                string cookieName = "rememberMe";
#if DEBUG
                cookieName += "-" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
#endif
                return cookieName;
            }
        }

        string UserNameCookie
        {
            get
            {
                string cookieName = "username";
#if DEBUG
                cookieName += "-" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
#endif
                return cookieName;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string userName)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel vm = new LoginViewModel();
            if (!string.IsNullOrEmpty(userName))
            {
                vm.UserName = userName;
            }
            else if (Request.Cookies[UserNameCookie] != null)
            {
                vm.UserName = Request.Cookies[UserNameCookie].Value;
            }

            if (Request.Cookies[RememberMeCookie] != null)
            {
                bool val;
                if (bool.TryParse(Request.Cookies[RememberMeCookie].Value, out val))
                {
                    vm.RememberMe = val;
                }
            }


            return View("Login", vm);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AjaxLogin(LoginViewModel model, string ReturnUrl)
        {
            object redirectUrl = Url.Action("Index", "Home");
            List<string> errors = new List<string>();
            bool success = false;

            SetCookie(RememberMeCookie, true, model.RememberMe.ToString());
            SetCookie(UserNameCookie, model.RememberMe, model.UserName);

            try
            {
                SignInResult result = await PasswordSignInAsync(model.UserName, model.Password.Trim(),
                    isPersistent: false, shouldLockout: true);

                switch (result.status)
                {
                    case ApplicationSignInStatus.Success:

                        if (ReturnUrl != null)
                        {
                            // if ReturnUrl is default or Logoff, strip it off
                            var dir = new System.Web.Routing.RouteValueDictionary();
                            dir.Add("controller", "");
                            dir.Add("action", "");
                            if ((ReturnUrl.Equals(Url.RouteUrl("default", dir),
                                    StringComparison.InvariantCultureIgnoreCase))
                                || (ReturnUrl.IndexOf("Logoff", StringComparison.InvariantCultureIgnoreCase) > -1))
                            {
                                ReturnUrl = null;
                            }
                        }

                        redirectUrl = ReturnUrl ?? Url.Action("Index", "Home");
                        success = true;
                        break;
                    case ApplicationSignInStatus.Deactivated:
                        errors.Add("This user account is not active.");
                        break;
                    case ApplicationSignInStatus.LockedOut:
                        errors.Add("This user account is locked out.");
                        break;
                    case ApplicationSignInStatus.RequiresVerification:
                        errors.Add("Requires Verification.");
                        break;
                    case ApplicationSignInStatus.Failure:
                        {
                            if (result.user != null)
                            {
                                int remainingAttempts = UserManager.MaxFailedAccessAttemptsBeforeLockout -
                                                        (result.user.AccessFailedCount);
                                errors.Add(String.Format(
                                    "Invalid password for this login. After {0} more attempt{1} this account will be locked out.",
                                    remainingAttempts,
                                    (remainingAttempts > 1) ? "s" : String.Empty));
                            }
                            else
                            {
                                errors.Add("Invalid username or password.");
                            }
                        }
                        break;
                    case ApplicationSignInStatus.UnknownUser:
                        errors.Add("Invalid username or password.");
                        break;

                    default:
                        errors.Add("Unknown Sign-in status.");
                        break;
                }
            }
            catch (Exception e)
            {
                success = false;
                errors.Add($"Exception raised when trying to log in.");
                LogHelper.Exception("Account", "Exception when logging in", e);
            }


            return new JsonResult()
            {
                Data = new
                {
                    success,
                    errors,
                    redirecturl = redirectUrl
                }
            };
        }


       // GET: /Account/ForgotPassword
       [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(LoginViewModel model)
        {
            ApplicationUser user = null;
            try
            {
                user = await users.SearchFor(u => u.Email == model.Email).FirstOrDefaultAsync();
                if (user == null) { return JsonResult(false, message: "No account could be found for this email address."); }

                if (!user.Active) { return JsonResult(false, message: "The account is not currently active."); }

                if (await UserManager.IsLockedOutAsync(user.Id)) { return JsonResult(false, message: "The account is not currently active."); }

                using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await accessControl.SendResetPassword(user.Id, AbsoluteResetPasswordUrl);

                    await dbApplicationUser.SaveChangesAsync();
                    tx.Complete();
                    return JsonResult(true);
                }
            }
            catch (Exception e)
            {
                LogHelper.Exception(Logger, $"{nameof(ForgotPassword)} failed for {user?.Id ?? "unknown user"}", e);
                return JsonResult(false, message: "Unable to reset password");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    await accessControl.SendResetPassword(id, AbsoluteResetPasswordUrl);
                    await dbApplicationUser.SaveChangesAsync();
                }
                return JsonResult(true);
            }
            catch (Exception e)
            {
                LogHelper.Exception(Logger, $"{nameof(ResetPassword)} failed for {id}", e);
                return JsonResult(false, message: "Unable to reset password");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SetPassword(string id)
        {
            var success = !String.IsNullOrWhiteSpace(id);
            var userName = string.Empty;

            try
            {
                ApplicationUser user = null;

                // Find user
                if (success)
                {
                    user = await users.SearchFor(u => u.PasswordResetKey == id).SingleOrDefaultAsync();
                    success = success && user != null;
                    if (success)
                    {
                        userName = user.UserName;
                    }
                }

                // Expired key?
                success = success && (user.DatePasswordReset.HasValue && user.DatePasswordReset.Value.AddHours(5) > DateTime.UtcNow);

                // If account is locked out, and "UnlockOnReset" flag not set fail for locked out
                success = success && (user.PasswordResetIsUnlock || !(await UserManager.IsLockedOutAsync(user.Id)));

            }
            catch
            {
                success = false;
            }

            if (!success)
            {
                return new HttpNotFoundResult();
            }

            var model = new ResetPasswordViewModel
            {
                IsReset = true,
                PasswordResetKey = id,
                UserName = userName
            };
            return View("SetPassword", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> SetPassword(ResetPasswordViewModel model)
        {
            try
            {
                model.ErrorMessage = string.Empty;
                if (ModelState.IsValid)
                {
                    using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {

                        // Password == ConfirmPassword is part of model state checks...

                        var res = await accessControl.ProcessSetPassword(model.PasswordResetKey, model.Password, true, model.UserName);
                        if (res.Success)
                        {
                            tx.Complete();
                            return RedirectToAction("Login", "Account");
                        }
                        model.ErrorMessage = string.Join("<br/>", res.Errors);
                    }
                }
            }
            catch
            {
                model.ErrorMessage = "Set password failed";
            }
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            // ensure signout of the current user - DefaultAuthenticationTypes.ApplicationCookie
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            ClaimsIdentity identity =
                await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        private void SetCookie(string cookieName, bool setCookie, string value)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.HttpOnly = true;
            if (setCookie)
            {
                cookie.Value = value;
                cookie.Expires = DateTime.Now.AddMonths(1);
                cookie.Path = Request.ApplicationPath;
            }
            else
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Path = Request.ApplicationPath;
            }
            if (!Request.ApplicationPath.Equals("/"))
            {
                ClearRootCookie(cookieName);
            }
            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        }

        private void ClearRootCookie(string cookieName)
        {
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(-1)
            };
            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        }

        private enum ApplicationSignInStatus
        {
            Success,
            LockedOut,
            RequiresVerification,
            Failure,
            Deactivated,
            UnknownUser
        }

        private struct SignInResult
        {
            public ApplicationSignInStatus status;
            public ApplicationUser user;
        }

        private async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent,
            bool shouldLockout)
        {
            ApplicationSignInStatus status = ApplicationSignInStatus.Success;

            ApplicationUser user = await UserManager.FindByNameAsync(userName);

            // Unknown user or is client user (and this is the Admin site)
            if (user == null || user.ClientId != null)
            {
                status = ApplicationSignInStatus.UnknownUser;
            }
            else if (!user.Active)
            {
                status = ApplicationSignInStatus.Deactivated;
            }
            else if (user.LockoutEnabled && (user.LockoutEndDateUtc != null) &&
                     (user.LockoutEndDateUtc >= DateTimeOffset.UtcNow))
            {
                status = ApplicationSignInStatus.LockedOut;
            }
            else if (!await UserManager.CheckPasswordAsync(user, password))
            {
                status = ApplicationSignInStatus.Failure;
                if (shouldLockout)
                {
                    await UserManager.AccessFailedAsync(user.Id);
                    if (await UserManager.IsLockedOutAsync(user.Id))
                    {
                        status = ApplicationSignInStatus.LockedOut;
                    }
                }
            }
            else
            {
                if (user.LastLoginUtc.HasValue)
                {
                    HttpContext.Session["lastLoginUtc"] =
                        DateTime.SpecifyKind(user.LastLoginUtc.Value, DateTimeKind.Utc).ToString("O");
                }
                else
                {
                    HttpContext.Session["lastLoginUtc"] = "";
                }
                user.AccessFailedCount = 0;
                user.LastLoginUtc = DateTime.UtcNow;
                await UserManager.UpdateAsync(user);
                await SignInAsync(user, isPersistent);
            }

            return new SignInResult() { status = status, user = user };
        }

        #endregion
    }
}
