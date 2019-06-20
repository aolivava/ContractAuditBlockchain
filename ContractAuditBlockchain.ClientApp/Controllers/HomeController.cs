using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ContractAuditBlockchain.ClientApp.Controllers
{
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