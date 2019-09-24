using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Services
{
    /// <summary>
    /// Implementación de la interfaz para consumo del api middleware Gesport4.0
    /// Se ha obtenido a partir del swagger suministrado. Sólo se han implementado
    /// los métodos de los que se hace uso en la aplicación.
    /// Se hace uso de la librería Polly para gestionar reintentos y gestión de errores
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        HttpClient _cli;
        IAuthService _authSvc;
        BlockchainClient _blockchainClient;
        RetryPolicy _httpRequestPolicy;
        Context _polCtx = new Context();

        public BlockchainService(IHttpClientFactory cliFactory, IAuthService authSvc) : this(cliFactory.CreateClient("blockchain"), authSvc)
        {
            
        }

        public BlockchainService(HttpClient client, IAuthService authSvc)
        {
            _cli = client;
            _authSvc = authSvc;
            _polCtx = new Polly.Context();
            _polCtx.Add("client", _cli);
            _httpRequestPolicy = Policy.Handle<Exception>(ex => ex is SwaggerException && ((SwaggerException)ex).StatusCode == (int)HttpStatusCode.Unauthorized)
                .RetryAsync(2, onRetryAsync: async (resp, retry, context) =>
                {
                    var token = await _authSvc.GetToken();
                    var cli = context["client"] as HttpClient;
                    if (cli != null && !string.IsNullOrEmpty(token))
                        cli.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                });
            _blockchainClient = new BlockchainClient(_cli);
        }

        public Task<FileResponse> AddOfficeAsync(IDictionary<string, object> office)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> AddOfficeAsync(IDictionary<string, object> office, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ArrivalNoticeServiceAsync(NotifyArrivalDeclaration notifyArrivalDeclaration)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ArrivalNoticeServiceAsync(NotifyArrivalDeclaration notifyArrivalDeclaration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BillOfLadingServiceAsync(ShippingAgentDeclaration billOfLadingDeclaration)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BillOfLadingServiceAsync(ShippingAgentDeclaration billOfLadingDeclaration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BillOfLadingServiceAsync(BillOfLadingDeclaration billOfLadingDeclaration)
        {
            Log.Debug("Añadiendo declaración BL");
            return _httpRequestPolicy.ExecuteAsync(async ctx => await _blockchainClient.BillOfLadingServiceAsync(billOfLadingDeclaration), _polCtx);
        }

        public Task<FileResponse> BillOfLadingServiceAsync(BillOfLadingDeclaration billOfLadingDeclaration, CancellationToken cancellationToken)
        {
            Log.Debug("Añadiendo declaración BL");
            return _httpRequestPolicy.ExecuteAsync((ctx, ct) => _blockchainClient.BillOfLadingServiceAsync(billOfLadingDeclaration, ct), _polCtx, cancellationToken);
        }

        public Task<FileResponse> BusinessEventAsync(EventQueue businessEvent, string portCallCode)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BusinessEventAsync(EventQueue businessEvent, string portCallCode, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ChangeOfficeAsync(IDictionary<string, object> office)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ChangeOfficeAsync(IDictionary<string, object> office, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ConfirmMembershipAsync(string identityId, string isAdmin, string officeKey)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ConfirmMembershipAsync(string identityId, string isAdmin, string officeKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetBillOfLadingAsync(BillOfLadingDeclaration billOfLadingDeclaration)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetBillOfLadingAsync(BillOfLadingDeclaration billOfLadingDeclaration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetCurrentIdentityIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetCurrentIdentityIdAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetCurrentParticipantAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetCurrentParticipantAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetEventAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetEventAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetOfficeAsync(string identity)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetOfficeAsync(string identity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetTxEventAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetTxEventAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetVesselPortCallAsync(VesselPortCallDeclaration vesselPortCallDeclaration)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetVesselPortCallAsync(VesselPortCallDeclaration vesselPortCallDeclaration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> RemoveOfficeAsync(IDictionary<string, object> office)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> RemoveOfficeAsync(IDictionary<string, object> office, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> RequestMembershipAsync(string membership)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> RequestMembershipAsync(string membership, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> VesselPortCallServiceAsync(VesselPortCallDeclaration vesselPortCallDeclaration)
        {
            Log.Debug("Añadiendo declaracion de escala");
            return _httpRequestPolicy.ExecuteAsync(async ctx => await _blockchainClient.VesselPortCallServiceAsync(vesselPortCallDeclaration), _polCtx);
        }

        public Task<FileResponse> VesselPortCallServiceAsync(VesselPortCallDeclaration vesselPortCallDeclaration, CancellationToken cancellationToken)
        {
            Log.Debug("Añadiendo declaracion de escala");
            return _httpRequestPolicy.ExecuteAsync((ctx, ct) => _blockchainClient.VesselPortCallServiceAsync(vesselPortCallDeclaration, ct), _polCtx, cancellationToken);
        }
    }
}
