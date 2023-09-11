using System;
using System.Linq;

namespace SapODataClient
{
    /// <summary>
    /// 查询filter条件
    /// </summary>
    public class SapFilterBuilder
    {
        private string _filter;

        public SapFilterBuilder(string filter = null)
        {
            _filter = filter ?? string.Empty;
        }
                
        public string GetFilter()
        {
            return _filter;
        }
        /// <summary>
        ///  等于（eq）：使用等于操作符可以检索与指定值相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Eq(string code, string value, string condition = "and")
        {
            var filter = $"{code} eq '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }

        /// <summary>
        ///  等于（eq）空的数据：使用等于操作符可以检索与指定值相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder EqNull(string code, string condition = "and")
        {
            var filter = $"{code} eq null";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        
        /// <summary>
        ///  添加一个and条件
        /// </summary>
        /// <param name="andFilter"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder AddCompositeFilter(SapFilterBuilder andFilter, string condition = "and")
        {
            if (andFilter ==null)
            {
                return this;
            }
            var filter = andFilter.GetFilter();
            var result = string.IsNullOrEmpty(_filter) ? filter : $"({_filter}) {condition} ({filter})";
            _filter = result;
            return this;
        }
        /// <summary>
        ///  等于多个（In）：使用等于操作符可以检索与指定值相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="values"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder In(string code, string[] values, string condition = "and")
        {
            if (values.Length == 1)
            {
                return Eq(code, values[0], condition);
            }
            var temps = values.Select(x => $"'{x}'").ToArray();
            var inValue = string.Join(",", temps);
            var filter = $"{code} in {inValue}";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        ///  插入 等于（eq）：使用等于操作符可以检索与指定值相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder InsertEq(string code, string value, string condition = "and")
        {
            var filter = $"{code} eq '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                 filter+ $" {condition} "+ $"({_filter})";
            _filter = result;
            return this;
        }
        /// <summary>
        ///  插入 等于多个（eq）：使用等于操作符可以检索与指定值相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="values"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder InsertIn(string code, string[] values, string condition = "and")
        {
            if (values.Length == 1)
            {
                return InsertEq(code, values[0], condition);
            }
            var temps = values.Select(x => $"'{x}'").ToArray();
            var inValue = string.Join(",", temps);
            var filter = $"{code} in {inValue}";
            var result = string.IsNullOrEmpty(_filter)? filter :
                filter+ $" {condition} "+ $"({_filter})";
            _filter = result;
            return this;
        }
        /// <summary>
        /// 不等于（ne）：使用不等于操作符可以检索与指定值不相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Ne(string code, string value, string condition = "and")
        {
            var filter = $"{code} ne '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 不等于（ne） Null：使用不等于操作符可以检索与指定值不相等的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder NeNull(string code, string condition = "and")
        {
            var filter = $"{code} ne null";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 时间范围 大于等于（ge） start  小于等于（le） end
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Time(string key, DateTime start, DateTime end, string condition = "and")
        {
            var filter = $"{key} ge '{start:yyyy-MM-dd} and {key}' lt '{end:yyyy-MM-dd}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter + $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 范围 大于等于（ge） start  小于等于（le） end
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Range(string key, string start, string end, string condition = "and")
        {
            var filter = $"{key} ge '{start}' and {key} lt '{end}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter + $" {condition} " + filter;
            _filter = result;
            return this;
        }
        public SapFilterBuilder Range(string key, DateTime start, DateTime end, string condition = "and")
        {
            var filter = $"{key} ge datetimeoffset'{start:yyyy-MM-ddTHH:mm:ssZ}' and {key} lt datetimeoffset'{end:yyyy-MM-ddTHH:mm:ssZ}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter + $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 大于（gt）：使用大于操作符可以检索大于指定值的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Gt(string code, string value, string condition = "and")
        {
            var filter = $"{code} gt '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }

        /// <summary>
        /// 小于（lt）：使用小于操作符可以检索小于指定值的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Lt(string code, string value, string condition = "and")
        {
            var filter = $"{code} lt '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 大于等于（ge）：使用大于等于操作符可以检索大于等于指定值的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Ge(string code, string value, string condition = "and")
        {
            var filter = $"{code} ge '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 大于等于（ge）：使用大于等于操作符可以检索大于等于指定值的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dateTime"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Ge(string code, DateTime dateTime, string condition = "and")
        {
            var filter = $"{code} ge datetimeoffset'{dateTime:yyyy-MM-ddTHH:mm:ssZ}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
        /// <summary>
        /// 小于等于（le）：使用小于等于操作符可以检索小于等于指定值的数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="condition">逻辑与（and）逻辑或（or）</param>
        /// <returns></returns>
        public SapFilterBuilder Le(string code, string value, string condition = "and")
        {
            var filter = $"{code} le '{value}'";
            var result = string.IsNullOrEmpty(_filter)? filter :
                _filter +  $" {condition} " + filter;
            _filter = result;
            return this;
        }
    }
}