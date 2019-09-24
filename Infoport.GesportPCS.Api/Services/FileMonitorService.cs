using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Services
{
    internal class FileMonitorService : IHostedService, IDisposable
    {
        private readonly IConfig _config;
        private HttpClient _client;
        private Timer _timer;

        public FileMonitorService(IOptions<Configuration.Configuration> config, IHttpClientFactory cliFactory) : this(config, cliFactory.CreateClient())
        {
        }

        public FileMonitorService(IOptions<Configuration.Configuration> config, HttpClient cli)
        {
            _config = config.Value;
            _client = cli;
            _client.BaseAddress = new Uri(_config.EventsEndpoint);
        }
                

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Iniciando monitorización de directorio");

            _timer = new Timer(DoWork, null, 2000, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (Directory.Exists(_config.FilesPath))
            {
                Log.Debug("Procesando archivos BERMAN...");
                ProcesarArchivos("BER", "BERMAN");
                Log.Debug("Procesando archivos IFCSUM...");
                ProcesarArchivos("IFC", "IFCSUM");
            }
            else
            {
                Log.Error("No existe el directorio de archivos a procesar");
            }

            _timer.Change(_config.CheckPeriodSeg * 1000, Timeout.Infinite);
        }

        private void ProcesarArchivos(string extension, string tipo)
        {
            var lstFiles = Directory.GetFiles(_config.FilesPath, $"*.{extension}");
            if (lstFiles.Length > 0)
                Log.Debug($"Procesando {lstFiles.Length} archivos {tipo}");
            foreach (var f in lstFiles)
            {
                try
                {
                    if (File.Exists(f))
                    {
                        Log.Information($"Procesando archivo: {f}");
                        var content = File.ReadAllText(f);
                        PCSMessage msg = new PCSMessage() { Type = tipo, Payload = content };
                        var resp = InvokePCSApiEvent(msg);
                        if (resp.IsSuccessStatusCode)
                        {
                            var dirDest = Path.Combine(Path.GetDirectoryName(f), "Procesados");
                            if (!Directory.Exists(dirDest))
                                Directory.CreateDirectory(dirDest);
                            File.Copy(f, Path.Combine(dirDest, Path.GetFileName(f)), true);
                            File.Delete(f);
                            Log.Debug($"Archivo procesado correctamente: {f}");
                        }
                        else
                        {
                            Log.Error("Fallo realizar la solicitud de transaccion");
                            Log.Debug(resp.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error procesando archivo");
                }
            }
        }

        private HttpResponseMessage InvokePCSApiEvent(PCSMessage msg)
        {
            return Task.Run(() => _client.PostAsJsonAsync("", msg)).GetAwaiter().GetResult();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Parando servicio...");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
