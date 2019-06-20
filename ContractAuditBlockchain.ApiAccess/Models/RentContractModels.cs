using System;
using static ContractAuditBlockchain.Core.LogicConstants;

namespace ContractAuditBlockchain.ApiAccess.Models
{
    public class RentContractModel : BaseModel
    {
        public override string _Class => "RentContract";

        public AdminParticipantModel contractProvider { get; set; }
        public ClientParticipantModel contractClient { get; set; }
        public DateTime expiryDate { get; set; }
        public int durationDays { get; set; }
        public RentContractStatus status { get; set; }
        public string content { get; set; }
    }

    public class RentContractModelResponse : BaseModelResponse
    {
        //"$class": "...",
        public string contractId { get; set; }
        public string contractProvider { get; set; }
        public string contractClient { get; set; }
        public DateTime expiryDate { get; set; }
        public int durationDays { get; set; }
        public RentContractStatus status { get; set; }
        public string content { get; set; }
    }
}
