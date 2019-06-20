using System.Configuration;

namespace ContractAuditBlockchain.Core.Config
{
    public class ApplicationConfig : IApplicationConfig
    {
        private const string Logger = nameof(ApplicationConfig);

        public string ApiModelNamespace => ConfigurationManager.AppSettings[nameof(ApiModelNamespace)];
        
        public string ApiBaseURL => ConfigurationManager.AppSettings[nameof(ApiBaseURL)];
        public string ApiURL_Admin => ConfigurationManager.AppSettings[nameof(ApiURL_Admin)];
        public string ApiURL_Client => ConfigurationManager.AppSettings[nameof(ApiURL_Client)];
        public string ApiURL_RentContract => ConfigurationManager.AppSettings[nameof(ApiURL_RentContract)];
        public string ApiURL_TxAmendRentContract => ConfigurationManager.AppSettings[nameof(ApiURL_TxAmendRentContract)];
        
        public string ApiHeader_MediaType => "application/json";
        public string EmailFromAddress => ConfigurationManager.AppSettings[nameof(EmailFromAddress)];
    }
}
