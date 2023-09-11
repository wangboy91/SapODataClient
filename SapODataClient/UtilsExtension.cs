using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace SapODataClient
{
    public static class UtilsExtension
    {
        /// <summary>
        /// 时间戳计时开始时间
        /// </summary>
        private static DateTime _timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static DateTime GetSapTimeStampToDateTime(this JObject data, string key)
        {
            var startDateString = data.SelectToken(key)?.ToString();
            return ToSapTimeStampToDateTime(startDateString);
        }
        public static DateTime ToSapTimeStampToDateTime(this string startDateString)
        {
            if (startDateString != null && startDateString.StartsWith("/Date"))
            {
                startDateString = startDateString.Replace("/", "").
                    Replace("Date", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("+0000","");
                var timeSpanParse = long.TryParse(startDateString, out long longTimeStamp);
                if(timeSpanParse)
                    return _timeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();
                return DateTime.MinValue;
            }
            DateTime.TryParse(startDateString, out var dateTime);
            return dateTime;
        }
    }
}