using Microsoft.AspNetCore.Mvc;
using WebApi_Files_Services.Service;
using System.Data;
using WebApi_Files_Services.Class;



namespace WebApi_Files_Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresupuestoController : ControllerBase
    {
        private PresupuestoService service;

        public PresupuestoController(IConfiguration configuration) 
        {
            this.service = new PresupuestoService(configuration);
        }

        [HttpGet]
        public IActionResult Get_Presupuesto()
        {
            return Ok("WebApi Online");

        }//cierra la funcion

        [HttpPost]
        public async Task<IActionResult> Make_Presupuesto([FromBody] PresupuestoRequest request)
        {
            try
            {
                if (request.Equals(null))
                {
                    throw new ArgumentException("Parameter can't be null");
                }

                double iva = 0.0;

                string response = await Task.Run(() => this.service.Make_presupuesto_pdf(
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

            }catch(Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }

            
        }//cierra la funcion 

        [HttpPost("get")]        
        public IActionResult Get_presupuesto([FromBody] PathRequest path)
        {
            try
            {
                if (string.IsNullOrEmpty(path.Path))
                {
                    throw new ArgumentNullException("The path can´t be empty or null");
                }

                string full_path = this.service.Get_path_presupuesto(path.Path);

                if (!System.IO.File.Exists(full_path))
                {
                    return NotFound("File not found.");
                }

                var contentType = "application/pdf"; // Ajusta el tipo MIME según el archivo
                var fileName = Path.GetFileName(full_path);

                return PhysicalFile(full_path, contentType, fileName);



            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            



        }//cierra el controlador Get_presupuesto

    }//cierra la clase

    public class PresupuestoRequest
    {
        public int NUMERO { get; set; }
        public BzClient BZCLNT { get; set; }
        public Cliente CLIENTE { get; set; }
        public List<Dictionary<string,object>> DETALLE { get; set; }
        public float TOTAL {  get; set; }
        public string OBSERVACION { get; set; }
        public int VALIDO { get; set; }        
        

    }//cierra la clase

    public class PathRequest
    {
        public string Path { get; set; }

    }//cierra la clase

    public class OPagoRequest
    {
        public int NUMERO { get; set; }
        public List<BzClient> BZCLNT { get; set; }  // Ahora es una lista
        public List<Proveedor> PROVEEDOR { get; set; }  // Ahora es una lista
        public List<Dictionary<string, object>> DETALLE { get; set; }
        public float TOTAL { get; set; }
        public string OBSERVACION { get; set; }         
        public string FECHA { get; set; }
               
        
    }//cierra la clase

    public class CobranzaRequest
    {
        public int NUMERO { get; set; }
        public BzClient BZCLNT { get; set; }
        public Cliente CLIENTE { get; set; }
        public List<Dictionary<string, object>> DETALLE { get; set; }
        //public float TOTAL { get; set; }
        //public string OBSERVACION { get; set; }
        //public int VALIDO { get; set; }

    }//cierra la clase

}//cierra el namespace
