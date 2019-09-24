using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Messages;
using Infoport.GesportPCS.Api.Services;
using Infoport.GesportPCS.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using wcfPcsEscalas;

namespace Infoport.GesportPCS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PcsEventController : ControllerBase
    {
        readonly IMessageBroker _broker;
        readonly IPCSService _pcsSvc;
        readonly IConfig _config;

        public PcsEventController(IMessageBroker broker, IPCSService pcsSvc, IOptions<Configuration.Configuration> config)
        {
            _broker = broker;
            _pcsSvc = pcsSvc;
            _config = config.Value;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PCSMessage msg)
        {
            Log.Information($"Evento PCS recibido: {msg.Type}");
            IActionResult resp = BadRequest();
            if (msg == null)
                return resp;
            if (AutorizacionValida(Request))
            {
                try
                {
                    switch (msg.Type)
                    {
                        case "BERMAN":
                            var berman = BERMAN.Deserialize(msg.Payload);
                            Log.Information($"BERMAN parseado correctamente -> Escala: {berman.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "SSR")?.ReferenceIdentifier.Value}");
                            var vpcd = berman.ToVesselPortCallDeclaration();
                            Log.Information($"BERMAN mapeado -> PortCall: {vpcd.VesselPortCall.PortCallId}");
                            await _broker.PublishAsync(new MsgAddVesselPortCall() { VesselPortCallDecl = vpcd });
                            resp = Ok();
                            break;
                        case "IFCSUM":
                            var ds = IfcsumDataset.Deserialize(msg.Payload);
                            var ifcsum = ds.Items.First() as IfcsumDatasetDeclarations;
                            Log.Information($"Sumaria parseada correctamente -> Sumaria: {ifcsum.SummaryDeclaration}");
                            Log.Information($"Obteniendo información de la escala -> Escala: {ifcsum.PortCallReference}");
                            var escala = await _pcsSvc.GetVesselPortCallInfo(ifcsum.PortCallReference);
                            if (escala != null)
                            {
                                var bld = Mappers.IfcsumMapper.Map(ds, escala);
                                Log.Information($"Sumaria mapeada: {bld.Declaration.SummaryDeclarationNumber}");
                                await _broker.PublishAsync(new MsgBillOfLading() { BillOfLadingDecl = bld });
                                resp = Ok();
                            }
                            else
                            {
                                Log.Error($"No se han podido obtener datos de la escala: {ifcsum.PortCallReference}");
                            }
                            break;
                        default:
                            Log.Error($"Tipo de mensaje no soportado -> Tipo: {msg.Type}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error al procesar mensaje de PCS");
                    throw;
                }
            }
            else
            {
                Log.Error($"Petición no autorizada -> Origen: {Request.HttpContext.Connection.RemoteIpAddress.ToString()}");
                resp = Unauthorized();
            }
            return resp;
        }

        private bool AutorizacionValida(HttpRequest request)
        {
            var res = false;
            if (string.IsNullOrEmpty(_config.SharedSecret))
            {
                res = true;
            }
            else
            {
                string auth = request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(auth) && auth.ToLower().Contains("bearer"))
                {
                    var tk = auth.Split(" ").Last().Trim();
                    if (tk == _config.SharedSecret)
                    {
                        res = true;
                    }
                }
            }
            return res;
        }
    }
}
