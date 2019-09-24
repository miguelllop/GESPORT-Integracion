using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gesport.Middleware.Api;

namespace Infoport.GesportPCS.Api.Messages
{
    /// <summary>
    /// DTO para mensajes IMsgBillOfLading
    /// </summary>
    public class MsgBillOfLading : IMsgBillOfLading
    {
        public BillOfLadingDeclaration BillOfLadingDecl { get; set; }
        public Guid id { get; set; }
    }
}
