using System;
using static ContractAuditBlockchain.Core.LogicConstants;

namespace ContractAuditBlockchain.ApiAccess.Models
{
    public class TxAmendContractModel : BaseModel
    {
        public override string _Class => "UpdateContract";

        public RentContractModel contract { get; set; }
        public NewDataModel newData { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class TxAmendContractModelResponse
    {
        //"$class": "...",
        public string contract { get; set; }
        public NewDataModel newData { get; set; }
        public string transactionId { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class NewDataModel : BaseModel
    {
        public override string _Class => "UpdateContractData";

        //"$class": "...",
        public DateTime expiryDate { get; set; }
        public int durationDays { get; set; }
        public RentContractStatus status { get; set; }
        public string content { get; set; }
    }
}
