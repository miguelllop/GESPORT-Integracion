using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Utils
{
    public static class BERMANExtension
    {
        public static VesselPortCallDeclaration ToVesselPortCallDeclaration(this BERMAN berman)
        {
            return BermanMapper.Map(berman);
        }
    }
}
