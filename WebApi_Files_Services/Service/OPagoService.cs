using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Data;
using WebApi_Files_Services.Class;

namespace WebApi_Files_Services.Service
{
    public class OPagoService : PresupuestoService
    {
        
        public OPagoService(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Generar la OP pdf, la guarda y nos retorna la ruta hacia el pdf
        /// </summary>
        /// <param name="bzClient"></param>
        /// <param name="numero_OP">Numero de OP</param>
        /// <param name="proveedor">Datos del proveedor</param>
        /// <param name="detail"></param>
        /// <param name="subtotal"></param>
        /// <param name="observacion"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string Make_OP_pdf(BzClient bzClient, int numero_OP, Cliente proveedor, List<Dictionary<string, object>> detail, float total, string observacion,string fecha)
        {
            try
            {
                string documento = "Orden de Pago";

                if (File.Exists(Path.Combine(this.root, bzClient.EMPRESA, this._configuration["Path:OP:template"])))
                {
                    this.template = Path.Combine(this.root, bzClient.EMPRESA, this._configuration["Path:OP:template"]);
                }
                else
                {
                    this.template = Path.Combine(this.root, this._configuration["Path:template"]);
                }

                //Convierto el detalle en un DataTable para poder trabajar con Pdf_Maker
                DataTable detalle = this.Pdf_Maker.ConvertToDataTable(detail);

                //Defino el titulo del documento
                string titulo = documento.ToUpper();
                string Num_ = $"N° {string.Format("{0:0000}", numero_OP)}";

                // Cargar la plantilla en memoria
                PdfDocument templatePdf = PdfReader.Open(this.template, PdfDocumentOpenMode.Import);

                // Crear un nuevo documento PDF basado en la plantilla            
                PdfDocument nuevoPdf = new PdfDocument();

                int numero_pagina = 1;
                // Copiar cada página de la plantilla al nuevo documento
                foreach (PdfPage page in templatePdf.Pages)
                {
                    PdfPage nuevaPagina = nuevoPdf.AddPage(page);
                    XGraphics gfx = XGraphics.FromPdfPage(nuevaPagina);

                    //Genero los datos para el encabezado                    
                    string fecha_ = DateTime.Parse(fecha).ToString("dd-MM-yyyy");//hoy.ToString("dd-MM-yyyy");                    

                    // Superponer encabezado en la parte superior                
                    this.Pdf_Maker.AgregarEncabezado(gfx, bzClient, titulo, Num_, fecha_, "-", nuevaPagina.Width, 90);

                    this.Pdf_Maker.AgregarDatosProveedor(gfx, proveedor, 160);

                    this.Pdf_Maker.AgregarTabla(gfx, detalle, 40, 215, "Arial", 10);

                    //Coloco el footer de la factura
                    string sbTtl = $"TOTAL:$ {total}";                    

                    string obser = $"Obser.: {observacion}";
                    int Y = ((int)nuevaPagina.Height - 120);
                    int X = ((int)nuevaPagina.Width - 230);
                    
                    //coloco el subtotal
                    this.Pdf_Maker.AgregarParrafo(gfx, sbTtl, new XFont("Arial", 12), new XPoint(X, Y));                    
                    
                    //Coloco validez y observaciones
                    this.Pdf_Maker.AgregarParrafo(gfx, obser, new XFont("Arial", 12), new XPoint(40, (Y * 1.03)));
                    this.Pdf_Maker.AgregarParrafo(gfx, numero_pagina.ToString(), new XFont("Arial", 12), new XPoint((nuevaPagina.Width - 50), (nuevaPagina.Height - 20)));

                    numero_pagina++;
                }//cierra el foreach

                //verifico que exista la carpeta donde debo guardar el archivo
                string path_folder = Path.Combine(this.output, bzClient.EMPRESA, documento);
                if (!Directory.Exists(path_folder))
                {
                    Directory.CreateDirectory(path_folder); //Si no existe creo la carpeta
                }
                //Genero el path de retorno
                string path_bz = Path.Combine(bzClient.EMPRESA, documento, $"{string.Concat(titulo, " ", Num_)}.pdf");

                //combino el path de retorno con el root para obtener la ruta absoluta
                string path_File = Path.Combine(this.output, path_bz);

                // Guardar el nuevo PDF con las superposiciones
                nuevoPdf.Save(path_File);

                if (File.Exists(path_File))
                {
                    return path_bz;
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }//cierra el metodo

        public string Get_Path_OP(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException("path");    
                }

                string full_path = this.Get_path_presupuesto(path);                

                return full_path;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


    }//cierra la clase

}//cierra el namespace
