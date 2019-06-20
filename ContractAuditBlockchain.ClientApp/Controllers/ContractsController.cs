using ContractAuditBlockchain.BusinessLogic.Contracts;
using ContractAuditBlockchain.BusinessLogic.Models;
using ContractAuditBlockchain.BusinessLogic.Participants;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ContractAuditBlockchain.ClientApp.Controllers
{
    [Authorize(Roles = Core.LogicConstants.Roles.Administrator)]
    public class ContractsController : BaseController
    {
        private ParticipantsLogicProcessor participantsProcessor;
        private ContractsLogicProcessor contractsProcessor;

        public ContractsController(ParticipantsLogicProcessor participantsProcessor, ContractsLogicProcessor contractsProcessor)
        {
            this.participantsProcessor = participantsProcessor;
            this.contractsProcessor = contractsProcessor;
        }

        public ActionResult Create(string id)
        {
            var vm = new ContractDetailsViewModel();

            PopulateViewBag(vm, id);

            return PartialView("_CreateEditContract", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContractDetailsViewModel vm)
        {
            var success = ModelState.IsValid;
            var message = string.Empty;

            try
            {
                if (success)
                {
                    (success, message) = await contractsProcessor.Create(vm.Contract);
                }
                else
                {
                    message = string.Join("<br/>", GetModelStateErrorMessages());
                }
            }
            catch (Exception e)
            {
                success = false;
                message = $"Error creating contract: {e.Message}";
            }

            return Json(new { success, message });
        }

        [OverrideAuthorization]
        [Authorize(Roles = AclFull)]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new HttpNotFoundResult();

            var contractDetails = await contractsProcessor.GetDetailsById(id, getAmendments: true, canEdit: !IsClientUser);

            if (contractDetails == null || contractDetails.Contract == null) return new HttpNotFoundResult();

            return View(contractDetails);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new HttpNotFoundResult();

            var contractDetails = await contractsProcessor.GetDetailsById(id, getAmendments: false, canEdit: !IsClientUser);

            if (contractDetails == null || contractDetails.Contract == null) return new HttpNotFoundResult();

            var vm = contractDetails;

            PopulateViewBag(vm);

            return PartialView("_CreateEditContract", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ContractDetailsViewModel vm)
        {
            var success = ModelState.IsValid;
            var message = string.Empty;

            try
            {
                if (success)
                {
                    (success, message) = await contractsProcessor.Amend(vm.Contract);
                }
                else
                {
                    message = string.Join("<br/>", GetModelStateErrorMessages());
                }
            }
            catch (Exception e)
            {
                message = $"Error amending contract: {e.Message}";
            }

            return Json(new { success, message });
        }

        private void PopulateViewBag(ContractDetailsViewModel vm, string participantId = "")
        {
            var admins = participantsProcessor.GetAllAdmins().Select(x => new SelectListItem { Text = x.ApplicationUser.UserName, Value = x.Id, Selected = x.Id == participantId }).ToList();
            ViewBag.Admins = new SelectList(admins, "Value", "Text");
            if (string.IsNullOrWhiteSpace(vm.Contract.ContractProvider.ID))
                vm.Contract.ContractProvider.ID = admins.Where(x => x.Selected).Select(x => x.Value).FirstOrDefault();

            var clients = participantsProcessor.GetAllClients().Select(x => new SelectListItem { Text = x.ApplicationUser.UserName, Value = x.Id, Selected = x.Id == participantId }).ToList();
            ViewBag.Clients = new SelectList(clients, "Value", "Text");
            if (string.IsNullOrWhiteSpace(vm.Contract.ContractClient.ID))
                vm.Contract.ContractClient.ID = clients.Where(x => x.Selected).Select(x => x.Value).FirstOrDefault();

            var statuses = Enum.GetNames(typeof(Core.LogicConstants.RentContractStatus)).Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            ViewBag.Statuses = new SelectList(statuses, "Value", "Text");
        }
    }
}