using System.Collections.Generic;
using Newtonsoft.Json;

namespace SapODataClient
{
    public abstract class SapResult
    {
        public string RequestUrl { set; get; }
        
        public string RequestBody { set; get; }
    }
    public class SapResultResponse<T>:SapResult
    {
        [JsonProperty("d")] 
        public T Data { get; set; }

    }
    public class SapResultListResponse<T> :SapResultResponse<SapResultList<T>>
    {
        public List<T> Results => Data.Results;
        public string NextUri=> Data.NextUri;
    }
    
    public class SapResultList<T>
    {
        [JsonProperty("results")]
        public List<T> Results { get; set; }
        
        [JsonProperty("__next")]
        public string NextUri { get; set; }
    }
    public class SapResultItem
    {
        [JsonProperty("key")]
        public string Key { set; get; }
        /// <summary>
        /// 状态 字符串"OK" 或 "ERROR"
        /// </summary>
        [JsonProperty("status")]
        public string Status { set; get; }
        /// <summary>
        /// 返回消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { set; get; }
        /// <summary>
        /// 返回结果
        /// </summary>
        [JsonProperty("httpCode")]
        public int HttpCode { set; get; }
        /// <summary>
        /// 数据编辑结果
        /// </summary>
        [JsonProperty("editStatus")]
        public string EditStatus { set; get; }

        public bool IsOK 
        {
            get { return this.Status == "OK"; }
        }

    }

}