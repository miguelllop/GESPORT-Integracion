using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Interfaces
{
    /// <summary>
    /// Interfaz que contiene parámetros de configuración internos
    /// </summary>
    public interface IConfig
    {
        string Client { get; set; }
        string Secret { get; set; }
        string HealthEndpoint { get; set; }
        string BlockchainEndpoint { get; set; }
        string EventsEndpoint { get; set; }
        string AuthEndpoint { get; set; }
        bool MonitorFilesEnabled { get; set; }
        int CheckPeriodSeg { get; set; }
        string FilesPath { get; set; }
        string SharedSecret { get; set; }

    }
}
