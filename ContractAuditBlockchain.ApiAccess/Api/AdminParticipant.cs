using ContractAuditBlockchain.ApiAccess.Models;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.ApiAccess.Api
{
    public class AdminParticipant : BaseParticipant
    {
        private const string Logger = nameof(AdminParticipant);
        protected override string ApiUrl => appConfig.ApiURL_Admin;

        public AdminParticipant(Core.Config.IApplicationConfig appConfig) : base(appConfig)
        {
        }

        public async Task<(bool, string)> Create(AdminParticipantModel participantData)
        {
            return await base.Create(participantData);
        }

        public async Task<(bool, string)> Update(AdminParticipantModel participantData)
        {
            return await base.Update(participantData);
        }
    }
}
