using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Interfaces
{
    /// <summary>
    /// Interfaz base de mansajes para gestión de mensajería
    /// </summary>
    public interface IMessage
    {
        Guid id { get; set; }
    }
}
