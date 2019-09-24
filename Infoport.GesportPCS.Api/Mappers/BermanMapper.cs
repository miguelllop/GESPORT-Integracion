using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Mappers
{
    /// <summary>
    /// Clase estática para mapeo de mensajes BERMAN de PCS
    /// </summary>
    public static class BermanMapper
    {
        /// <summary>
        /// Método que mapea un mensaje BERMAN de PCS a una entidad VesselPortCall de Gesport4.0
        /// </summary>
        /// <param name="berman">BERMAN de PCS</param>
        /// <returns>VesselPortCallDeclaration de Gesport4.0</returns>
        public static VesselPortCallDeclaration Map(BERMAN berman)
        {
            var vpc = new VesselPortCall();
            vpc.DischargePortReferences = new List<Gesport.Middleware.Api.Reference>();
            if (berman == null)
                throw new ArgumentException("BERMAN nulo");
            // Escala (Port Call Number)
            vpc.PortCallNumber = berman.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "SSR")?.ReferenceIdentifier.Value;
            // Sumaria
            vpc.SummaryDeclarationNumber = $"{Constantes.PortCodes[vpc.PortCallNumber.Substring(0, 1)]}{vpc.PortCallNumber.Substring(4, 1)}5{vpc.PortCallNumber.Substring(5, 5)}";
            // Puerto escala // TODO: Añadir el resto de campos si es posible
            vpc.PortOfCall = new Code()
            {
                Code1 = berman.TransportInformationGroup.PlaceLocationIdentificationGroup
                                                .FirstOrDefault(x => x.PlaceLocationIdentification.PlaceLocationQualifier.Value == "153")?
                                                    .PlaceLocationIdentification.LocationIdentification.PlaceLocationIdentificationLine.Value
            };
            // VesselPortCallId
            vpc.PortCallId = $"{vpc.PortCallNumber}@{vpc.PortOfCall.Code1}";
            // Puerto Siguiente // TODO: Añadir el resto de campos si es posible
            vpc.NextPortOfCall = new Code()
            {
                Code1 = berman.TransportInformationGroup.PlaceLocationIdentificationGroup
                                                .FirstOrDefault(x => x.PlaceLocationIdentification.PlaceLocationQualifier.Value == "61")?
                                                    .PlaceLocationIdentification.LocationIdentification.PlaceLocationIdentificationLine.Value
            };
            // País Destino
            vpc.CountryOfEntry = new Code()
            {
                Code1 = berman.TransportInformationGroup.PlaceLocationIdentificationGroup
                            .FirstOrDefault(x => x.PlaceLocationIdentification.PlaceLocationQualifier.Value == "9")?
                                .PlaceLocationIdentification.LocationIdentification.PlaceLocationIdentificationLine.Value
            };
            // ETA
            vpc.Eta = berman.TransportInformationGroup.DateTimePeriod.FirstOrDefault(x => x.DateOrTimeOrPeriodFunctionCodeQualifier.Value == "132")?
                                                                                        .DateOrTimeOrPeriodValue.Value;
            var nag = berman.NameAndAddressGroup.FirstOrDefault(x => x.NameAndAddress.PartyFunctionCodeQualifier.Value == "CV");
            if (nag != null)
            {
                vpc.VesselAgent = new Party()
                {
                    Organization = new Code()
                    {
                        Code1 = nag.NameAndAddress.PartyIdentificationDetails.PartyIdentifier.Value,
                        Name = nag.NameAndAddress.PartyName.FirstOrDefault()?.Value
                    },
                    Address = new Address()
                    {
                        Street = nag.NameAndAddress.StreetNumberPostOfficeBox.Value,
                        City = nag.NameAndAddress.CityName.Value,
                        PostalCode = nag.NameAndAddress.PostalIdentificationCode.Value,
                        Country = nag.NameAndAddress.CountryNameCode.Value
                    },
                    // TODO: a revisar. De momento fijado por código
                    Office = new Code()
                    {
                        Code1 = "ESVLC"
                    },
                    OfficeId = $"{nag.NameAndAddress.PartyIdentificationDetails.PartyIdentifier.Value}@ESVLC"  // TODO: Revisar texto literal
                };
            }
            var govRG = berman.TransportInformationGroup.GovernalRequirementsGroup.FirstOrDefault(x => x.GovernmentalRequirements.TransportMovementCode.Value == "2"
                                                                                                    || x.GovernmentalRequirements.TransportMovementCode.Value == "ZDL");
            if (govRG != null)
            {
                vpc.ArrivalVoyageNumber = berman.TransportInformationGroup.GovernalRequirementsGroup
                                            .FirstOrDefault(x => x.Reference.Any(y => y.ReferenceQualifier.Value == "ZVE"))?
                                                .Reference.FirstOrDefault(o => o.ReferenceQualifier.Value == "ZVE").ReferenceIdentifier.Value;
                // Incluimos el número de sumarias como referencia XC
                vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                {
                    ReferenceType = "XC",
                    ReferenceComment = "Number of declarations",
                    ReferenceValue = govRG.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "ZCE")?.ReferenceIdentifier.Value
                });
            }
            // Barco
            vpc.Vessel = new Code()
            {
                Code1 = berman.TransportInformationGroup.TransportInformation.TransportIdentification.TransportMeansIdentificationNameIdentifier.Value,
                Name = berman.TransportInformationGroup.TransportInformation.TransportIdentification.TransportMeansIdentificationName.Value
            };
            // Bandera // TODO: Añadir el resto de campos si es posible
            vpc.Flag = new Code()
            {
                Code1 = berman.TransportInformationGroup.TransportInformation.TransportIdentification.TransportMeansNationalityCode.Value
            };
            // Referencias dischargePortReferences
            MapReferences(berman, vpc);

            return new VesselPortCallDeclaration() { VesselPortCall = vpc };
        }

        /// <summary>
        /// Procesa las "referencias" del BERMAN
        /// </summary>
        /// <param name="berman">BERMAN de PCS</param>
        /// <param name="vpc">Instancia VesselPortCall</param>
        private static void MapReferences(BERMAN berman, VesselPortCall vpc)
        {
            GovernalRequirementsGroup govRG;
            if (berman.TransportInformationGroup.PlaceLocationIdentificationGroup
                 .Any(x => x.PlaceLocationIdentification.PlaceLocationQualifier.Value == "64"))
            {
                vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                {
                    ReferenceType = "ZTC",
                    ReferenceComment = "Terminal of operation",
                    ReferenceValue = berman.TransportInformationGroup.PlaceLocationIdentificationGroup
                                        .FirstOrDefault(x => x.PlaceLocationIdentification.PlaceLocationQualifier.Value == "64")?
                                            .PlaceLocationIdentification.LocationIdentification.PlaceLocationIdentificationLine.Value
                });
            }

            govRG = berman.TransportInformationGroup.GovernalRequirementsGroup.FirstOrDefault(x => x.GovernmentalRequirements.TransportMovementCode.Value == "2");
            if (govRG != null)
            {
                var refer = govRG.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "ZNE");
                if (refer != null)
                {
                    vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                    {
                        ReferenceType = "ZRE",
                        ReferenceComment = "EU Regular Service",
                        ReferenceValue = refer.ReferenceIdentifier.Value
                    });
                }
                else
                {
                    refer = govRG.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "ZNR");
                    if (refer != null)
                    {
                        vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "ZNR",
                            ReferenceComment = "Non EU Regular Service"
                        });
                    }
                }

                refer = govRG.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "ZS1");
                if (refer != null)
                {
                    vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                    {
                        ReferenceType = "ZS1",
                        ReferenceComment = "EU Simplified Transit"
                    });
                }
                else
                {
                    refer = govRG.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "ZS2");
                    if (refer != null)
                    {
                        vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "ZS2",
                            ReferenceComment = "EU Simplified Transit"
                        });
                    }
                    else
                    {
                        refer = govRG.Reference.FirstOrDefault(x => x.ReferenceQualifier.Value == "ZS4");
                        if (refer != null)
                        {
                            vpc.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                            {
                                ReferenceType = "ZS4",
                                ReferenceComment = "Non EU Simplified Transit"
                            });
                        }
                    }
                }
            }
        }
    }
}
