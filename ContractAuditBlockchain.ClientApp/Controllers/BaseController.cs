using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace ContractAuditBlockchain.ClientApp.Controllers
{
    public class BaseController : Controller
    {
        protected const string AclFull = Core.LogicConstants.Roles.Administrator
                                    + "," + Core.LogicConstants.Roles.Client;

        protected string AbsoluteUrl(string actionName, string controllerName, object routeValues)
        {
#if DEBUG
            // Can't use "{0}" directly because it will be URL encoded...
            string url = Url.Action(actionName, controllerName, routeValues, Request.Url.Scheme);
#else
            // Using Url.GetComponents to make sure the PORT is not included in the URL.
            var url = Request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.UriEscaped); //+ new UrlHelper(Request.RequestContext).Content("~");
            url += Url.Action(actionName, controllerName, routeValues);
            url = url.Replace("http://", "https://");
#endif

            return url;
        }

        // Can't use "{0}" directly because it will be URL encoded...
        protected string AbsoluteResetPasswordUrl => AbsoluteUrl("SetPassword", "Account", new { id = "--id--" }).Replace("--id--", "{0}");

        protected string[] GetModelStateErrorMessages()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();
            return errors;
        }

        public JsonResult JsonResult(bool success, object message = null, object redirectUrl = null, object warning = null)
        {
            return new JsonResult()
            {
                Data = new
                {
                    success,
                    message,
                    redirectUrl,
                    warning
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        protected bool IsClientUser => ViewBag.IsClientUser ?? false;
        protected string ClientUserID => ViewBag.ClientUserID;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.IsInRole(Core.LogicConstants.Roles.Client))
            {
                ViewBag.IsClientUser = true;
                ViewBag.ClientUserID = User.Identity.GetUserId();
            }
        }
    }
}
