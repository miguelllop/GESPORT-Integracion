using Infoport.GesportPCS.Api.Mappers;
using Infoport.GesportPCS.Api.Models;
using Infoport.GesportPCS.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Infoport.GesportPCS.Api.Test
{
    [TestClass]
    public class GesportPCSMappingTest
    {
        [TestInitialize]
        public void Setup()
        {

        }

        #region Test sobre BERMAN
        [TestMethod]
        public void LoadBermanWithXsdTest()
        {
            var xmlStr = File.ReadAllText("./SampleFiles/BERMAN_sample2.xml");
            var berman = BERMAN.Deserialize(xmlStr);
            Assert.IsNotNull(berman);
        }

        [TestMethod]
        public void MappingBermanToVesselPortCallDeclaration2Test()
        {
            var xml = File.ReadAllText("./SampleFiles/MSCE_BERMAN.xml");
            var berman = BERMAN.Deserialize(xml);
            var vpcd = Mappers.BermanMapper.Map(berman);
            Assert.IsNotNull(vpcd);
            var json = vpcd.ToJson();

            xml = File.ReadAllText("./SampleFiles/MSCE_IfcsumDataset.xml");
            var ifcsum = IfcsumDataset.Deserialize(xml);
            xml = File.ReadAllText("./SampleFiles/MSCE_VesselPortCallInfo.xml");
            var esc = Mappers.VesselPortCallInfoMapper.Map(xml);
            var bld = Mappers.IfcsumMapper.Map(ifcsum, esc);
            json = bld.ToJson();
        }

        [TestMethod]
        public void MappingBermanToVesselPortCallDeclarationTest()
        {
            var xml = File.ReadAllText("./SampleFiles/BERMAN_sample3.xml");
            var berman = BERMAN.Deserialize(xml);

            var vpcd = Mappers.BermanMapper.Map(berman);
            Assert.IsNotNull(vpcd);
            var json = vpcd.ToJson();
            var vpc = vpcd.VesselPortCall;
            Assert.AreEqual("1201806590", vpcd.VesselPortCall.PortCallNumber); // PortCallNumber
            Assert.AreEqual("1201806590@ESVLC", vpc.PortCallId);    // PortCallId
            Assert.AreEqual("46118506590", vpc.SummaryDeclarationNumber); // Sumaria
            Assert.AreEqual("ESVLC", vpc.PortOfCall.Code1); // Codigo port of call
            Assert.AreEqual("ITSPE", vpc.NextPortOfCall.Code1); // Codigo next port of call
            Assert.AreEqual("MT", vpc.CountryOfEntry.Code1); // Codigo country of entry
            Assert.AreEqual("201901031700", vpc.Eta); // ETA
            Assert.AreEqual("ESVLC", vpc.VesselAgent.Office.Code1); // Office code
            Assert.AreEqual("B98261944", vpc.VesselAgent.Organization.Code1); // Organization code
            Assert.AreEqual("M.S.C. ESPAÑA, SLU", vpc.VesselAgent.Organization.Name); // Organization name
            Assert.AreEqual("B98261944@ESVLC", vpc.VesselAgent.OfficeId); // Office Id
            Assert.AreEqual("Avda. Del Puerto, nº 273 - 3º - 3ª,", vpc.VesselAgent.Address.Street); // Address
            Assert.AreEqual("ESVLC", vpc.VesselAgent.Address.City); // City
            Assert.AreEqual("46011", vpc.VesselAgent.Address.PostalCode); // Postal code
            Assert.AreEqual("ES", vpc.VesselAgent.Address.Country); // Country code
            Assert.AreEqual("FJ852E", vpc.ArrivalVoyageNumber); // Arrival voyage number
            Assert.AreEqual("9461415", vpc.Vessel.Code1); // Vessel IMO
            Assert.AreEqual("MSC DEILA           ", vpc.Vessel.Name); // Vessel name
            Assert.AreEqual("PA", vpc.Flag.Code1); // Flag code
            Assert.AreEqual(4, vpc.DischargePortReferences.Count); // 4 Referencias
            Assert.AreEqual("6", vpc.DischargePortReferences.First(x => x.ReferenceType == "XC").ReferenceValue); // Referencia XC existe y valor correcto
            Assert.AreEqual("VLC013", vpc.DischargePortReferences.First(x => x.ReferenceType == "ZTC").ReferenceValue); // Tiene referencia ZTC y valor correcto
            Assert.IsTrue(vpc.DischargePortReferences.Any(x => x.ReferenceType == "ZNR")); // Tiene referencia ZNR
            Assert.IsTrue(vpc.DischargePortReferences.Any(x => x.ReferenceType == "ZS4")); // Tiene referencia ZS4
        }
        #endregion

        #region Test sobre IFCSUM
        [TestMethod]
        public void LoadIfcsumWithXsdTest()
        {
            var xmlStr = File.ReadAllText("./SampleFiles/SUMARIA_sample1.xml");
            var sumaria = IfcsumDataset.Deserialize(xmlStr);
            Assert.IsNotNull(sumaria);
        }

        [TestMethod]
        public void MappingIfcsumToBillOfLadingDeclarationTest()
        {
            var xml = File.ReadAllText("./SampleFiles/SUMARIA_sample2.xml");
            var ifcsum = IfcsumDataset.Deserialize(xml);
            xml = File.ReadAllText("./SampleFiles/VesselPortCallInfo_sample1.xml");
            var vpci = Mappers.VesselPortCallInfoMapper.Map(xml);
            var bld = Mappers.IfcsumMapper.Map(ifcsum, vpci);
            var json = bld.ToJson();
            Assert.IsNotNull(bld);
        }
        #endregion

        #region VesselPortCallInfo
        [TestMethod]
        public void LoadVesselPortCallInfoTest()
        {
            VesselPortCallInfo resp;
            var xmlStr = File.ReadAllText("./SampleFiles/VesselPortCallInfo_sample1.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(VesselPortCallInfo));

            using (var stream = new StringReader(xmlStr))
            {
                resp = (VesselPortCallInfo)serializer.Deserialize(stream);
            }
            Assert.IsNotNull(resp);
        }

        [TestMethod]
        public void MappingVesselPortCallInfoTest()
        {
            var xml = File.ReadAllText("./SampleFiles/VesselPortCallInfo_sample1.xml");
            var vpci = Mappers.VesselPortCallInfoMapper.Map(xml);
            Assert.IsNotNull(vpci);
        }
        #endregion
    }
}
