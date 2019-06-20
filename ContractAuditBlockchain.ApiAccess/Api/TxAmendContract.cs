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
    public class TxAmendContract : BaseApiClient
    {
        private const string Logger = nameof(TxAmendContract);
        protected override string ApiUrl => appConfig.ApiURL_TxAmendRentContract;

        public TxAmendContract(Core.Config.IApplicationConfig appConfig) : base(appConfig)
        {
        }

        public async Task<(bool, string, TxAmendContractModelResponse)> GetByID(string ID)
        {
            var response = await SendRequestAsync<TxAmendContractModelResponse>(RequestType.Get, ApiUrl + $"/{ID}");
            // a 404 NotFound status code will be returned if the entity does not exist.
            return (response.Success, response.Message, response.ResponseObj);
        }

        public async Task<(bool, string, IEnumerable<TxAmendContractModelResponse>)> GetAllByContract(RentContractModel contractData)
        {
            var success = true;
            var errorMsg = string.Empty;
            var txList = new List<TxAmendContractModelResponse>().AsEnumerable();

            try
            {
                var jsonContent = ReturnApiJsonForSearch(contractData);
                var urlEncoded = $"{ApiUrl}?filter={HttpUtility.UrlEncode(jsonContent)}";
                var response = await SendRequestAsync<IEnumerable<TxAmendContractModelResponse>>(RequestType.Get, urlEncoded);

                LogHelper.Info(Logger, $"{nameof(Create)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
                else if (response.ResponseObj?.Count() > 0)
                {
                    txList = response.ResponseObj;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(Create)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg, txList);
        }

        public async Task<(bool, string)> Create(TxAmendContractModel amendData)
        {
            var success = true;
            var errorMsg = string.Empty;

            try
            {
                var jsonContent = ReturnApiJsonForInsert(amendData);
                var response = await SendRequestAsync<TxAmendContractModelResponse>(RequestType.Post, ApiUrl, jsonContent);

                LogHelper.Info(Logger, $"{nameof(Create)} - success: {response.Success} - message: {response.Message}");

                if (!response.Success)
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(Logger, $"{nameof(Create)} Exception", ex);
                errorMsg = ex.Message;
                success = false;
            }

            return (success, errorMsg);
        }

        private string ReturnApiJsonForInsert(TxAmendContractModel amendData)
        {
            /*
                {
                  "$class": "org.example.basic.UpdateContract",
                  "contract": "resource:org.example.basic.RentContract#8eddbcb9-879f-427d-bca2-0e63ff2f22dd",
                  "newData": {
                    "$class": "org.example.basic.UpdateContractData",
                    "expiryDate": "2019-11-01T08:57:34.9228348+01:00",
                    "durationDays": 305,
                    "status": "SIGNED",
                    "content": "The content of the contract has been amended."
                  }
                }
            */

            var jsonObj = (new[] { amendData }).Select(x =>
            {
                var xObj =
                        new Newtonsoft.Json.Linq.JObject(
                                         new Newtonsoft.Json.Linq.JProperty("$class", GetFullClass(x._Class)),
                                         new Newtonsoft.Json.Linq.JProperty("contract", $"resource:{GetFullResource(x.contract._Class, x.contract.ID)}"),
                                         new Newtonsoft.Json.Linq.JProperty("newData", new Newtonsoft.Json.Linq.JObject(
                                             new Newtonsoft.Json.Linq.JProperty("$class", GetFullClass(x.newData._Class)),
                                             new Newtonsoft.Json.Linq.JProperty("expiryDate", x.newData.expiryDate.ToString("o")),
                                             new Newtonsoft.Json.Linq.JProperty("durationDays", x.newData.durationDays),
                                             new Newtonsoft.Json.Linq.JProperty("status", x.newData.status.ToString()),
                                             new Newtonsoft.Json.Linq.JProperty("content", x.newData.content)
                                         ))
                            );

                return xObj;
            }).First();

            string json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return json;
        }

        private string ReturnApiJsonForSearch(RentContractModel contractData)
        {
            /*
                {"where": {"contract": "resource:org.example.basic.RentContract#8eddbcb9-879f-427d-bca2-0e63ff2f22dd"}}
             */

            var jsonObj = (new[] { contractData }).Select(x =>
            {
                var xObj =
                        new Newtonsoft.Json.Linq.JObject(
                                         new Newtonsoft.Json.Linq.JProperty("where", new Newtonsoft.Json.Linq.JObject(
                                             new Newtonsoft.Json.Linq.JProperty("contract", $"resource:{GetFullResource(x._Class, x.ID)}")
                                         ))
                            );

                return xObj;
            }).First();

            string json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            return json;
        }

    }
}
