using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infoport.GesportPCS.Api.Models
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute("Result", Namespace = "", IsNullable = false)]
    public class VesselPortCallInfo
    {
        public string PortCall { get; set; }
        public string Status { get; set; }
        public string Vessel { get; set; }
        public string Voyage { get; set; }
        public string IMO { get; set; }
        public string MMSI { get; set; }
        public string CallSign { get; set; }
        public string VesselFlag { get; set; }
        public string VesselBuildingDate { get; set; }
        public string InternationalNavigation { get; set; }
        public string VesselType { get; set; }
        public string VesselGT { get; set; }
        public string VesselBeam { get; set; }
        public string VesselLength { get; set; }
        public string VesselMaximumDraught { get; set; }
        public string SummaryDeclaration { get; set; }
        public string SummaryRegistrationDate { get; set; }
        public string SummaryActivationDate { get; set; }
        [XmlElement("ETA")]
        public string EstimatedTimeArrival { get; set; }
        [XmlElement("ATA")]
        public string ActualTimeArrival { get; set; }
        [XmlElement("ETD")]
        public string EstimatedTimeDeparture { get; set; }
        [XmlElement("ATD")]
        public string ActualTimeDeparture { get; set; }
        public string PreviousPortOfCallName { get; set; }
        public string PortOfCallName { get; set; }
        public string NextPortOfCallName { get; set; }
        public string RegularLine { get; set; }
        public string VesselAgentName { get; set; }
        public string VesselAgentCif { get; set; }
        public string VesselAgentAddress { get; set; }
        public string VesselAgentPhone { get; set; }
        public string VesselAgentFax { get; set; }
        public string ShipOwner { get; set; }
        public string ShipOwnerCif { get; set; }
        public string LoadCarrierAgent { get; set; }
        public string UnloadCarrierAgent { get; set; }
        [XmlElementAttribute("Operation")]
        public VesselPortCallOperation[] Operation { get; set; }
    }
}
