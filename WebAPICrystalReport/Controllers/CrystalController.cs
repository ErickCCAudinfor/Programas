using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace WebAPICrystalReport.Controllers
{
    public class CrystalController : ApiController
    {
        [HttpPost]
        [Route("Crystal/GenerarPdf")]
        public IHttpActionResult GenerarPdf(CrystalRequest req)
        {
            // Si el cuerpo no se puede leer (tamaño, JSON mal formado...) Web API deja req a null y el motivo en ModelState
            if (req == null)
            {
                string motivo = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.Exception != null ? e.Exception.Message : e.ErrorMessage));
                return BadRequest("La solicitud llegó vacía." + (string.IsNullOrEmpty(motivo) ? "" : " Motivo: " + motivo));
            }

            // Un .rpt temporal por peticion: con un nombre fijo, dos impresiones a la vez se pisaban y la segunda
            // encontraba el fichero bloqueado por la primera
            string tempRptPath = Path.Combine(Path.GetTempPath(), "CrystalApi_" + Guid.NewGuid().ToString("N") + ".rpt");
            ReportDocument rpt = new ReportDocument();
            try
            {
                // 1 Guardar el RPT temporalmente y cargarlo
                File.WriteAllBytes(tempRptPath, req.ReportModel);
                rpt.Load(tempRptPath);

                // 2 Convertir el string XML a DataSet
                DataSet ds = new DataSet();
                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(req.DataSetXml)))
                {
                    ds.ReadXml(ms, XmlReadMode.ReadSchema);
                }
                QuitarClavesPrimarias(ds);

                // 3 Asignar el DataSet al principal y a cada subinforme (como SigeReportHelper.PropagateToSections)
                rpt.SetDataSource(ds);
                foreach (ReportDocument sub in rpt.Subreports)
                {
                    sub.SetDataSource(ds);
                }

                // 4 Parametros: despues del SetDataSource, que los reinicia
                AplicarParametros(rpt, req.Parameters ?? new Dictionary<string, object>());

                // 5 Exportar a PDF (se copia a byte[] para poder cerrar el report antes de responder)
                byte[] pdf;
                using (Stream pdfStream = rpt.ExportToStream(ExportFormatType.PortableDocFormat))
                using (var copia = new MemoryStream())
                {
                    pdfStream.CopyTo(copia);
                    pdf = copia.ToArray();
                }

                // 6 Preparar respuesta HTTP
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdf)
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Reporte.pdf"
                };

                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                try { rpt.Close(); rpt.Dispose(); } catch { }
                try { if (File.Exists(tempRptPath)) File.Delete(tempRptPath); } catch { }
            }
        }

        // Crystal no se lleva bien con las claves primarias del DataSet (igual que SigeReportHelper.AssignDataSet)
        private static void QuitarClavesPrimarias(DataSet ds)
        {
            foreach (DataTable tabla in ds.Tables)
            {
                foreach (DataColumn columna in tabla.PrimaryKey)
                {
                    columna.AutoIncrement = false;
                }
                try { tabla.PrimaryKey = null; } catch { }
            }
        }

        // Los parametros que manda SIGE se convierten al tipo del parametro del report (en el JSON pueden llegar como
        // texto: "True", "8"...). Los que no llegan se rellenan con un valor neutro (texto vacio, 0, false, hoy), como
        // hace la vista previa, para que Crystal no pare con "Faltan los valores del parametro". Los vinculados a un
        // subinforme se los pone Crystal.
        private static void AplicarParametros(ReportDocument rpt, Dictionary<string, object> parametros)
        {
            foreach (ParameterFieldDefinition p in rpt.DataDefinition.ParameterFields)
            {
                if (p.IsLinked()) continue;

                object valor;
                object recibido;
                if (parametros.TryGetValue(p.Name, out recibido) && recibido != null)
                {
                    valor = Convertir(recibido, p.ParameterValueKind);
                }
                else
                {
                    valor = ValorPorDefecto(p.ParameterValueKind);
                }

                if (string.IsNullOrEmpty(p.ReportName))
                    rpt.SetParameterValue(p.Name, valor);
                else
                    rpt.SetParameterValue(p.Name, valor, p.ReportName);
            }
        }

        private static object Convertir(object valor, ParameterValueKind tipo)
        {
            try
            {
                switch (tipo)
                {
                    case ParameterValueKind.BooleanParameter:
                        return Convert.ToBoolean(valor, CultureInfo.InvariantCulture);
                    case ParameterValueKind.NumberParameter:
                    case ParameterValueKind.CurrencyParameter:
                        return Convert.ToDecimal(valor, CultureInfo.InvariantCulture);
                    case ParameterValueKind.DateParameter:
                    case ParameterValueKind.DateTimeParameter:
                        return Convert.ToDateTime(valor, CultureInfo.InvariantCulture);
                    case ParameterValueKind.TimeParameter:
                        return Convert.ToDateTime(valor, CultureInfo.InvariantCulture);
                    default:
                        return Convert.ToString(valor, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                return ValorPorDefecto(tipo);
            }
        }

        private static object ValorPorDefecto(ParameterValueKind tipo)
        {
            switch (tipo)
            {
                case ParameterValueKind.BooleanParameter: return false;
                case ParameterValueKind.NumberParameter:
                case ParameterValueKind.CurrencyParameter: return 0m;
                case ParameterValueKind.DateParameter:
                case ParameterValueKind.DateTimeParameter:
                case ParameterValueKind.TimeParameter: return DateTime.Today;
                default: return string.Empty;
            }
        }
    }
}
