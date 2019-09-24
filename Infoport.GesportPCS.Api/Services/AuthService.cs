using Infoport.GesportPCS.Api.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Services
{
    /// <summary>
    /// Implementación de la interfaz de autorización. Se encarga de obtener tokens y de validarlos contra
    /// el servicio de autorización del middleware Gesport4.0
    /// </summary>
    public class AuthService : IAuthService
    {
        HttpClient client;
        IConfig _config;

        public AuthService(HttpClient cli, IOptions<Configuration.Configuration> config)
        {
            client = cli;
            _config = config.Value;
        }

        public AuthService(IHttpClientFactory cliFactory, IOptions<Configuration.Configuration> config) : this(cliFactory.CreateClient("auth"), config)
        {
        }

        public async Task<string> GetToken()
        {
            string res = (string)null;

            Log.Debug("Obteniendo token");
            var req = new HttpRequestMessage(HttpMethod.Post, "/oauth/token?grant_type=client_credentials");
            var cred = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_config.Client}:{_config.Secret}"));
            req.Headers.Add("Authorization", $"Basic {cred}");
            var resp = await client.SendAsync(req);
            if (resp.IsSuccessStatusCode)
            {
                Log.Debug("Toker obtenido");
                var json = await resp.Content.ReadAsStringAsync();
                res = JObject.Parse(json)["access_token"].ToString();
            }
            else
            {
                Log.Error($"No se ha podido obtener un token -> Resp: {resp.StatusCode}\n{resp.Content.ReadAsStringAsync().Result}");
            }

            return res;
        }

        public async Task<bool> CheckToken(string token)
        {
            var res = false;

            Log.Debug("Comprobando token");
            var req = new HttpRequestMessage(HttpMethod.Post, $"/oauth/check_token?token={token}");
            var cred = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_config.Client}:{_config.Secret}"));
            req.Headers.Add("Authorization", $"Basic {cred}");
            var resp = await client.SendAsync(req);
            if (resp.IsSuccessStatusCode)
            {
                res = true;
                Log.Debug("Token validado");
            }
            else
            {
                Log.Warning($"Fallo validación token -> Error: {resp.StatusCode} - {resp.ReasonPhrase}\n{Task.FromResult(resp.Content.ReadAsStreamAsync())}");
            }
            return res;
        }
    }
}
