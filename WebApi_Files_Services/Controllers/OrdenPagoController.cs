using Microsoft.AspNetCore.Mvc;
using WebApi_Files_Services.Service;

namespace WebApi_Files_Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdenPagoController : ControllerBase
    {
        private OPagoService pagoService;
        public OrdenPagoController(IConfiguration configuration) 
        { 
            
            this.pagoService = new OPagoService(configuration);

        }//cierra el constructor


        [HttpPost]
        public async Task<IActionResult> Make_OP([FromBody] OPagoRequest request)
        {
            try
            {
                if(request == null)
                {
                    throw new ArgumentNullException("Parameter can't be null");
                }

                string response = await Task.Run(() => this.pagoService.Make_OP_pdf(
                    request.BZCLNT[0],
                    request.NUMERO,
                    request.PROVEEDOR[0],
                    request.DETALLE,
                    request.TOTAL,                    
                    request.OBSERVACION,
                    request.FECHA
                ));

                if (!string.IsNullOrEmpty(response))
                {
                    return Ok(new { path = response });
                }

                return BadRequest("Error al generar la orden de pago.");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }//cierra el controlador


        [HttpPost("get")]
        public IActionResult Get_OP(PathRequest path)
        {
            try
            {
                if (string.IsNullOrEmpty(path.Path))
                {
                    throw new ArgumentNullException("Path value can´t be null");
                }

                string path_OP = this.pagoService.Get_Path_OP(path.Path);

                var contentType = "application/pdf"; // Ajusta el tipo MIME según el archivo
                var fileName = Path.GetFileName(path_OP);

                return PhysicalFile(path_OP, contentType, fileName);

            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

        }//cierra el controlador


    }//cierra la clase

}//cierra el namespace
