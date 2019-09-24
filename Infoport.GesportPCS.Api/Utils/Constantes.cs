using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Utils
{
    public static class Constantes
    {
        public static readonly Dictionary<string, string> PortCodes = new Dictionary<string, string>() { { "1", "4611" }, { "2", "4631" }, { "3", "4621" } };
        public static readonly Dictionary<string, string> PortLocation = new Dictionary<string, string>() { { "1", "ESVLC" }, { "2", "ESGAN" }, { "3", "ESSAG" } };
    }
}
