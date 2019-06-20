using ContractAuditBlockchain.ApiAccess.Models;
using ContractAuditBlockchain.BusinessLogic.Models;
using ContractAuditBlockchain.BusinessLogic.Participants;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace ContractAuditBlockchain.BusinessLogic.Contracts
{
    public class ContractsLogicProcessor
    {
        private ApiAccess.Api.RentContract bcContracts;
        private ApiAccess.Api.TxAmendContract bcContractTx;

        private Domain.IRepository<Domain.Contract, string> contractsRepo;
        private ParticipantsLogicProcessor participantsProcessor;

        public ContractsLogicProcessor(ApiAccess.Api.RentContract bcContracts, 
                                        ApiAccess.Api.TxAmendContract bcContractTx,
                                        Domain.IRepository<Domain.Contract, string> contractsRepo,
                                        ParticipantsLogicProcessor participantsProcessor)
        {
            this.bcContracts = bcContracts;
            this.bcContractTx = bcContractTx;
            this.contractsRepo = contractsRepo;
            this.participantsProcessor = participantsProcessor;
        }

        public async Task<(bool, string)> Create(RentContractDataViewModel contractVM)
        {
            var success = true;
            var msg = string.Empty;
            var contract = ContractVMToApiModel(contractVM);

            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                contract.ID = Guid.NewGuid().ToString(); 

                //  Create Domain in DB.
                var domainContract = new Domain.Contract
                {
                    Id = contract.ID,
                    ProviderId = contract.contractProvider.ID,
                    ClientId = contract.contractClient.ID
                };
                contractsRepo.Insert(domainContract);

                await contractsRepo.SaveAsync();

                //  Create Blockchain data.
                (success, msg) = await bcContracts.Create(contract);

                if (success)
                    tx.Complete();
            }
            return (success, msg);
        }

        public IQueryable<Domain.Contract> GetAll()
        {
            return contractsRepo.GetAll()
                    .Include(x => x.Provider.ApplicationUser)
                    .Include(x => x.Client.ApplicationUser);
        }

        public async Task<(bool, string, IEnumerable<RentContractDataViewModel>)> GetAllForAdmin(AdminParticipantListViewModel admin)
        {
            bool success = true;
            string message = string.Empty;
            var contractList = new List<RentContractDataViewModel>().AsEnumerable();

            if (admin != null)
            {
                IEnumerable<RentContractModelResponse> bcContractList;
                (success, message, bcContractList) = await bcContracts.GetAllForAdmin(participantsProcessor.AdminVMToApiModel(admin));

                if (success && bcContractList?.Count() > 0)
                {
                    var contractClientIDList = bcContractList.Select(x => x.GetIDFromAttributedID(x.contractClient)).ToList();
                    var contractClients = participantsProcessor.GetAllClients().Where(x => contractClientIDList.Contains(x.Id)).ToList();

                    contractList = bcContractList.Select(x => new RentContractDataViewModel
                    {
                        ID = x.contractId,
                        ContractProvider = admin,
                        ContractClient = participantsProcessor.ClientDomainToVM(contractClients.Where(cc => cc.Id == x.GetIDFromAttributedID(x.contractClient)).FirstOrDefault()),
                        ExpiryDate = x.expiryDate,
                        DurationDays = x.durationDays,
                        Status = x.status,
                        Content = x.content
                    });
                }
            }

            return (success, message, contractList);
        }

        public async Task<(bool, string, IEnumerable<RentContractDataViewModel>)> GetAllForClient(ClientParticipantListViewModel client)
        {
            bool success = true;
            string message = string.Empty;
            var contractList = new List<RentContractDataViewModel>().AsEnumerable();

            if (client != null)
            {
                IEnumerable<RentContractModelResponse> bcContractList;
                (success, message, bcContractList) = await bcContracts.GetAllForClient(participantsProcessor.ClientVMToApiModel(client));

                if (success && bcContractList?.Count() > 0)
                {
                    var contractAdminIDList = bcContractList.Select(x => x.GetIDFromAttributedID(x.contractProvider)).ToList();
                    var contractAdmins = participantsProcessor.GetAllAdmins().Where(x => contractAdminIDList.Contains(x.Id)).ToList();

                    contractList = bcContractList.Select(x => new RentContractDataViewModel
                    {
                        ID = x.contractId,
                        ContractProvider = participantsProcessor.AdminDomainToVM(contractAdmins.Where(cc => cc.Id == x.GetIDFromAttributedID(x.contractProvider)).FirstOrDefault()),
                        ContractClient = client,
                        ExpiryDate = x.expiryDate,
                        DurationDays = x.durationDays,
                        Status = x.status,
                        Content = x.content
                    });
                }
            }

            return (success, message, contractList);
        }

        public async Task<ContractDetailsViewModel> GetDetailsById(string ID, bool getAmendments, bool canEdit)
        {
            RentContractModel contract = await GetByIdAsync(ID);
            IEnumerable<AmendContractViewModel> amendments = new List<AmendContractViewModel>();

            if (contract != null && getAmendments)
            {
                (var success, var message, var txAmendments) = await bcContractTx.GetAllByContract(contract);
                if (success && txAmendments?.Count() > 0)
                {
                    amendments = txAmendments.OrderByDescending(x => x.timestamp).Select(x => new AmendContractViewModel
                    {
                        TransactionID = x.transactionId,
                        Timestamp = x.timestamp,
                        ContractData = new RentContractDataViewModel
                        {
                            ContractProvider = participantsProcessor.AdminApiModelToVM(contract.contractProvider),
                            ContractClient = participantsProcessor.ClientApiModelToVM(contract.contractClient),
                            ID = x.newData.ID,
                            ExpiryDate = x.newData.expiryDate,
                            DurationDays = x.newData.durationDays,
                            Status = x.newData.status,
                            Content = x.newData.content
                        }
                    });
                }
            }

            return new ContractDetailsViewModel
            {
                Contract = ContractApiModelToVM(contract, canEdit: canEdit),
                Amendments = amendments
            };
        }

        private async Task<RentContractModel> GetByIdAsync(string ID)
        {
            RentContractModel contract = null;
            var domainContract = contractsRepo.SearchFor(x => x.Id == ID).FirstOrDefault();

            if (domainContract != null)
            {
                (var success, var message, var contractResponse) = await bcContracts.GetById(ID);
                contract = new RentContractModel
                {
                    ID = contractResponse.contractId,
                    contractProvider = participantsProcessor.AdminDomainToApiModel(participantsProcessor.GetAdminById(contractResponse.GetIDFromAttributedID(contractResponse.contractProvider))),
                    contractClient = participantsProcessor.ClientDomainToApiModel(participantsProcessor.GetClientById(contractResponse.GetIDFromAttributedID(contractResponse.contractClient))),
                    expiryDate = contractResponse.expiryDate,
                    durationDays = contractResponse.durationDays,
                    status = contractResponse.status,
                    content = contractResponse.content
                };
            }

            return contract;
        }

        public async Task<(bool, string)> Amend(RentContractDataViewModel newContractVM)
        {
            //newData = new NewDataModel
            //{
            //    durationDays = aContract.durationDays + 10,
            //    expiryDate = aContract.expiryDate.AddDays(10),
            //    status = RentContractStatus.SIGNED,
            //    content = "The content of the contract has been amended again."
            //}
            var newContract = ContractVMToApiModel(newContractVM);

            var originalContract = await GetByIdAsync(newContract.ID);

            var aTxAmendContract = new TxAmendContractModel
            {
                contract = originalContract,
                newData = new NewDataModel {
                    ID = newContract.ID,
                    expiryDate = newContract.expiryDate,
                    durationDays = newContract.durationDays,
                    status = newContract.status,
                    content = newContract.content,
                }
            };
            (var success, var message) = await bcContractTx.Create(aTxAmendContract);

            return (success, message);
        }

        private RentContractDataViewModel ContractApiModelToVM(RentContractModel rentContract, bool canEdit)
        {
            return new RentContractDataViewModel
            {
                ID = rentContract.ID,
                ContractProvider = new AdminParticipantListViewModel
                {
                    ID = rentContract.contractProvider.ID,
                    Name = rentContract.contractProvider.Name
                },
                ContractClient = new ClientParticipantListViewModel
                {
                    ID = rentContract.contractClient.ID,
                    Name = rentContract.contractClient.Name
                },
                ExpiryDate = rentContract.expiryDate,
                DurationDays = rentContract.durationDays,
                Status = rentContract.status,
                Content = rentContract.content,

                CanEdit = canEdit
            };
        }

        private RentContractModel ContractVMToApiModel(RentContractDataViewModel vm)
        {
            return new RentContractModel
            {
                ID = vm.ID,
                contractProvider = GetAdminParticipantById(vm.ContractProvider.ID),
                contractClient = GetClientParticipantById(vm.ContractClient.ID),
                expiryDate = vm.ExpiryDate,
                durationDays = vm.DurationDays,
                status = vm.Status,
                content = vm.Content,
            };
        }

        private AdminParticipantModel GetAdminParticipantById(string ID)
        {
            return participantsProcessor.AdminDomainToApiModel(participantsProcessor.GetAdminById(ID));
        }
        private ClientParticipantModel GetClientParticipantById(string ID)
        {
            return participantsProcessor.ClientDomainToApiModel(participantsProcessor.GetClientById(ID));
        }
    }
}