using Infoport.GesportPCS.Api.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Services
{
    /// <summary>
    /// Servicio para testear que el api middleware de Gesport está operativo
    /// </summary>
    public static class HealthService
    {
        public static async Task<bool> TestHealth(HttpClient client, IOptions<Configuration.Configuration> config)
        {
            var res = false;

            if (client == null)
                throw new ArgumentException("HttpClient is null");
            try
            {
                var resp = await client.GetAsync(config.Value.HealthEndpoint);
                if (resp.IsSuccessStatusCode)
                    res = true;
            }
            catch (Exception ex)
            {
                Log.Error("Health test failed", ex);
            }
            return res;
        }
    }
}
