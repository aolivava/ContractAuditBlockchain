using ContractAuditBlockchain.BusinessLogic.Contracts;
using ContractAuditBlockchain.BusinessLogic.Models;
using ContractAuditBlockchain.BusinessLogic.Participants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ContractAuditBlockchain.ClientApp.Controllers
{
    [Authorize(Roles = Core.LogicConstants.Roles.Administrator)]
    public class ParticipantsController : BaseController
    {
        private ParticipantsLogicProcessor participantsProcessor;
        private ContractsLogicProcessor contractsProcessor;

        public ParticipantsController(ParticipantsLogicProcessor participantsProcessor, ContractsLogicProcessor contractsProcessor)
        {
            this.participantsProcessor = participantsProcessor;
            this.contractsProcessor = contractsProcessor;
        }

        #region Admins
        public ActionResult Admins()
        {
            var admins = participantsProcessor.GetAllAdmins();

            var vm = admins.Select(x => new AdminParticipantListViewModel {
                ID = x.Id,
                Name = x.ApplicationUser.UserName
            }).ToList();

            PopulateAdminRoles();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAdmin(AdminParticipantCreateEditViewModel vm)
        {
            var success = ModelState.IsValid;
            var message = string.Empty;

            try
            {
                if (success)
                {
                    (success, message) = await participantsProcessor.CreateAdmin(AdminVMToApplicationUser(vm), AbsoluteResetPasswordUrl);
                }
                else
                {
                    message = string.Join("<br/>", GetModelStateErrorMessages());
                }
            }
            catch (Exception e)
            {
                success = false;
                message = $"Error creating admin user: {e.Message}";
            }

            return Json(new { success, message });
        }

        public async Task<ActionResult> DetailsForAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new HttpNotFoundResult();

            var admin = participantsProcessor.GetAdminById(id);

            if (admin == null) return new HttpNotFoundResult();

            var adminParticipantModel = participantsProcessor.AdminDomainToVM(admin);
            (var success, var message, var contratList) = await contractsProcessor.GetAllForAdmin(adminParticipantModel);

            var vm = new DetailsForAdminViewModel
            {
                AdminParticipant = new AdminParticipantCreateEditViewModel
                {
                    Login = AdminToLoginVM(admin)
                },
                ContractList = contratList ?? new List<RentContractDataViewModel>()
            };

            return View(vm);
        }

        public ActionResult EditAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new HttpNotFoundResult();

            var admin = participantsProcessor.GetAdminById(id);

            if (admin == null) return new HttpNotFoundResult();

            var vm = new AdminParticipantCreateEditViewModel
            {
                Login = AdminToLoginVM(admin)
            };

            PopulateAdminRoles();

            return PartialView("_CreateEditAdmin", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAdmin(AdminParticipantCreateEditViewModel vm)
        {
            var success = ModelState.IsValid;
            var message = string.Empty;

            try
            {
                if (success)
                {
                    (success, message) = await participantsProcessor.UpdateAdmin(AdminVMToApplicationUser(vm));
                }
                else
                {
                    message = string.Join("<br/>", GetModelStateErrorMessages());
                }
            }
            catch (Exception e)
            {
                success = false;
                message = $"Error updating admin user: {e.Message}";
            }

            return Json(new { success, message });
        }

        private UserLoginDataViewModel AdminToLoginVM(Domain.Admin admin)
        {
            return new UserLoginDataViewModel
            {
                ID = admin.Id,
                Active = admin.ApplicationUser.Active,
                Roles = admin.ApplicationUser.Roles.Select(r => r.Role.Name).ToList(),
                Forename = admin.ApplicationUser.Forename,
                Surname = admin.ApplicationUser.Surname,
                Email = admin.ApplicationUser.Email,
                CanEdit = !IsClientUser
            };
        }

        private Domain.ApplicationUser AdminVMToApplicationUser(AdminParticipantCreateEditViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ID))
            {
                return new Domain.ApplicationUser
                {
                    AddedWhen = DateTime.UtcNow,
                    Active = vm.Login.Active,
                    Forename = vm.Login.Forename,
                    Surname = vm.Login.Surname,
                    UserName = vm.Name,
                    Email = vm.Login.Email
                };
            }
            else
            {
                var existingUser = participantsProcessor.GetAdminById(vm.ID).ApplicationUser;
                existingUser.Active = vm.Login.Active;
                existingUser.Forename = vm.Login.Forename;
                existingUser.Surname = vm.Login.Surname;
                existingUser.UserName = vm.Name;
                existingUser.Email = vm.Login.Email;

                return existingUser;
            }
        }

        private void PopulateAdminRoles()
        {
            var allowedRoles = participantsProcessor.GetAllRoles().Where(x => x.Name == Core.LogicConstants.Roles.Administrator);
            ViewBag.UserRoles = new SelectList(allowedRoles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }), "Value", "Text");
        }

        #endregion Admins

        #region Clients

        public ActionResult Clients()
        {
            var clients = participantsProcessor.GetAllClients();

            var vm = clients.Select(x => new ClientParticipantListViewModel
            {
                ID = x.Id,
                Name = x.ApplicationUser.UserName
            }).ToList();

            PopulateClientRoles();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateClient(ClientParticipantCreateEditViewModel vm)
        {
            var success = ModelState.IsValid;
            var message = string.Empty;

            try
            {
                if (success)
                {
                    (success, message) = await participantsProcessor.CreateClient(ClientVMToApplicationUser(vm), AbsoluteResetPasswordUrl);
                }
                else
                {
                    message = string.Join("<br/>", GetModelStateErrorMessages());
                }
            }
            catch (Exception e)
            {
                success = false;
                message = $"Error creating client user: {e.Message}";
            }

            return Json(new { success, message });
        }

        [OverrideAuthorization]
        [Authorize(Roles = AclFull)]
        public async Task<ActionResult> DetailsForClient(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new HttpNotFoundResult();

            var client = participantsProcessor.GetClientById(id);

            if (client == null) return new HttpNotFoundResult();

            var clientParticipantModel = participantsProcessor.ClientDomainToVM(client);
            (var success, var message, var contratList) = await contractsProcessor.GetAllForClient(clientParticipantModel);

            var vm = new DetailsForClientViewModel
            {
                ClientParticipant = new ClientParticipantCreateEditViewModel
                {
                    Login = ClientToLoginVM(client)
                },
                ContractList = contratList ?? new List<RentContractDataViewModel>()
            };

            return View(vm);
        }

        public ActionResult EditClient(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new HttpNotFoundResult();

            var client = participantsProcessor.GetClientById(id);

            if (client == null) return new HttpNotFoundResult();

            var vm = new ClientParticipantCreateEditViewModel
            {
                Login = ClientToLoginVM(client)
            };

            PopulateClientRoles();

            return PartialView("_CreateEditClient", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditClient(ClientParticipantCreateEditViewModel vm)
        {
            var success = ModelState.IsValid;
            var message = string.Empty;

            try
            {
                if (success)
                {
                    (success, message) = await participantsProcessor.UpdateClient(ClientVMToApplicationUser(vm));
                }
                else
                {
                    message = string.Join("<br/>", GetModelStateErrorMessages());
                }
            }
            catch (Exception e)
            {
                success = false;
                message = $"Error updating client user: {e.Message}";
            }

            return Json(new { success, message });
        }

        private UserLoginDataViewModel ClientToLoginVM(Domain.Client client)
        {
            return new UserLoginDataViewModel
            {
                ID = client.Id,
                Active = client.ApplicationUser.Active,
                Roles = client.ApplicationUser.Roles.Select(r => r.Role.Name).ToList(),
                Forename = client.ApplicationUser.Forename,
                Surname = client.ApplicationUser.Surname,
                Email = client.ApplicationUser.Email,
                CanEdit = !IsClientUser
            };
        }

        private Domain.ApplicationUser ClientVMToApplicationUser(ClientParticipantCreateEditViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ID))
            {
                return new Domain.ApplicationUser
                {
                    AddedWhen = DateTime.UtcNow,
                    Active = vm.Login.Active,
                    Forename = vm.Login.Forename,
                    Surname = vm.Login.Surname,
                    UserName = vm.Name,
                    Email = vm.Login.Email
                };
            }
            else
            {
                var existingUser = participantsProcessor.GetClientById(vm.ID).ApplicationUser;
                existingUser.Active = vm.Login.Active;
                existingUser.Forename = vm.Login.Forename;
                existingUser.Surname = vm.Login.Surname;
                existingUser.UserName = vm.Name;
                existingUser.Email = vm.Login.Email;

                return existingUser;
            }
        }

        private void PopulateClientRoles()
        {
            var allowedRoles = participantsProcessor.GetAllRoles().Where(x => x.Name == Core.LogicConstants.Roles.Client);
            ViewBag.UserRoles = new SelectList(allowedRoles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }), "Value", "Text");
        }
        #endregion Clients

    }
}