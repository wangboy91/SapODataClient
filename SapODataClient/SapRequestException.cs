using System;

namespace SapODataClient
{
    public class SapRequestException: Exception
    {
        public string Url { set; get; }
        public int HttpStatusCode { set; get; }
        public SapRequestException(int statusCode, string message) :
            base(message)
        {
            HttpStatusCode = statusCode;
        }
        public SapRequestException(string url,int statusCode, string message) :
            base(message)
        {
            Url = url;
            HttpStatusCode = statusCode;
        }
        public SapRequestException(string url,int statusCode, string message,Exception ex) :
            base(message,ex)
        {
            Url = url;
            HttpStatusCode = statusCode;
        }
    }
}