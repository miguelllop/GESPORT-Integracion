using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Retry;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using Infoport.GesportPCS.Api.Utils;
using Infoport.GesportPCS.Api.Services;
using wcfPcsEscalas;
using System.Threading.Tasks;
using Infoport.GesportPCS.Api.Mappers;

namespace Infoport.GesportPCS.Api.Test
{
    [TestClass]
    public class GesportPCSTest
    {



        [TestMethod]
        public void PcsGetVesselPortCallInforTest()
        {
            var svc = new PublicWebServiceClient(PublicWebServiceClient.EndpointConfiguration.BasicHttpBinding_PublicWebService);
            var vpc = svc.GetVesselPortCallInfoAsync("1201901244");
            var resp = VesselPortCallInfoMapper.Map(vpc.Result);
            Assert.IsNotNull(resp);
        }
        
        [TestInitialize]
        public void Setup()
        {
            
        }
    }
}
