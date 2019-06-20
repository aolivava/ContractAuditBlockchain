using ContractAuditBlockchain.ApiAccess.Models;
using ContractAuditBlockchain.Core.NLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContractAuditBlockchain.ApiAccess.Api
{
    public class RentContract : BaseApiClient
    {
        private const string Logger = nameof(RentContract);
        protected override string ApiUrl => appConfig.ApiURL_RentContract;

        public RentContract(Core.Config.IApplicationConfig appConfig) : base(appConfig)
        {
        }

        public async Task<(bool, string)> Create(RentContractModel contractData)
        {
            return !(await Exists(contractData)) ? await Create_do(contractData) : (false, "Admin already exists");
        }

        public async Task<(bool, string, RentContractModelResponse)> GetById(string id)
        {
            var success = true;
            var errorMsg = string.Empty;
            RentContractModelResponse contract = null;

            try
            {
                var response = await SendRequestAsync<RentContractModelResponse>(RequestType.Get, ApiUrl + $"/{id}");

                LogHelper.Info(Logger, $"{nameof(GetById)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
                else
                {
                    contract = response.ResponseObj;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(GetById)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg, contract);

        }

        public async Task<(bool, string, IEnumerable<RentContractModelResponse>)> GetAllForAdmin(AdminParticipantModel adminParticipant)
        {
            var success = true;
            var errorMsg = string.Empty;
            var contractList = new List<RentContractModelResponse>().AsEnumerable();

            try
            {
                var jsonContent = ReturnApiJsonForSearchByAdmin(adminParticipant);
                var urlEncoded = $"{ApiUrl}?filter={HttpUtility.UrlEncode(jsonContent)}";
                var response = await SendRequestAsync<IEnumerable<RentContractModelResponse>>(RequestType.Get, urlEncoded);

                LogHelper.Info(Logger, $"{nameof(GetAllForAdmin)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
                else if (response.ResponseObj?.Count() > 0)
                {
                    contractList = response.ResponseObj;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(GetAllForAdmin)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg, contractList);

        }

        public async Task<(bool, string, IEnumerable<RentContractModelResponse>)> GetAllForClient(ClientParticipantModel clientParticipant)
        {
            var success = true;
            var errorMsg = string.Empty;
            var contractList = new List<RentContractModelResponse>().AsEnumerable();

            try
            {
                var jsonContent = ReturnApiJsonForSearchByClient(clientParticipant);
                var urlEncoded = $"{ApiUrl}?filter={HttpUtility.UrlEncode(jsonContent)}";
                var response = await SendRequestAsync<IEnumerable<RentContractModelResponse>>(RequestType.Get, urlEncoded);

                LogHelper.Info(Logger, $"{nameof(GetAllForClient)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
                else if (response.ResponseObj?.Count() > 0)
                {
                    contractList = response.ResponseObj;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(GetAllForClient)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg, contractList);

        }

        public async Task<(bool, string)> Update(RentContractModel contractData)
        {
            return !(await Exists(contractData)) ? await Create_do(contractData) : await Update_do(contractData);
        }

        private async Task<(bool, string)> Create_do(RentContractModel contractData)
        {
            var success = true;
            var errorMsg = string.Empty;

            try
            {
                var jsonContent = ReturnApiJsonForInsert(contractData);
                var response = await SendRequestAsync<RentContractModelResponse>(RequestType.Post, ApiUrl, jsonContent);

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

        private async Task<(bool, string)> Update_do(RentContractModel contractData)
        {
            var success = true;
            var errorMsg = string.Empty;

            try
            {
                var jsonContent = ReturnApiJsonForInsert(contractData);
                var response = await SendRequestAsync<RentContractModelResponse>(RequestType.Put, ApiUrl + $"/{contractData.ID}", jsonContent);

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

        private string ReturnApiJsonForInsert(RentContractModel contractData)
        {
            /*
                {
                  "$class": "org.example.basic.RentContract",
                  "contractId": "contractId:1",
                  "contractProvider": "resource:org.example.basic.HubAdmin#AnAdmin",
                  "contractClient": "resource:org.example.basic.HubClient#AlbertoOliva",
                  "expiryDate": "2019-09-10 12:00",
                  "durationDays": 300,
                  "status": "CREATED",
                  "content": "This is the original content of the contract held between my company and Alberto Oliva.",
                }
            */

            var jsonObj = (new[] { contractData }).Select(x =>
            {
                var xObj =
                        new Newtonsoft.Json.Linq.JObject(
                                         new Newtonsoft.Json.Linq.JProperty("$class", GetFullClass(x._Class)),
                                         //new Newtonsoft.Json.Linq.JProperty("contractId", $"contractId:{x.ID}"),
                                         new Newtonsoft.Json.Linq.JProperty("contractId", x.ID),
                                         new Newtonsoft.Json.Linq.JProperty("contractProvider", $"resource:{GetFullResource(x.contractProvider._Class, x.contractProvider.ID)}"),
                                         new Newtonsoft.Json.Linq.JProperty("contractClient", $"resource:{GetFullResource(x.contractClient._Class, x.contractClient.ID)}"),
                                         new Newtonsoft.Json.Linq.JProperty("expiryDate", x.expiryDate.ToString("o")),
                                         new Newtonsoft.Json.Linq.JProperty("durationDays", x.durationDays),
                                         new Newtonsoft.Json.Linq.JProperty("status", x.status.ToString()),
                                         new Newtonsoft.Json.Linq.JProperty("content", x.content)
                            );

                return xObj;
            }).First();

            string json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return json;
        }

        private string ReturnApiJsonForSearchByAdmin(AdminParticipantModel adminParticipant)
        {
            /*
                {"where": {"contractProvider": "resource:org.example.basic.HubAdmin#f874d863-aba7-448c-965f-f174cb80c01b"}}
             */

            var jsonObj = (new[] { adminParticipant }).Select(x =>
            {
                var xObj =
                        new Newtonsoft.Json.Linq.JObject(
                                         new Newtonsoft.Json.Linq.JProperty("where", new Newtonsoft.Json.Linq.JObject(
                                             new Newtonsoft.Json.Linq.JProperty("contractProvider", $"resource:{GetFullResource(x._Class, x.ID)}")
                                         ))
                            );

                return xObj;
            }).First();

            string json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return json;
        }

        private string ReturnApiJsonForSearchByClient(ClientParticipantModel clientParticipant)
        {
            /*
                {"where": {"contractClient": "resource:org.example.basic.HubClient#f874d863-aba7-448c-965f-f174cb80c01b"}}
             */

            var jsonObj = (new[] { clientParticipant }).Select(x =>
            {
                var xObj =
                        new Newtonsoft.Json.Linq.JObject(
                                         new Newtonsoft.Json.Linq.JProperty("where", new Newtonsoft.Json.Linq.JObject(
                                             new Newtonsoft.Json.Linq.JProperty("contractClient", $"resource:{GetFullResource(x._Class, x.ID)}")
                                         ))
                            );

                return xObj;
            }).First();

            string json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return json;
        }
    }
}
