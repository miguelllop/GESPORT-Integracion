using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wcfPcsEscalas;

namespace Infoport.GesportPCS.Api.Services
{
    /// <summary>
    /// Implementación de la interfaz IPCService. Contiene métodos de solicitud de información a PCS
    /// </summary>
    public class PCSService : IPCSService
    {
        PublicWebServiceClient _pcsClient;
        public PCSService(PublicWebServiceClient pcsClient)
        {
            _pcsClient = pcsClient;
        }
        public Task<Code> GetCountryByCode(string code)
        {
            throw new NotImplementedException();
        }

        public Task<Code> GetFlagByCode(string code)
        {
            throw new NotImplementedException();
        }

        public Task<Code> GetPortByCode(string code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Método para obtener información de una escala
        /// </summary>
        /// <param name="portCall">Número de escala</param>
        /// <returns>VesselPortCallInfo con la información de la escala</returns>
        public async Task<VesselPortCallInfo> GetVesselPortCallInfo(string portCall)
        {
            VesselPortCallInfo res = null;
            var xml = await _pcsClient.GetVesselPortCallInfoAsync(portCall);
            Log.Debug($"Datos obtenidos de la escala en PCS:\n{xml}");
            res = Mappers.VesselPortCallInfoMapper.Map(xml);
            return res;
        }
    }
}
