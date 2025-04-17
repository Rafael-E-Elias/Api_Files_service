using Microsoft.AspNetCore.Mvc;
using WebApi_Files_Services.Service;

namespace WebApi_Files_Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacturaController : ControllerBase
    {
        private FacturaService service;

        public FacturaController(IConfiguration configuration)
        {
            this.service = new FacturaService(configuration);
        }

        [HttpPost]
        public async Task<IActionResult> Make_factura([FromBody] PresupuestoRequest request)
        {
            try
            {

                if (request.Equals(null))
                {
                    return BadRequest("Parameter can't be null");
                }

                double iva = 0.0; //hardcodeo para poder probar la funcionalidad

                string response = await Task.Run(() => this.service.Make_Factura_pdf(
                    request.BZCLNT,
                    request.NUMERO,
                    request.CLIENTE,
                    request.DETALLE,
                    request.TOTAL,
                    request.VALIDO,
                    request.OBSERVACION,
                    iva
                ));

                if (!string.IsNullOrEmpty(response))
                {
                    return Ok(new { path = response });
                }

                return BadRequest("Error al generar el presupuesto.");

            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        

        }//Cierra el endpoint


        [HttpPost("get")]
        public IActionResult Get_factura([FromBody] PathRequest path)
        {
            try
            {
                if (string.IsNullOrEmpty(path.Path))
                {
                    throw new ArgumentException("Parameter can't be null");
                }

                string full_path = this.service.Get_path_Factura(path.Path);

                if (!System.IO.File.Exists(full_path))
                {
                    return NotFound("File not found.");
                }

                var contentType = "application/pdf"; // Ajusta el tipo MIME según el archivo
                var fileName = Path.GetFileName(full_path);

                return PhysicalFile(full_path, contentType, fileName);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }//cierra el endpoint Get_factura


        [HttpPost("cobranza")]
        public async Task<IActionResult> Make_cobranza([FromBody] CobranzaRequest request)
        {
            try
            {
                if (request.Equals(null))
                {
                    return BadRequest("Parameter can't be null");
                }                

                string response = await Task.Run(() => this.service.Make_cobranza_pdf(
                    request.BZCLNT,
                    request.NUMERO,
                    request.CLIENTE,
                    request.DETALLE                    
                ));

                if (!string.IsNullOrEmpty(response))
                {
                    return Ok(new { path = response });
                }

                return BadRequest("Error al generar el presupuesto.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }//cierra el endpoint



    }//Cierra le controlador

}//Cierra el namespace
