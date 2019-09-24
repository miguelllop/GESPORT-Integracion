using Infoport.GesportPCS.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infoport.GesportPCS.Api.Mappers
{
    /// <summary>
    /// Clase estática que mapea xml a objeto de escala PCS
    /// </summary>
    public static class VesselPortCallInfoMapper
    {
        /// <summary>
        /// Método que genera instancia de escala PCS a partir de un xml
        /// </summary>
        /// <param name="xml">xml de la escala obtenido desde PCS</param>
        /// <returns>VesselPortCallInfo</returns>
        public static VesselPortCallInfo Map(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VesselPortCallInfo));

            using (var stream = new StringReader(xml))
            {
                return (VesselPortCallInfo)serializer.Deserialize(stream);
            }

        }
    }

    
    
}
