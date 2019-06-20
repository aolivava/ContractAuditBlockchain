namespace ContractAuditBlockchain.Core.Config
{
    public interface IApplicationConfig
    {
        string ApiModelNamespace { get; }
        string ApiBaseURL { get; }
        string ApiURL_Admin { get; }
        string ApiURL_Client { get; }
        string ApiURL_RentContract { get; }
        string ApiURL_TxAmendRentContract { get; }

        string ApiHeader_MediaType { get; }

        string EmailFromAddress { get; }
    }
}
