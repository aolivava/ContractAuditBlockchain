using ContractAuditBlockchain.ApiAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ContractAuditBlockchain.Core.LogicConstants;

namespace ContractAuditBlockchain.BusinessLogic.Models
{
    public class RentContractDataViewModel
    {
        public string ID { get; set; }
        [Display(Name = "Provider")]
        public AdminParticipantListViewModel ContractProvider { get; set; }
        [Display(Name = "Client")]
        public ClientParticipantListViewModel ContractClient { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        [Display(Name = "Duration Days")]
        public int DurationDays { get; set; }
        public RentContractStatus Status { get; set; }
        public string Content { get; set; }

        public bool CanEdit { get; set; }

        public RentContractDataViewModel()
        {
            this.ContractProvider = new AdminParticipantListViewModel();
            this.ContractClient = new ClientParticipantListViewModel();
        }
    }

    public class AmendContractViewModel
    {
        public string TransactionID { get; set; }
        public DateTime Timestamp { get; set; }
        public RentContractDataViewModel ContractData { get; set; }

        public AmendContractViewModel()
        {
            this.ContractData = new RentContractDataViewModel();
        }
    }

    public class ContractDetailsViewModel
    {
        [UIHint("RentContractData")]
        public RentContractDataViewModel Contract { get; set; }
        public IEnumerable<AmendContractViewModel> Amendments { get; set; }

        public ContractDetailsViewModel()
        {
            this.Contract = new RentContractDataViewModel();
            this.Amendments = new List<AmendContractViewModel>();
        }
    }
}