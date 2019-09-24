using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Controllers;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Messages;
using Infoport.GesportPCS.Api.Services;
using Infoport.GesportPCS.Api.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Serilog;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Options;

namespace Infoport.GesportPCS.Api.Test
{
    [TestClass]
    public class GestportPCSBrokerTest
    {
        string UrlHealth = "http://gesport4offchain.belikesoftware.com:8080/public/health";
        string UrlApiBlockchain = "http://gesport4offchain.belikesoftware.com:8080";
        string UrlAuth = "http://gesport4offchain.belikesoftware.com:8080";
        IOptions<Configuration.Configuration> config;
        IMessageBroker broker;

        [TestMethod]
        public void SendAddVesselPortCallBrokerTest()
        {
            broker.PublishAsync(new MsgAddVesselPortCall() { id = new Guid(), VesselPortCallDecl = GetVesselPortCallDeclarationMock() }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void AddVesselPortCallFullFlowBrokerTest()
        {
            var guid = new Guid();
            var bcSvc = new BlockChainServiceMock();
            var _broker = new MessageBrokerService(bcSvc);
            var cli = new HttpClient();
            cli.BaseAddress = new Uri(config.Value.AuthEndpoint);
            var authSvc = new AuthService(cli, config);
            // Generamos una transacción
            _broker.PublishAsync(new MsgAddVesselPortCall() { id = guid, VesselPortCallDecl = GetVesselPortCallDeclarationMock() }).Wait();
            // Validamos que hay una transacción pendiente de confirmar
            Assert.IsTrue(_broker.PendingTransactions == 1);
            // Generamos un evento de confirmación y lo procesamos mediante su controller
            var evt = GetVesselPortCallDeclarationTransactionEventMock();
            // Creamos instancia de controller para invocar recepción de evento
            var ctrlBE = new BusinessEventController(_broker, authSvc);
            var res = ctrlBE.Post(evt);
            // Validamos que no queda ninguna transacción pendiente
            Assert.IsTrue(broker.PendingTransactions == 0);
        }

        [TestInitialize]
        public void Setup()
        {
            var cfg = new Infoport.GesportPCS.Api.Configuration.Configuration()
            {
                Client = "pcs-client",
                Secret = "pcs-secret",
                AuthEndpoint = UrlAuth,
                BlockchainEndpoint = UrlApiBlockchain,
                HealthEndpoint = UrlHealth
            };
            config = Options.Create(cfg);
            var cli = new HttpClient();
            cli.BaseAddress = new Uri(config.Value.BlockchainEndpoint);
            var bcSvc = new BlockchainService(cli, new AuthService(new HttpClient() { BaseAddress = new Uri(config.Value.AuthEndpoint) }, config));
            broker = new MessageBrokerService(bcSvc);
        }

        private static VesselPortCallDeclaration GetVesselPortCallDeclarationMock()
        {
            var json = File.ReadAllText("./SampleFiles/VesselPortCallDeclaration.json");
            var vpcd = Newtonsoft.Json.JsonConvert.DeserializeObject<VesselPortCallDeclaration>(json);
            return vpcd;
        }

        private static EventQueue GetVesselPortCallDeclarationTransactionEventMock()
        {
            var json = File.ReadAllText("./SampleFiles/TransactionEvent.json");
            var transEvt = Newtonsoft.Json.JsonConvert.DeserializeObject<EventQueue>(json);
            transEvt.TransactionId = "ID_TEST";
            return transEvt;
        }
    }

    
}
