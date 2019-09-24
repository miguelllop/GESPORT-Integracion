using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de autenticación con middelware Gesport4.0. 
    /// Define métodos para obtener y validar tokens
    /// </summary>
    public interface IAuthService
    {
        Task<string> GetToken();
        Task<bool> CheckToken(string token);
    }
}
