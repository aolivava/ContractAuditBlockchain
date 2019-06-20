using ContractAuditBlockchain.Core.NLog;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.ApiAccess.Api
{
    public abstract class BaseApiClient
    {
        private const string Logger = nameof(BaseApiClient);

        protected Core.Config.IApplicationConfig appConfig;
        protected abstract string ApiUrl { get; }

        protected string GetFullClass(string clss) => $"{appConfig.ApiModelNamespace}.{clss}";
        protected string GetFullResource(string clss, string ID) => $"{GetFullClass(clss)}#{ID}";

        private const int ErrorMsgMaxLength = 255;

        protected enum RequestType
        {
            Get = 1,
            Post = 2,
            Put = 3,
            Head = 4
        }

        protected class RequestResult<T>
        {
            public bool Success { get; set; }
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public T ResponseObj { get; set; }
        }

        // Restrict the constructor to child classes.
        internal BaseApiClient(Core.Config.IApplicationConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        protected async Task<bool> Exists<T>(T modelData) where T : Models.BaseModel
        {
            var response = await SendRequestAsync<string>(RequestType.Head, ApiUrl + $"/{modelData.ID}");
            // a 404 NotFound status code will be returned if the entity does not exist.
            return response.Success && (response.StatusCode == (int)System.Net.HttpStatusCode.OK);
        }

        protected async Task<RequestResult<T>> SendRequestAsync<T>(RequestType requestType, string url, string msgContent = "")
        {
            LogHelper.Info(Logger, $"{nameof(SendRequestAsync)} - requestType {requestType} - url {url} - msgContent {msgContent}");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(appConfig.ApiBaseURL);
                    httpClient.DefaultRequestHeaders.Accept.Clear();

                    var stringContent = new StringContent(msgContent, Encoding.UTF8, appConfig.ApiHeader_MediaType);
                    HttpResponseMessage response = null;
                    switch (requestType)
                    {
                        case RequestType.Get:
                            response = await httpClient.GetAsync(url);
                            break;
                        case RequestType.Post:
                            response = await httpClient.PostAsync(url, stringContent);
                            break;
                        case RequestType.Put:
                            response = await httpClient.PutAsync(url, stringContent);
                            break;
                        case RequestType.Head:
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
                            response = await httpClient.SendAsync(request);
                            break;
                    }

                    var success = response.IsSuccessStatusCode;
                    var responseContenStr = await response.Content.ReadAsStringAsync();
                    var objResult = default(T);
                    if (success)
                    {
                        // If T is built-in system type, convert result to that primitive type.
                        var ttypeNamespace = typeof(T).Namespace;
                        if (ttypeNamespace.StartsWith("System") && !ttypeNamespace.StartsWith("System.Collections"))
                        {
                            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                            objResult = (T)converter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, responseContenStr);
                        }
                        else
                        {
                            objResult = JsonConvert.DeserializeObject<T>(responseContenStr);
                        }
                    }

                    var resultMsg = GetStatusCodeMessage(response.StatusCode, response.ReasonPhrase) + (success ? string.Empty : ":" + responseContenStr);
                    LogHelper.Info(Logger, $"{nameof(SendRequestAsync)} - result {resultMsg}. response content: {responseContenStr}");
                    return new RequestResult<T> { Success = success, StatusCode = (int)response.StatusCode, Message = TruncateMessage(resultMsg), ResponseObj = objResult };
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"{nameof(SendRequestAsync)} exception";
                LogHelper.Exception(Logger, errorMsg, ex);
                return new RequestResult<T> { Success = false, StatusCode = 0, Message = TruncateMessage(errorMsg), ResponseObj = default(T) };
            }
        }

        private string TruncateMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return msg;
            return msg.Substring(0, Math.Min(msg.Length, ErrorMsgMaxLength));
        }

        private string GetStatusCodeMessage(System.Net.HttpStatusCode statusCode, string reasonPhrase)
        {
            var intCode = (int)statusCode;
            var resultMsg = statusCode.ToString();
            return $"{intCode} - {resultMsg} - {reasonPhrase}";
        }
    }
}
