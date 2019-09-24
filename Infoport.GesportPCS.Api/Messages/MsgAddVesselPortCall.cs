using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gesport.Middleware.Api;

namespace Infoport.GesportPCS.Api.Messages
{
    /// <summary>
    /// DTO para mensajes IMsgAddVesselPortCall
    /// </summary>
    public class MsgAddVesselPortCall : IMsgAddVesselPortCall
    {
        public Guid id { get; set; }
        public VesselPortCallDeclaration VesselPortCallDecl { get; set; }
    }
}
