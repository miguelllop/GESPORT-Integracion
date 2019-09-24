using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Messages;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Services
{
    /// <summary>
    /// Implementación de servicio simple de gestión las transacciones realizadas.
    /// Está implementado emulando un broker de mensajería aunque no se implementa realmente
    /// Las transacciones se almacenan internamente en un diccionario hasta recibir
    /// una confirmación positiva de la transacción
    /// </summary>
    public class MessageBrokerService : IMessageBroker
    {
        Dictionary<string, object> _pendingTrans = new Dictionary<string, object>();
        IBlockchainService _bcSvc;

        public int PendingTransactions => _pendingTrans.Count;

        public MessageBrokerService(IBlockchainService bcSvc)
        {
            _bcSvc = bcSvc;
        }

        public void Publish(IMessage msg)
        {
            throw new NotImplementedException();
        }

        public async Task PublishAsync(IMessage msg)
        {
            if (_bcSvc == null)
                throw new ArgumentNullException("Mensaje nulo");
            if (msg is IMsgAddVesselPortCall)
            {
                try
                {
                    var msgVpc = msg as IMsgAddVesselPortCall;
                    // Call to blockchain function
                    Log.Debug($"Enviando VesselPortCall -> Escala:{msgVpc.VesselPortCallDecl.VesselPortCall.PortCallId}");
                    var resp = await _bcSvc.VesselPortCallServiceAsync(msgVpc.VesselPortCallDecl);
                    if (resp.StatusCode == (int)HttpStatusCode.OK)
                    {
                        // Add message to pending transactions buffer
                        var sr = new StreamReader(resp.Stream, Encoding.UTF8);
                        var transId = sr.ReadToEnd();
                        if (string.IsNullOrEmpty(transId))
                        {
                            Log.Error($"No se ha recibido ID de transacción -> Escala: {msgVpc.VesselPortCallDecl.VesselPortCall.PortCallId}");
                            throw new Exception("Respuesta sin ID de transacción");
                        }
                        Log.Information($"VesselPortCall enviado -> Escala:{msgVpc.VesselPortCallDecl.VesselPortCall.PortCallId} - Trans. ID: {transId}");
                        _pendingTrans.Add(transId, msgVpc);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Fallo al invocar transaccion -> Respuesta: {resp.StatusCode}");
                        sb.AppendLine($"Escala: {msgVpc.VesselPortCallDecl.VesselPortCall.PortCallId}");
                        sb.AppendLine($"Respuesta:");
                        sb.AppendLine(new StreamReader(resp.Stream, Encoding.UTF8).ReadToEnd());
                        Log.Error(sb.ToString());
                        throw new Exception("Error al invocar llamada");
                    }
                } catch(Exception ex)
                {
                    Log.Error(ex, "Error procesando petición");
                }
            }
            else
            {
                if (msg is IMsgBillOfLading)
                {
                    var msgBl = msg as IMsgBillOfLading;
                    try
                    {
                        // Call to blockchain function
                        Log.Debug($"Enviando BillOfLading -> BL:{msgBl.BillOfLadingDecl.Declaration.Bls.First().BlNumber}");
                        var resp = await _bcSvc.BillOfLadingServiceAsync(msgBl.BillOfLadingDecl);
                        if (resp.StatusCode == (int)HttpStatusCode.OK)
                        {
                            // Add message to pending transactions buffer
                            var sr = new StreamReader(resp.Stream, Encoding.UTF8);
                            var transId = sr.ReadToEnd();
                            if (string.IsNullOrEmpty(transId))
                            {
                                Log.Error($"No se ha recibido ID de transacción -> BL: {msgBl.BillOfLadingDecl.Declaration.Bls.First().BlNumber}");
                                throw new Exception("Respuesta sin transaction ID");
                            }
                            Log.Information($"BillOfLading enviada -> BL:{msgBl.BillOfLadingDecl.Declaration.Bls.First().BlNumber} - TransId:{transId}");
                            _pendingTrans.Add(transId, msgBl);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"Fallo al invocar transaccion -> Respuesta: {resp.StatusCode}");
                            sb.AppendLine($"BL: {msgBl.BillOfLadingDecl.Declaration.Bls.First().BlNumber}");
                            sb.AppendLine($"Respuesta:");
                            sb.AppendLine(new StreamReader(resp.Stream, Encoding.UTF8).ReadToEnd());
                            Log.Error(sb.ToString());
                            throw new Exception("Error al invocar llamada");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Error procesando petición -> BL: {msgBl.BillOfLadingDecl.Declaration.Bls.First().BlNumber}");
                    }
                }
                else
                {
                    if (msg is IMsgBusinessEvent)
                    {
                        var msgEvt = msg as IMsgBusinessEvent;
                        if (msgEvt.Event.Type == EventQueueType.TRANSACTION)
                        {
                            await Task.Run(() => System.Threading.Thread.Sleep(5000));
                            Log.Information($"Evento de confirmacion de transacción recibida -> TransaccionId: {msgEvt.Event.TransactionId}");
                            if (_pendingTrans.TryGetValue(msgEvt.Event.TransactionId, out object obj))
                            {
                                if (obj is IMsgAddVesselPortCall)
                                {
                                    Log.Debug($"VesselPortCall notificacion de transaccion: {JsonConvert.SerializeObject(obj)}");
                                    if (msgEvt.Event.TransactionEvent.TransactionStatus.Equals("completed"))
                                        Log.Information($"Transaccion completada -> Escala: {((IMsgAddVesselPortCall)obj).VesselPortCallDecl.VesselPortCall.PortCallId} - TransId: {msgEvt.Event.TransactionId}");
                                    else
                                        Log.Error($"Fallo en la transaccion -> Status: {msgEvt.Event.TransactionEvent.TransactionStatus} - Escala: {((IMsgAddVesselPortCall)obj).VesselPortCallDecl.VesselPortCall.PortCallId} - TransId: {msgEvt.Event.TransactionId}");
                                    _pendingTrans.Remove(msgEvt.id.ToString());
                                    var res = msgEvt.Event.TransactionEvent.TransactionPayload;
                                    Log.Debug($"Payload: {JsonConvert.SerializeObject(res)}");
                                }
                                else
                                {
                                    if (obj is IMsgBillOfLading)
                                    {
                                        Log.Debug($"BillOfLading notificacion de transaccion: {JsonConvert.SerializeObject(obj)}");
                                        if (msgEvt.Event.TransactionEvent.TransactionStatus.Equals("completed"))
                                            Log.Information($"Transaccion completada -> BL: {((IMsgBillOfLading)obj).BillOfLadingDecl.Declaration.Bls.First().BlNumber} - TransId: {msgEvt.Event.TransactionId}");
                                        else
                                            Log.Error($"Fallo en la transaccion -> Status: {msgEvt.Event.TransactionEvent.TransactionStatus} - BL: {((IMsgBillOfLading)obj).BillOfLadingDecl.Declaration.Bls.First().BlNumber} - TransId: {msgEvt.Event.TransactionId}");
                                        _pendingTrans.Remove(msgEvt.id.ToString());
                                        var res = msgEvt.Event.TransactionEvent.TransactionPayload;
                                        Log.Debug($"Payload: {JsonConvert.SerializeObject(res)}");
                                    }
                                }
                            }
                            else
                            {
                                Log.Warning($"Confirmación de transacción recibida no solicitada ->\n{JsonConvert.SerializeObject(msgEvt.Event)}");
                            }
                        }
                        else
                        {
                            // If "broadcast" event, then log entry
                            if (msgEvt.Event.Type == EventQueueType.BUSINESS_EVENT)
                            {
                                var res = msgEvt.Event.BusinessEvent.EventPayload;
                                Log.Information($"Evento de notificacion recibido -> \nEvent Name: {msgEvt.Event.BusinessEvent.EventName}\nPayLoad: \n{JsonConvert.SerializeObject(msgEvt.Event.BusinessEvent.EventPayload)}");
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Message type not supported");
                    }
                }
            }
            return;
        }

        public void Send(IMessage msg)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(IMessage msg)
        {
            await PublishAsync(msg);
            return;
        }
    }
}
