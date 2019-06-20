using ContractAuditBlockchain.ApiAccess.Models;
using ContractAuditBlockchain.BusinessLogic.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace ContractAuditBlockchain.BusinessLogic.Participants
{
    public class ParticipantsLogicProcessor
    {
        private ApiAccess.Api.AdminParticipant bcAdminParticipants;
        private ApiAccess.Api.ClientParticipant bcClientParticipants;

        private Domain.IRepository<Domain.Admin, string> adminParticipantRepo;
        private Domain.IRepository<Domain.Client, string> clientParticipantRepo;

        private Domain.IRepository<Domain.ApplicationRole, string> roleRepo;

        private AccessControl.IAccessControl accesscontrol;

        public ParticipantsLogicProcessor(ApiAccess.Api.AdminParticipant bcAdminParticipants, 
                                          ApiAccess.Api.ClientParticipant bcClientParticipants,
                                          Domain.IRepository<Domain.Admin, string> adminParticipantRepo,
                                          Domain.IRepository<Domain.Client, string> clientParticipantRepo,
                                          Domain.IRepository<Domain.ApplicationRole, string> roleRepo,
                                          AccessControl.IAccessControl accesscontrol)
        {
            this.bcAdminParticipants = bcAdminParticipants;
            this.bcClientParticipants = bcClientParticipants;
            this.adminParticipantRepo = adminParticipantRepo;
            this.clientParticipantRepo = clientParticipantRepo;
            this.roleRepo = roleRepo;
            this.accesscontrol = accesscontrol;
        }

        public IQueryable<Domain.ApplicationRole> GetAllRoles()
        {
            return roleRepo.GetAll();
        }

        #region Admin
        public async Task<(bool, string)> CreateAdmin(Domain.ApplicationUser user, string url)
        {
            var success = true;
            var msg = string.Empty;

            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //  Create Domain in DB.
                var newUser = await accesscontrol.CreateUser(user, url, new[] { Core.LogicConstants.Roles.Administrator });
                var admin = new Domain.Admin
                {
                    ApplicationUser = newUser,
                    Id = newUser.Id
                };
                adminParticipantRepo.Insert(admin);

                await adminParticipantRepo.SaveAsync();

                //  Create Blockchain data.
                (success, msg) = await bcAdminParticipants.Create(AdminDomainToApiModel(admin));

                if (success)
                    tx.Complete();
            }
            return (success, msg);
        }

        public IQueryable<Domain.Admin> GetAllAdmins()
        {
            return adminParticipantRepo.GetAll()
                    .Include(s => s.ApplicationUser);
        }

        public Domain.Admin GetAdminById(string Id)
        {
            return adminParticipantRepo.SearchFor(x => x.Id == Id)
                    .Include(s => s.ApplicationUser)
                    .Include(s => s.ApplicationUser.Roles.Select(r => r.Role))
                    .FirstOrDefault();
        }

        public async Task<(bool, string)> UpdateAdmin(Domain.ApplicationUser admin)
        {
            var success = true;
            var msg = string.Empty;

            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //  Update Domain in DB.
                var user = await accesscontrol.UpdateUser(admin, new[] { Core.LogicConstants.Roles.Administrator });
                
                //  Create Blockchain data.
                (success, msg) = await bcAdminParticipants.Update(AdminDomainToApiModel(user.Admin));

                if (success)
                    tx.Complete();
            }
            return (success, msg);

        }

        public AdminParticipantListViewModel AdminApiModelToVM(AdminParticipantModel admin)
        {
            return new AdminParticipantListViewModel
            {
                ID = admin.ID,
                Name = admin.Name
            };
        }

        public AdminParticipantModel AdminVMToApiModel(AdminParticipantListViewModel vm)
        {
            return new AdminParticipantModel
            {
                ID = vm.ID,
                Name = vm.Name
            };
        }

        public AdminParticipantModel AdminDomainToApiModel(Domain.Admin admin)
        {
            return new AdminParticipantModel
            {
                ID = admin.Id,
                Name = admin.ApplicationUser.UserName
            };
        }

        public AdminParticipantListViewModel AdminDomainToVM(Domain.Admin admin)
        {
            return new AdminParticipantListViewModel
            {
                ID = admin.Id,
                Name = admin.ApplicationUser.UserName
            };
        }

        #endregion Admin

        #region Client

        public async Task<(bool, string)> CreateClient(Domain.ApplicationUser user, string url)
        {
            var success = true;
            var msg = string.Empty;

            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //  Create Domain in DB.
                var newUser = await accesscontrol.CreateUser(user, url, new[] { Core.LogicConstants.Roles.Client });
                var client = new Domain.Client
                {
                    ApplicationUser = newUser,
                    Id = newUser.Id
                };
                clientParticipantRepo.Insert(client);

                await clientParticipantRepo.SaveAsync();

                //  Create Blockchain data.
                (success, msg) = await bcClientParticipants.Create(ClientDomainToApiModel(client));

                if (success)
                    tx.Complete();
            }
            return (success, msg);
        }

        public IQueryable<Domain.Client> GetAllClients()
        {
            return clientParticipantRepo.GetAll()
                    .Include(s => s.ApplicationUser);
        }

        public Domain.Client GetClientById(string Id)
        {
            return clientParticipantRepo.SearchFor(x => x.Id == Id)
                    .Include(s => s.ApplicationUser)
                    .Include(s => s.ApplicationUser.Roles.Select(r => r.Role))
                    .FirstOrDefault();
        }

        public async Task<(bool, string)> UpdateClient(Domain.ApplicationUser client)
        {
            var success = true;
            var msg = string.Empty;

            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //  Update Domain in DB.
                var user = await accesscontrol.UpdateUser(client, new[] { Core.LogicConstants.Roles.Client });

                //  Create Blockchain data.
                (success, msg) = await bcClientParticipants.Update(ClientDomainToApiModel(user.Client));

                if (success)
                    tx.Complete();
            }
            return (success, msg);

        }

        public ClientParticipantListViewModel ClientApiModelToVM(ClientParticipantModel client)
        {
            return new ClientParticipantListViewModel
            {
                ID = client.ID,
                Name = client.Name
            };
        }

        public ClientParticipantModel ClientVMToApiModel(ClientParticipantListViewModel vm)
        {
            return new ClientParticipantModel
            {
                ID = vm.ID,
                Name = vm.Name
            };
        }

        public ClientParticipantModel ClientDomainToApiModel(Domain.Client client)
        {
            return new ClientParticipantModel
            {
                ID = client.Id,
                Name = client.ApplicationUser.UserName
            };
        }

        public ClientParticipantListViewModel ClientDomainToVM(Domain.Client client)
        {
            return new ClientParticipantListViewModel
            {
                ID = client.Id,
                Name = client.ApplicationUser.UserName
            };
        }
        #endregion Client
    }
}