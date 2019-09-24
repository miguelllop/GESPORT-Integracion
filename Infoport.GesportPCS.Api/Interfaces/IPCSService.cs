using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Interfaces
{
    /// <summary>
    /// Interfaz para acceso a servicios publicados por PCS que se consumen desde el api
    /// </summary>
    public interface IPCSService
    {
        Task<Code> GetCountryByCode(string code);
        Task<Code> GetPortByCode(string code);
        Task<Code> GetFlagByCode(string code);
        Task<VesselPortCallInfo> GetVesselPortCallInfo(string portCall);
    }
}
