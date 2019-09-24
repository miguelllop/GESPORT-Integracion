using Gesport.Middleware.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Messages
{
    /// <summary>
    /// DTO para mensajes IMsgBusinessEvent
    /// </summary>
    public class MsgBusinessEvent : IMsgBusinessEvent
    {
        public Guid id { get; set; }

        public EventQueue Event { get; set; }
    }
}
