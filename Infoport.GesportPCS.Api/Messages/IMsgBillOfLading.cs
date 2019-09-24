using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Messages
{
    /// <summary>
    /// Interfaz de mensaje BillOfLadingDeclaration
    /// </summary>
    interface IMsgBillOfLading : IMessage
    {
        BillOfLadingDeclaration BillOfLadingDecl { get; set; }
    }
}
