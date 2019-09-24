using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infoport.GesportPCS.Api.Models
{
    [XmlTypeAttribute(AnonymousType = true)]
    public class VesselPortCallOperation
    {
        public string TerminalName { get; set; }
        public string QuayName { get; set; }
        public string BerthNumber { get; set; }
        public string OperationTypeCode { get; set; }
        public string EstimatedStartOfOperationsDate { get; set; }
        public string ActualStartOfOperationsDate { get; set; }
        public string EstimatedEndOfOperationsDate { get; set; }
        public string ActualEndOfOperationsDate { get; set; }
    }
}
