using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SapODataClient.Model;

namespace SapODataClient
{
    public class SapHttpClient
    {
        private readonly string _baseAddress;
        private readonly string _userName;
        private readonly string _pwd;
        private readonly string _authorization;
        public string BaseAddress => _baseAddress;

        public SapHttpClient(AuthConfig config)
        {
            _baseAddress = string.IsNullOrEmpty(config.BaseAddress) ? "https://api15.sapsf.cn" : config.BaseAddress;
            _userName = config.UserName;
            _pwd = config.Pwd;
            _authorization = BasicAuth(_userName, _pwd);
        }

        public SapHttpClient(string userName, string pwd, string baseAddress = "https://api15.sapsf.cn")
        {
            _baseAddress = baseAddress;
            _userName = userName;
            _pwd = pwd;
            _authorization = BasicAuth(_userName, _pwd);
        }

        private string BasicAuth(string userName, string pwd)
        {
            string mergedCredentials = $"{userName}:{pwd}";
            byte[] byteCredentials = Encoding.UTF8.GetBytes(mergedCredentials);
            return "Basic " + Convert.ToBase64String(byteCredentials);
        }

        public async Task<T> HttpRequest<T>(HttpMethod method, string url, string postData = "",
            string contentType = "application/json") where T : SapResult
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, url);
                request.Headers.Add("Authorization", _authorization);
                if (!string.IsNullOrEmpty(postData))
                {
                    var content = new StringContent(postData, Encoding.UTF8, contentType);
                    request.Content = content;
                }

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                //response.EnsureSuccessStatusCode();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new SapRequestException(url, (int)response.StatusCode, responseContent);
                }

                var result = JsonConvert.DeserializeObject<T>(responseContent);
                result.RequestUrl = url;
                result.RequestBody = postData;
                return result;
            }
        }

        /// <summary>
        /// 基础请求 根据实体编码自动和业务数据 构建Sap请求对象
        /// </summary>
        /// <param name="metadata">对应元数据</param>
        /// <param name="data">传入基础业务数据对象</param>
        /// <returns></returns>
        public async Task<T> AddOrUpdateData<T>(Dictionary<string, object> metadata, Dictionary<string, object> data)
            where T : SapResult
        {
            var url = $"{_baseAddress}/odata/v2/upsert?$format=json";
            data.Add("__metadata", metadata);
            var requestData = JsonConvert.SerializeObject(data);
            return await HttpRequest<T>(HttpMethod.Post, url, requestData);
        }

        public async Task<T> AddOrUpdateData<T>(string urlKey, Dictionary<string, object> data) where T : SapResult
        {
            var metadataJt = new Dictionary<string, object>()
            {
                { "uri", urlKey }
            };
            return await AddOrUpdateData<T>(metadataJt, data);
        }

        public async Task<T> GetData<T>(string url) where T : SapResult
        {
            if (url.IndexOf("$format=json", StringComparison.Ordinal) < 0 &&
                url.IndexOf("?", StringComparison.Ordinal) < 0)
            {
                url = $"{url}?$format=json";
            }

            return await HttpRequest<T>(HttpMethod.Get, url);
        }

        public async Task<T> GetUserByUserId<T>(string userId) where T : SapResult
        {
            var url = $"{_baseAddress}/odata/v2/User('{userId}')?$format=json";
            return await GetData<T>(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaName"></param>
        /// <param name="paramsBuilder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetDataByParams<T>(string metaName, SapParamsBuilder paramsBuilder = null)
            where T : SapResult
        {
            var url = $"{_baseAddress}/odata/v2/{metaName}?$format=json";
            if (paramsBuilder != null && !string.IsNullOrEmpty(paramsBuilder.GetParams()))
            {
                url += $"&{paramsBuilder.GetParams()}";
            }

            return await GetData<T>(url);
        }

        /// <summary>
        /// 获取当前时间asOfDate 默认现在  有效的数据
        /// </summary>
        /// <param name="metaName"></param>
        /// <param name="sapFilterBuilder"></param>
        /// <param name="asOfDate"></param>
        /// <param name="expands"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetDataAsOfDateByFilter<T>(string metaName, SapFilterBuilder sapFilterBuilder = null,
            DateTime? asOfDate = null, string[] expands = null) where T : SapResult
        {
            var paramsBuilder = new SapParamsBuilder();
            paramsBuilder.Expands(expands);
            paramsBuilder.Filter(sapFilterBuilder);
            paramsBuilder.AsOfDate(asOfDate);
            return await GetDataByParams<T>(metaName, paramsBuilder);
        }
        
        /// <summary>
        /// 分页获取数据并处理数据
        /// </summary>
        /// <param name="metaName"></param>
        /// <param name="handleData"></param>
        /// <param name="paramsBuilder"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task<string> GetDataByPageParams<TData>(string metaName, Action<List<TData>, int> handleData,
            SapParamsBuilder paramsBuilder = null) where TData : class
        {
            var url = "";
            if (handleData == null)
                return url;
            var result = await GetDataByParams<SapResultListResponse<TData>>(metaName, paramsBuilder);
            url = result.RequestUrl;
            var index = 1;
            handleData.Invoke(result.Results, index);
            var nextUrl = result.NextUri;
            while (!string.IsNullOrEmpty(nextUrl))
            {
                index++;
                result = await GetData<SapResultListResponse<TData>>(nextUrl);
                handleData.Invoke(result.Results, index);
                nextUrl = result.NextUri;
            }

            return url;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaName"></param>
        /// <param name="handleData"></param>
        /// <param name="sapFilterBuilder"></param>
        /// <param name="toDate"></param>
        /// <param name="fromDate"></param>
        /// <param name="expands"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task<string> GetDataByPageFilter<TData>(string metaName, Action<List<TData>, int> handleData,
            SapFilterBuilder sapFilterBuilder = null, DateTime? toDate = null, DateTime? fromDate = null,
            string[] expands = null) where TData : class
        {
            var paramsBuilder = new SapParamsBuilder();
            paramsBuilder.Expands(expands);
            paramsBuilder.Filter(sapFilterBuilder);
            paramsBuilder.FromDate(fromDate);
            paramsBuilder.ToDate(toDate ?? DateTime.Now);
            paramsBuilder.Paging();
            return await GetDataByPageParams<TData>(metaName, handleData, paramsBuilder);
        }

        public async Task<SapResultResponse<List<Picklist>>> GetAllPickList()
        {
            var url = $"{_baseAddress}/odata/v2/Picklist?$format=json";
            var result = await GetData<SapResultListResponse<Picklist>>(url);
            var list = new List<Picklist>();
            list.AddRange(result.Results);
            var nextUrl = result.NextUri;
            while (!string.IsNullOrEmpty(nextUrl))
            {
                result = await GetData<SapResultListResponse<Picklist>>(nextUrl);
                list.AddRange(result.Results);
                nextUrl = result.NextUri;
            }

            return new SapResultResponse<List<Picklist>>() { RequestUrl = url, Data = list };
        }

        public async Task<SapResultResponse<Picklist>> GetPickList(string picklistId)
        {
            var url = $"{_baseAddress}/odata/v2/Picklist('{picklistId}')?$format=json";
            var result = await GetData<SapResultResponse<Picklist>>(url);
            return new SapResultResponse<Picklist>() { RequestUrl = url, Data = result.Data };
        }

        public async Task<SapResultResponse<List<PicklistOption>>> GetAllPicklistOptions(Picklist picklist)
        {
            var result = await GetData<SapResultListResponse<PicklistOption>>(picklist.picklistOptions.__deferred.uri);
            var list = new List<PicklistOption>();
            list.AddRange(result.Results);
            var nextUrl = result.NextUri;
            while (!string.IsNullOrEmpty(nextUrl))
            {
                result = await GetData<SapResultListResponse<PicklistOption>>(nextUrl);
                list.AddRange(result.Results);
                nextUrl = result.NextUri;
            }

            return new SapResultResponse<List<PicklistOption>>()
                { RequestUrl = picklist.picklistOptions.__deferred.uri, Data = list };
        }

        public async Task<SapResultResponse<List<PicklistLabels>>> GetPicklistLabels(PicklistOption picklistOption)
        {
            var result =
                await GetData<SapResultListResponse<PicklistLabels>>(picklistOption.picklistLabels.__deferred.uri);
            return new SapResultResponse<List<PicklistLabels>>()
                { RequestUrl = picklistOption.picklistLabels.__deferred.uri, Data = result.Results };
        }
    }
}