using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
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
            try
            {
                // 1️ Guardar el RPT temporalmente
                if (req == null) return BadRequest("La solicitud llegó vacía.");
                string tempRptPath = Path.Combine(Path.GetTempPath(), "tempReport.rpt");
                File.WriteAllBytes(tempRptPath, req.ReportModel); // byte[] del RPT

                // 2️ Cargar el reporte
                ReportDocument rpt = new ReportDocument();
                rpt.Load(tempRptPath);

                // 3️ Convertir el string XML a DataSet
                DataSet ds = new DataSet();
                // Agrega System.Text al inicio si no está
                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(req.DataSetXml)))
                {
                    ds.ReadXml(ms, XmlReadMode.ReadSchema);
                }

                // 4️ Asignar DataSet al reporte
                rpt.SetDataSource(ds);

                // 5️ Exportar a PDF
                var pdfStream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                pdfStream.Position = 0; // Reiniciar stream

                // 6️ Preparar respuesta HTTP
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(pdfStream)
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
        }
    }
}