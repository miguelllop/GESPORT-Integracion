using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Interfaces
{
    /// <summary>
    /// Interfaz del servicio interno de mensajería que gestiona las transacciones pendientes de confirmar
    /// </summary>
    public interface IMessageBroker
    {
        int PendingTransactions { get; }
        void Publish(IMessage msg);
        Task PublishAsync(IMessage msg);
        void Send(IMessage msg);
        Task SendAsync(IMessage msg);
    }
}
