using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Test.Mocks
{
    public class BlockChainServiceMock : IBlockchainService
    {
        public Task<FileResponse> ArrivalNoticeServiceAsync(NotifyArrivalDeclaration notifyArrivalDeclaration)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> ArrivalNoticeServiceAsync(NotifyArrivalDeclaration notifyArrivalDeclaration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BillOfLadingServiceAsync(BillOfLadingDeclaration billOfLadingDeclaration)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BillOfLadingServiceAsync(BillOfLadingDeclaration billOfLadingDeclaration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BusinessEventAsync(EventQueue businessEvent, string portCallCode)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> BusinessEventAsync(EventQueue businessEvent, string portCallCode, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> VesselPortCallServiceAsync(VesselPortCallDeclaration vesselPortCallDeclaration)
        {
            var id = "ID_TEST";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(id));
            var resp = new  FileResponse((int)HttpStatusCode.OK, null, ms, null, null);
            return Task.FromResult(resp);
        }

        public Task<FileResponse> VesselPortCallServiceAsync(VesselPortCallDeclaration vesselPortCallDeclaration, CancellationToken cancellationToken)
        {
            return VesselPortCallServiceAsync(vesselPortCallDeclaration);
        }
    }
}
