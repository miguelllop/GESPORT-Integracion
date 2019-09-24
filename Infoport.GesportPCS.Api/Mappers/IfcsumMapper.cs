using Gesport.Middleware.Api;
using Infoport.GesportPCS.Api.Models;
using Infoport.GesportPCS.Api.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Infoport.GesportPCS.Api.Mappers
{
    /// <summary>
    /// Clase estática de mapeado de BLs en IFCSUM de PCS a BillOfLadingDeclaration
    /// </summary>
    public static class IfcsumMapper
    {
        /// <summary>
        /// Mapea los BLs en IFCSUM de PCS a BillOfLadingDeclaration de Gesport4.0 haciendo uso también de
        /// la información de la escala obtenida de PCS
        /// </summary>
        /// <param name="dataset">IFCSUM de PCS</param>
        /// <param name="vpcInfo">Datos de escala</param>
        /// <returns></returns>
        public static BillOfLadingDeclaration Map(IfcsumDataset dataset, VesselPortCallInfo vpcInfo)
        {
            var blDec = new BillOfLadingDeclaration();
            if (dataset == null || dataset.Items.Count == 0 )
                throw new ArgumentException("BL es nulo o no tiene declaraciones");
            if (vpcInfo == null)
                throw new ArgumentException("No hay información de la escala");
            var sumDec = new ShippingAgentDeclaration();

            var sumaria = dataset.Items.SingleOrDefault() as IfcsumDatasetDeclarations;
            if (sumaria != null)
            {
                sumDec.Bls = new List<BlDeclaration>();
                sumDec.SummaryDeclarationNumber = sumaria.SummaryDeclaration;
                sumDec.ShippingAgent = new Party()
                {
                    Office = new Code() { Code1 = sumaria.DeclaringAgent },
                    OfficeId = $"{sumaria.DeclaringAgent}@ESVLC",
                    Organization = new Code() { Code1 = "ESVLC" }
                };
                sumDec.PortCallId = $"{vpcInfo.PortCall}@{Constantes.PortLocation[sumaria.PortCallReference.Substring(0,1)]}";
                sumDec.DischargeTerminalOperator = new Party()
                {
                    Organization = new Code() { Code1 = sumaria.TerminalOperator},
                    OfficeId = $"{sumaria.TerminalOperator}@ESVLC",
                    Office = new Code() { Code1 = sumaria.TerminalOperator}
                };
                foreach(var consig in sumaria.Consignments)
                {
                    var bl = new BlDeclaration()
                    {
                        BlNumber = consig.BLNumber,
                        Carrier = new Party()
                        {

                            //Organization = new Code() { Code1 = vpcInfo.VesselAgentCif, Name = vpcInfo.VesselAgentName },
                            Organization = new Code() { Code1 = "MSCU", Name = vpcInfo.VesselAgentName },
                            Address = new Address() { Street = vpcInfo.VesselAgentAddress },
                            //OfficeId = $"{vpcInfo.VesselAgentCif}@ESVLC",
                            Office = new Code() { Code1 = "ESVLC", Name = "" },
                            Phone = vpcInfo.VesselAgentPhone
                        }, // TODO: Obtener Naviera (consignataria de buque)
                        ShippingAgent = sumDec.ShippingAgent,
                        Vessel = new Code() { Code1 = vpcInfo.IMO, Name = vpcInfo.Vessel }, // TODO: Obtener de la escala
                        //Flag = new Code() {Code1 = vpcInfo., // TODO: Obtener de la escala
                        VoyageNumber = vpcInfo.Voyage, // TODO: Confirmar que es correcto este dato
                        PlaceOfOrigin = new Code() { Code1 = consig.PlaceOfOrigin}, // TODO: Incluir descripción
                        PortOfLoading = new Code() { Code1 = consig.PortOfLoading}, // TODO: Incluir descripción
                        PortOfTranshipment = new Code() { Code1 = consig.PortOfTranshipment}, // TODO: Incluir descripción
                        PortOfDischarge = new Code() { Code1 = Constantes.PortLocation[sumaria.PortCallReference.Substring(0, 1)], Name = vpcInfo.PortOfCallName }, // TODO: Obtener de la escala
                        PlaceOfDelivery = new Code() { Code1 = consig.PlaceOfDestination }, // TODO: Incluir descripción
                        SubsequentTransportMode = consig.SecondaryTransportType == "30" ? new Code() { Code1 = consig.SecondaryTransportMode} : null, // TODO: Incluir descripción
                        DischargeBerth = new Code() { Code1 = sumaria.ServerAssignedCode }, // TODO: Confirmar ¿Es el tramo?
                        DischargePortReferences = null, // TODO: Consultar qué se hace con esto
                        DischargeTerminalOperator = null // TODO: Consultar qué se hace con esto
                    };
                    #region GoodsItems
                    bl.GoodsItems = new List<GoodsItem>();
                    foreach (var gi in consig.GoodsItems)
                    {
                        var gItem = new GoodsItem()
                        {
                            GoodsItemNumber = gi.GoodsItemNumber,
                            GoodsDescription = new Code() { Code1 = gi.TaricCode }, // TODO: Se dispone de un TARIC code. Faltaría descripción
                            MarksAndNumbers = "NO Disponible", // TODO: No disponible, a consultar
                            GrossWeight = gi.GrossWeight.ToString(CultureInfo.InvariantCulture) // TODO: Confirmar formato de decimales  
                        };
                        // Documentos (references) de la mercancía
                        gItem.GoodsReferences = new List<Gesport.Middleware.Api.Reference>();
                        foreach (var cr in gi.GoodsDocuments)
                        {
                            var custRef = new Gesport.Middleware.Api.Reference()
                            {
                                ReferenceValue = cr.DocumentNumber,
                                ReferenceStatus = cr.DocumentStatus,
                                ReferenceDate = cr.DocumentDate.ToString(), // TODO: Revisar formato fecha/hora
                                ReferenceType = cr.DocumentType
                            };
                            gItem.GoodsReferences.Add(custRef);
                        }
                        // Packages
                        gItem.Packages = new List<Package>();
                        foreach (var gd in gi.GoodsDistribution)
                        {
                            var pkg = new Package()
                            {
                                PackageItem = new Code() { Code1 = gd.EquipmentId }, // TODO: Validar que este es el valor a guardar
                                PackageNumber = Convert.ToInt32(gd.PackageNumber),
                                PackageType = new Code() { Code1 = gi.PackageType }
                            };
                            gItem.Packages.Add(pkg);
                        }
                        // Mercancías peligrosas
                        gItem.DangerousGoods = new List<DangerousGoodsItem>(){new DangerousGoodsItem {
                            UndgClass = gi.IMDGClass,
                            Undg = new Code() { Code1 = gi.UNDGNumber }                            
                            }
                        };

                        // Campos pendientes
                        gItem.NetWeight = null; // TODO: No disponible
                        gItem.CustomsReferences = null; // TODO: Ver de donde se saca la info para este campo

                        // Añadimos el goodItem
                        bl.GoodsItems.Add(gItem);
                    }
                    #endregion
                    #region Equipments/Contenedores
                    bl.Containers = new List<ContainerItem>();
                    foreach (var eq in consig.Equipments)
                    {
                        var cnt = new ContainerItem()
                        {
                            BlNumber = bl.BlNumber,
                            //BookingNumber = "", // TODO: No disponible
                            Carrier = bl.Carrier,
                            ShippingAgent = bl.ShippingAgent,
                            // CustomsReleased = // TODO: No disponible
                            // ExpectedTime = // TODO: No disponible
                            // GrossWeight = // TODO: No disponible
                            // OperationTime = // TODO: No disponible
                            // ShipmentClause = // TODO: Averiguar lo que es
                            SubsequentTransportMode = new Code() { Code1 = consig.SecondaryTransportMode }, // TODO: Confirmar que este es el valor
                            TerminalOperator = sumDec.DischargeTerminalOperator , // TODO: Ver de dónde sacar este valor
                            ContainerNumber = eq.EquipmentId,
                            ContainerType = new Code() { Code1 = eq.ContainerType},
                            ContainerStatus = eq.FullEmptyIndicator.Equals("4") ? ContainerItemContainerStatus.EMPTY : ContainerItemContainerStatus.FULL,
                            TareWeight = eq.TareWeight.ToString(CultureInfo.InvariantCulture) // TODO: Establecer formato
                        };
                        // Sellos
                        cnt.Seals = eq.Seals.Select(x => new Seal() { Number = x.SealNumber }).ToList(); // TODO: Averiguar qué se pone en Owner
                        // GoodsItems
                        var gins = consig.GoodsItems.Where(x => x.GoodsDistribution.Any(g => g.EquipmentId == cnt.ContainerNumber));//?.Select(s => s.GoodsItemNumber);
                        cnt.GoodsItems = bl.GoodsItems.Where(x => gins.Any(a => a.GoodsItemNumber == x.GoodsItemNumber)).ToList();
                        bl.Containers.Add(cnt);
                    }

                    #endregion

                    // Añadimos la BL
                    sumDec.Bls.Add(bl);
                }
                blDec.Declaration = sumDec;
                sumDec.DischargePortReferences = new List<Gesport.Middleware.Api.Reference>();
                switch (sumaria.TransportMovement)
                {
                    case "2":
                        sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "GOR",
                            ReferenceComment = "With Discharge",
                            ReferenceValue = sumaria.TransportMovement
                        });
                        break;
                    case "ZDL":
                        sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "GOR",
                            ReferenceComment = "Ship in ballast",
                            ReferenceValue = sumaria.TransportMovement
                        });
                        break;
                    case "ZSD":
                        sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "GOR",
                            ReferenceComment = "Without Discharge",
                            ReferenceValue = sumaria.TransportMovement
                        });
                        break;
                }
                sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                {
                    ReferenceType = "ZLS",
                    ReferenceComment = "Shipping service",
                    ReferenceValue = sumaria.ServiceCode
                });
                sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                {
                    ReferenceType = "ZTC",
                    ReferenceComment = "Operation terminal",
                    ReferenceValue = sumaria.CustomsLocation
                });
                sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                {
                    ReferenceType = "ZRS",
                    ReferenceComment = "Simplified port tax",
                    ReferenceValue = (!string.IsNullOrEmpty(sumaria.SimplifiedRegime) && sumaria.SimplifiedRegime.Equals("ZRS")) ? "SI" : "NO"
                });
                if (sumaria.RegularService.Equals("ZNR"))
                {
                    sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                    {
                        ReferenceType = "ZNR",
                        ReferenceComment = "Regular service"
                    });
                }
                else
                {
                    if (sumaria.RegularService.Equals("ZNR"))
                    {
                        sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "ZNR",
                            ReferenceComment = "Non EU regular service"
                        });
                    }
                }
                switch (sumaria.TransitProcedure)
                {
                    case "ZS1":
                    case "ZS2":
                        sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = sumaria.TransitProcedure,
                            ReferenceComment = "EU simplified transit"
                        });
                        break;
                    case "ZS4":
                        sumDec.DischargePortReferences.Add(new Gesport.Middleware.Api.Reference()
                        {
                            ReferenceType = "ZNR",
                            ReferenceComment = "Non EU simplified transit"
                        });
                        break;
                }
            }

            return blDec;
        }
    }
}
