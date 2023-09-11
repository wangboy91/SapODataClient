namespace SapODataClient
{
    public class AuthConfig
    {
        public string UserName { set; get; }
        public string Pwd { set; get; }
        public string BaseAddress { set; get; } = "https://api15.sapsf.cn";
    }
}