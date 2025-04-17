using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using WebApi_Files_Services.Class;

namespace WebApi_Files_Services.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DepositoController : ControllerBase
    {
        [HttpPost("Inventario")]
        public async Task<IActionResult> Make_inventario([FromBody] InventarioRequest request)
        {
            try
            {
                // Crear un documento PDF en memoria
                using (MemoryStream stream = new MemoryStream())
                {
                    string documento = "Inventario";
                    PdfDocument document = new PdfDocument();
                    document.Info.Title = documento;                   

                    // Crear instancia de Pdf_Maker
                    Pdf_Maker pdfMaker = new Pdf_Maker();
                    string fecha = DateTime.Now.ToString("dd-MM-yyyy");
                    
                    DataTable table = pdfMaker.ConvertToDataTable(request.DETALLE);
                    List<DataTable> listTables = pdfMaker.SplitDataTable(table,30);
                    int ind = 1;

                    foreach (DataTable dt in listTables)
                    {
                        PdfPage page = document.AddPage();
                        using (XGraphics gfx = XGraphics.FromPdfPage(page)) // Asegura la liberación de recursos gráficos
                        {
                            pdfMaker.AgregarEncabezado(gfx, request.BZCLNT, documento.ToUpper(), " ", fecha, "-", page.Width, 80);
                            pdfMaker.AgregarTabla(gfx, dt, 40, 130, "Arial", 12);
                            pdfMaker.AgregarParrafo(gfx, ind.ToString(), new XFont("Arial", 14, XFontStyleEx.Bold), new XPoint((page.Width - 60), (page.Height - 50)));
                        }
                        ind++;
                    }
                                        
                    // Guardar el PDF en el stream de memoria
                    document.Save(stream, false);
                    stream.Position = 0;

                    // Devolver el PDF como un archivo descargable
                    return File(stream.ToArray(), "application/pdf", "Inventario.pdf");
                }
            }
            catch (Exception ex) 
            {
                // Registra el error antes de devolver la respuesta
                Console.WriteLine($"Error al generar el PDF: {ex.Message}");
                return BadRequest("Ocurrió un error al generar el inventario PDF.");
            }

        }//cierra el controlador


    }//cierra la clase


    public class InventarioRequest
    {
        //public List<BzClient> BZCLNT { get; set; }  // Ahora es una lista     
        public BzClient BZCLNT { get; set; }
        public List<Dictionary<string, object>> DETALLE { get; set; }                

    }//cierra la clase


}//cierra el namespace
