using Infoport.GesportPCS.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Configuration
{
    public class Configuration : IConfig
    {
        public string Client { get; set; }
        public string Secret { get; set; }
        public string HealthEndpoint { get; set; }
        public string BlockchainEndpoint { get; set; }
        public string EventsEndpoint { get; set; }
        public string AuthEndpoint { get; set; }
        public bool MonitorFilesEnabled { get; set; }
        public int CheckPeriodSeg { get; set; }
        public string FilesPath { get; set; }
        public string SharedSecret { get; set; }
    }
}
