using ContractAuditBlockchain.ApiAccess.Models;
using ContractAuditBlockchain.Core.NLog;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.ApiAccess.Api
{
    public abstract class BaseParticipant : BaseApiClient
    {
        private const string Logger = nameof(BaseParticipant);
        
        internal BaseParticipant(Core.Config.IApplicationConfig appConfig) : base(appConfig)
        {
        }

        protected async Task<(bool, string)> Create<T>(T participantData) where T : ParticipantModel
        {
            return !(await Exists(participantData)) ? await Create_do(participantData) : (false, "Admin already exists");
        }

        protected async Task<(bool, string)> Update<T>(T participantData) where T : ParticipantModel
        {
            return !(await Exists(participantData)) ? await Create_do(participantData) : await Update_do(participantData);
        }

        private async Task<(bool, string)> Create_do(ParticipantModel participantData)
        {
            var success = true;
            var errorMsg = string.Empty;

            try
            {
                var jsonContent = ReturnApiJsonForInsert(participantData);
                var response = await SendRequestAsync<ParticipantModelResponse>(RequestType.Post, ApiUrl, jsonContent);

                LogHelper.Info(Logger, $"{nameof(Create_do)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(Create_do)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg);
        }

        private async Task<(bool, string)> Update_do(ParticipantModel participantData)
        {
            var success = true;
            var errorMsg = string.Empty;

            try
            {
                var jsonContent = ReturnApiJsonForInsert(participantData);
                var response = await SendRequestAsync<ParticipantModelResponse>(RequestType.Put, ApiUrl + $"/{participantData.ID}", jsonContent);

                LogHelper.Info(Logger, $"{nameof(Update_do)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(Create_do)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg);
        }

        private string ReturnApiJsonForInsert(ParticipantModel participantData)
        {
            /*
                {
                "$class": $"{appConfig.ApiModelNamespace}.HubAdmin/HubClient",
                "participantId": "aGuid",
                "name": "A Participant"
                }
            */

            var jsonObj = (new[] { participantData }).Select(x =>
            {
                var xObj =
                        new Newtonsoft.Json.Linq.JObject(
                                         new Newtonsoft.Json.Linq.JProperty("$class", GetFullClass(x._Class)),
                                         new Newtonsoft.Json.Linq.JProperty("participantId", x.ID),
                                         new Newtonsoft.Json.Linq.JProperty("name", x.Name)
                            );

                return xObj;
            }).First();

            string json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return json;
        }

    }
}
