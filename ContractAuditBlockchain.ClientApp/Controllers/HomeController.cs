using System.Web.Mvc;

namespace ContractAuditBlockchain.ClientApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (IsClientUser)
                return RedirectToAction("DetailsForClient", "Participants", new { id = ClientUserID });

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Alberto Oliva Varela's Graduation Project.";

            return View();
        }
    }
}