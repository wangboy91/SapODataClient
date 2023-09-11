using System;

namespace SapODataClient
{
    public enum SapPagingType
    {
        /// <summary>
        /// 快照
        /// </summary>
        Snapshot=0,
        /// <summary>
        /// 游标
        /// </summary>
        Cursor=1,
    }
    public enum SapOrderByType
    {
        /// <summary>
        /// 正序
        /// </summary>
        Asc=0,
        /// <summary>
        /// 倒序
        /// </summary>
        Desc=1,
    }
    public class SapParamsBuilder
    {
        private string _params;
        public SapParamsBuilder(string paramsString = "")
        {
            _params = paramsString ?? string.Empty;
        }
                
        public string GetParams()
        {
            return _params;
        }
        /// <summary>
        /// 在什么日期之前当前生效的数据
        /// </summary>
        /// <param name="asOfDate"></param>
        /// <returns></returns>
        public SapParamsBuilder AsOfDate(DateTime? asOfDate= null)
        {
            if (!asOfDate.HasValue)
                return this;
            var paramsString = $"$asOfDate={asOfDate.Value:yyyy-MM-dd}";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        /// <summary>
        /// 在什么日期之前的全部数据  不能和AsOfDate 混用
        /// </summary>
        /// <param name="asOfDate"></param>
        /// <returns></returns>
        public SapParamsBuilder ToDate(DateTime? asOfDate= null)
        {
            if (!asOfDate.HasValue)
                return this;
            var paramsString = $"toDate={asOfDate.Value:yyyy-MM-dd}";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        /// <summary>
        /// 在什么日期之后的全部数据  不能和AsOfDate 混用
        /// </summary>
        /// <param name="asOfDate"></param>
        /// <returns></returns>
        public SapParamsBuilder FromDate(DateTime? asOfDate= null)
        {
            if (!asOfDate.HasValue)
                return this;
            var paramsString = $"fromDate={asOfDate.Value:yyyy-MM-dd}";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        public SapParamsBuilder Filter(SapFilterBuilder sapFilterBuilder)
        {
            if(sapFilterBuilder==null)
                return this;
            var paramsString = $"$filter={sapFilterBuilder.GetFilter()}";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        /// <summary>
        /// 分页类型（部分实体 不支持游标cursor）
        /// </summary>
        /// <param name="pagingType"></param>
        /// <returns></returns>
        public SapParamsBuilder Paging(SapPagingType pagingType=SapPagingType.Snapshot)
        {
            var paramsString = pagingType==SapPagingType.Snapshot? $"paging=snapshot":$"paging=cursor";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        public SapParamsBuilder OrderBy(string[] fields,SapOrderByType orderByType=SapOrderByType.Asc)
        {
            if (fields == null || fields.Length == 0)
                return this;
            var paramsString = $"$orderby={string.Join(",",fields)}";
            paramsString = orderByType == SapOrderByType.Asc ? $"{paramsString} asc" : $"{paramsString} desc";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        public SapParamsBuilder Expands(string[] fields)
        {
            if (fields == null || fields.Length == 0)
                return this;
            var paramsString = $"$expand={string.Join(",",fields)}";
            var result = string.IsNullOrEmpty(_params)? paramsString :
                _params +  $"&" + paramsString;
            _params = result;
            return this;
        }
        
    }
}