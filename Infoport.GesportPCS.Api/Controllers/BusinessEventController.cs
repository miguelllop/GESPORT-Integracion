using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Infoport.GesportPCS.Api.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Infoport.GesportPCS.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    public class BusinessEventController : Controller
    {
        IMessageBroker _broker;
        IAuthService _authSvc;
        public BusinessEventController(IMessageBroker broker, IAuthService authSvc)
        {
            _broker = broker;
            _authSvc = authSvc;
        }
        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]EventQueue value)
        {
            IActionResult res = BadRequest();
            if (value != null)
            {
                var token = getTokenFromReq(this.Request);
                if (await _authSvc.CheckToken(token))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Evento recibido y autorizado");
                    sb.AppendLine($"URL: {Request.GetDisplayUrl()}");
                    sb.AppendLine($"Contenido: {Newtonsoft.Json.JsonConvert.SerializeObject(value)}");
                    Log.Debug(sb.ToString());
                    await _broker.PublishAsync(new MsgBusinessEvent() { id = new Guid(), Event = value });
                    res = Ok();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Llamada no autorizada");
                    sb.AppendLine($"URL: {Request.GetDisplayUrl()}");
                    Log.Error(sb.ToString());
                    //var headers = Request.Headers.Select(x => $"{x.Key}={x.Value}");
                    //Log.Debug($"Headers: \n{string.Join("\n", headers.ToArray())}");
                    res = Unauthorized();
                }
            }

            return res;
        }

        private string getTokenFromReq(HttpRequest request)
        {
            var res = "";
            string auth = request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(auth) && auth.ToLower().Contains("bearer"))
            {
                res = auth.Split(" ").Last().Trim();
            }
            return res;
        }
    }
}
