using ContractAuditBlockchain.ApiAccess.Models;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.ApiAccess.Api
{
    public class ClientParticipant : BaseParticipant
    {
        private const string Logger = nameof(ClientParticipant);
        protected override string ApiUrl => appConfig.ApiURL_Client;

        public ClientParticipant(Core.Config.IApplicationConfig appConfig) : base(appConfig)
        {
        }

        public async Task<(bool, string)> Create(ClientParticipantModel participantData)
        {
            return await base.Create(participantData);
        }

        public async Task<(bool, string)> Update(ClientParticipantModel participantData)
        {
            return await base.Update(participantData);
        }
    }
}
