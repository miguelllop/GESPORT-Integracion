using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Test
{
    [TestClass]
    public class GesportPCSBlockchainApiTest
    {
        string UrlHealth = "http://gesport4offchain.belikesoftware.com:8080/public/health";
        string UrlApiBlockchain = "http://gesport4offchain.belikesoftware.com:8080";
        string UrlAuth = "http://gesport4offchain.belikesoftware.com:8080";
        IOptions<Configuration.Configuration> config;

        [TestMethod]
        public void HealthTest()
        {
            HttpClient client = new HttpClient();
            var resp = client.GetAsync(UrlHealth);
            Assert.IsTrue(resp.Result.IsSuccessStatusCode);
            Assert.IsTrue(HealthService.TestHealth(client, config).Result);
        }

        [TestMethod]
        public void AuthWithValidCredentialsTest()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(config.Value.AuthEndpoint);
            var authSvc = new AuthService(client, config);
            var token = authSvc.GetToken().Result;
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
        }

        [TestMethod]
        public void AuthWithWrongCredentialsTest()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(config.Value.AuthEndpoint);
            config.Value.Client = "WrongClientName";
            var authSvc = new AuthService(client, config);
            var token = authSvc.GetToken().Result;
            Assert.IsNull(token);
        }

        [TestMethod]
        public void CheckTokenTest()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(UrlAuth);

            var authSvc = new AuthService(client, config);
            var token = authSvc.GetToken().Result;
            var resp = authSvc.CheckToken(token).Result;
            Assert.IsTrue(resp);
        }

        [TestMethod]
        public void AddVesselPortCallSwaggerApiTest()
        {
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri(config.Value.BlockchainEndpoint);

            RetryPolicy _httpRequestPolicy;
            var retContext = new Polly.Context();
            retContext.Add("client", clientApi);
            _httpRequestPolicy = Policy.Handle<Exception>(
                    ex => ex.InnerException is SwaggerException && ((SwaggerException)ex.InnerException).StatusCode == (int)HttpStatusCode.Unauthorized)
                .Retry(2, onRetry: (resp, retry, context) =>
                {
                    var token = GetToken();
                    var cli = context["client"] as HttpClient;
                    if (cli != null && !string.IsNullOrEmpty(token))
                        cli.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                });
            VesselPortCallDeclaration vesselPortCallDec = GetVesselPortCallDeclarationMock();
            IBlockchainClient blcApi = new BlockchainClient(clientApi);
            var res = _httpRequestPolicy.Execute(ctx => blcApi.VesselPortCallServiceAsync(vesselPortCallDec).Result, retContext);
            Assert.IsTrue(res.StatusCode == (int)HttpStatusCode.OK);
            var sr = new StreamReader(res.Stream, Encoding.UTF8);
            var conten = sr.ReadToEnd();
            Assert.IsFalse(string.IsNullOrEmpty(conten));
        }

        [TestMethod]
        public void AddVesselPortCallPollyRetryTest()
        {
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri(config.Value.BlockchainEndpoint);

            RetryPolicy<HttpResponseMessage> _httpRequestPolicy;
            var retContext = new Polly.Context();
            retContext.Add("client", clientApi);
            _httpRequestPolicy = Policy.HandleResult<HttpResponseMessage>(
                    r => r.StatusCode == HttpStatusCode.Unauthorized)
                .Retry(1, onRetry: (resp, retry, context) =>
                {
                    if (resp.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var token = GetToken();
                        var cli = context["client"] as HttpClient;
                        if (cli != null && !string.IsNullOrEmpty(token))
                            cli.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                });
            VesselPortCallDeclaration vesselPortCallDec = GetVesselPortCallDeclarationMock();
            var res = _httpRequestPolicy.Execute(ctx => clientApi.PostAsJsonAsync<VesselPortCallDeclaration>("/api/VesselPortCallServiceCopy", vesselPortCallDec).Result, retContext);
            if (!res.IsSuccessStatusCode)
                Assert.Fail();
        }

        [TestMethod]
        public void AddVesselPortCallSinPollyTest()
        {
            HttpResponseMessage res = null;
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri(config.Value.BlockchainEndpoint);
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                var vesselPortCallDec = GetVesselPortCallDeclarationMock();
                string vpcdStr = vesselPortCallDec.ToJson();
                clientApi.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                res = clientApi.PostAsJsonAsync<VesselPortCallDeclaration>("api/VesselPortCallService", vesselPortCallDec).Result;
            }
            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsSuccessStatusCode);
        }

        #region Metodos BillOfLading
        [TestMethod]
        public void AddBillOfLadingSwaggerApiTest()
        {
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri(config.Value.BlockchainEndpoint);

            RetryPolicy _httpRequestPolicy;
            var retContext = new Polly.Context();
            retContext.Add("client", clientApi);
            _httpRequestPolicy = Policy.Handle<Exception>(
                    ex => ex.InnerException is SwaggerException && ((SwaggerException)ex.InnerException).StatusCode == (int)HttpStatusCode.Unauthorized)
                .Retry(2, onRetry: (resp, retry, context) =>
                {
                    var token = GetToken();
                    var cli = context["client"] as HttpClient;
                    if (cli != null && !string.IsNullOrEmpty(token))
                        cli.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                });
            //BillOfLadingDeclaration bld = GetBillOfLadingDeclaration();
            string blstr = File.ReadAllText("./SampleFiles/BillOfLadingDeclaration.json");
            BillOfLadingDeclaration bld = Newtonsoft.Json.JsonConvert.DeserializeObject<BillOfLadingDeclaration>(blstr);
            IBlockchainClient blcApi = new BlockchainClient(clientApi);
            var res = _httpRequestPolicy.Execute(ctx => blcApi.BillOfLadingServiceAsync(bld).Result, retContext);
            Assert.IsTrue(res.StatusCode == (int)HttpStatusCode.OK);
            var sr = new StreamReader(res.Stream, Encoding.UTF8);
            var conten = sr.ReadToEnd();
            Assert.IsFalse(string.IsNullOrEmpty(conten));
        }

        private static BillOfLadingDeclaration GetBillOfLadingDeclaration()
        {
            var xml = File.ReadAllText("./SampleFiles/MSCE_IfcsumDataset.xml");
            var ifcsum = IfcsumDataset.Deserialize(xml);
            xml = File.ReadAllText("./SampleFiles/MSCE_VesselPortCallInfo.xml");
            var vpci = Mappers.VesselPortCallInfoMapper.Map(xml);
            var bld = Mappers.IfcsumMapper.Map(ifcsum, vpci);
            return bld;
        }

        [TestMethod]
        public void AddBillOfLadingPollyRetryTest()
        {
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri(config.Value.BlockchainEndpoint);

            RetryPolicy<HttpResponseMessage> _httpRequestPolicy;
            var retContext = new Polly.Context();
            retContext.Add("client", clientApi);
            _httpRequestPolicy = Policy.HandleResult<HttpResponseMessage>(
                    r => r.StatusCode == HttpStatusCode.Unauthorized)
                .Retry(1, onRetry: (resp, retry, context) =>
                {
                    if (resp.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var token = GetToken();
                        var cli = context["client"] as HttpClient;
                        if (cli != null && !string.IsNullOrEmpty(token))
                            cli.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                });
            string blstr = File.ReadAllText("./SampleFiles/BillOfLadingDeclaration.json");
            BillOfLadingDeclaration bld = Newtonsoft.Json.JsonConvert.DeserializeObject<BillOfLadingDeclaration>(blstr);
            var res = _httpRequestPolicy.Execute(ctx => clientApi.PostAsJsonAsync<BillOfLadingDeclaration>("/api/BillOfLadingService", bld).Result, retContext);
            if (!res.IsSuccessStatusCode)
                Assert.Fail();
        }

        [TestMethod]
        public void AddBillOfLadingSinPollyTest()
        {
            HttpResponseMessage res = null;
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri(config.Value.BlockchainEndpoint);
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                BillOfLadingDeclaration bld = GetBillOfLadingDeclarationMock();
                clientApi.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                res = clientApi.PostAsJsonAsync<BillOfLadingDeclaration>("api/BillOfLadingService", bld).Result;
            }
            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsSuccessStatusCode);
        }

        private static BillOfLadingDeclaration GetBillOfLadingDeclarationMock()
        {
            string json = File.ReadAllText("./SampleFiles/MSCE_BillOfLadingDeclaration.json");
            BillOfLadingDeclaration bld = BillOfLadingDeclaration.FromJson(json);
            return bld;
        }

        #endregion

        private static VesselPortCallDeclaration GetVesselPortCallDeclarationMock()
        {
            var json = File.ReadAllText("./SampleFiles/MSCE_VesselPortCallDeclaration.json");
            return VesselPortCallDeclaration.FromJson(json);
        }

        private string GetToken()
        {
            string res = null;

            HttpClient authClient = new HttpClient();
            authClient.BaseAddress = new Uri(config.Value.AuthEndpoint);
            var authSvc = new AuthService(authClient, config);
            res = authSvc.GetToken().Result;

            return res;
        }

        [TestInitialize]
        public void Setup()
        {
            var cfg = new Infoport.GesportPCS.Api.Configuration.Configuration()
            {
                Client = "pcs-client",
                Secret = "pcs-secret2",
                AuthEndpoint = UrlAuth,
                BlockchainEndpoint = UrlApiBlockchain,
                HealthEndpoint = UrlHealth
            };
            config = Options.Create(cfg);
        }
    }
}
