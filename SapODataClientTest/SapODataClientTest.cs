using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SapODataClient;
using Xunit;

namespace SapODataClientTest
{
    public class SapODataClientTest
    {
        private readonly string userName = "xx@chintgroupT1";
        private readonly string pwd = "xxx";
        
        private readonly SapHttpClient _sapHttpClient;
        
        public SapODataClientTest()
        {
            _sapHttpClient = new SapHttpClient(userName, pwd);
        }
        
        [Fact]
        public async Task GetFOCompanyTest()
        {
            try
            {
                var list = await _sapHttpClient.GetDataAsOfDateByFilter<SapResultListResponse<JObject>>("FOCompany");
                Console.WriteLine($"获取到数据条数{list.Data.Results}");
                Assert.True(list.Data.Results.Count>0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.True(false);
            }
        }
        [Fact]
            public async Task GetFOCompanyTest2()
            {
                try
                {
                    var sapFilterBuilder = new SapFilterBuilder()
                        .In("externalCode", new[] { "70000068", "70000068" });
                    
                    var list = await _sapHttpClient.GetDataAsOfDateByFilter<SapResultListResponse<JObject>>("FOCompany",sapFilterBuilder);
                    Console.WriteLine($"获取到数据条数{list.Data.Results}");
                    Assert.True(list.Data.Results.Count>0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Assert.True(false);
                }
            }
    }
}