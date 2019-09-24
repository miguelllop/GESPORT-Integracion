using Gesport.Middleware.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Interfaces
{
    /// <summary>
    /// Interfaz para consumo de los métodos publicados por el middleware Gesport4.0
    /// </summary>
    public interface IBlockchainService
    {
        /// <summary>NotifyArrival</summary>
        /// <param name="notifyArrivalDeclaration">notifyArrivalDeclaration</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> ArrivalNoticeServiceAsync(NotifyArrivalDeclaration notifyArrivalDeclaration);

        /// <summary>NotifyArrival</summary>
        /// <param name="notifyArrivalDeclaration">notifyArrivalDeclaration</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> ArrivalNoticeServiceAsync(NotifyArrivalDeclaration notifyArrivalDeclaration, System.Threading.CancellationToken cancellationToken);

        /// <summary>AddBls</summary>
        /// <param name="billOfLadingDeclaration">billOfLadingDeclaration</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> BillOfLadingServiceAsync(BillOfLadingDeclaration billOfLadingDeclaration);

        /// <summary>AddBls</summary>
        /// <param name="billOfLadingDeclaration">billOfLadingDeclaration</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> BillOfLadingServiceAsync(BillOfLadingDeclaration billOfLadingDeclaration, System.Threading.CancellationToken cancellationToken);

        /// <summary>BusinessEvent</summary>
        /// <param name="businessEvent">businessEvent</param>
        /// <param name="portCallCode">portCallCode</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> BusinessEventAsync(EventQueue businessEvent, string portCallCode);

        /// <summary>BusinessEvent</summary>
        /// <param name="businessEvent">businessEvent</param>
        /// <param name="portCallCode">portCallCode</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> BusinessEventAsync(EventQueue businessEvent, string portCallCode, System.Threading.CancellationToken cancellationToken);

        /// <summary>AddVesselPortCall</summary>
        /// <param name="vesselPortCallDeclaration">vesselPortCallDeclaration</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> VesselPortCallServiceAsync(VesselPortCallDeclaration vesselPortCallDeclaration);

        /// <summary>AddVesselPortCall</summary>
        /// <param name="vesselPortCallDeclaration">vesselPortCallDeclaration</param>
        /// <returns>OK</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> VesselPortCallServiceAsync(VesselPortCallDeclaration vesselPortCallDeclaration, System.Threading.CancellationToken cancellationToken);
    }
}
