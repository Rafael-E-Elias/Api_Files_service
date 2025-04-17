using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Data;
using WebApi_Files_Services.Class;

namespace WebApi_Files_Services.Service
{
    public class FacturaService : PresupuestoService
    {

        public FacturaService(IConfiguration configuration) : base(configuration)
        {
            
        }

        public string Make_Factura_pdf(BzClient bzClient, int numero_pres, Cliente cliente, List<Dictionary<string, object>> detail, float subtotal, int valido, string observacion, double iva)
        {
            try
            {
                string documento = "Factura";

                if (File.Exists(Path.Combine(this.root, bzClient.EMPRESA, this._configuration["Path:factura:template"])))
                {
                    this.template = Path.Combine(this.root, bzClient.EMPRESA, this._configuration["Path:factura:template"]);
                }
                else
                {
                    this.template = Path.Combine(this.root, this._configuration["Path:template"]);
                }

                //Convierto el detalle en un DataTable para poder trabajar con Pdf_Maker
                DataTable detalle = this.Pdf_Maker.ConvertToDataTable(detail);
                List<DataTable> List_table = this.Pdf_Maker.SplitDataTable(detalle,22);


                //Defino el titulo del documento
                string titulo = documento.ToUpper();
                string Num_ = $"N° {string.Format("{0:0000}", numero_pres)}";

                // Cargar la plantilla en memoria
                PdfDocument templatePdf = PdfReader.Open(this.template, PdfDocumentOpenMode.Import);

                // Crear un nuevo documento PDF basado en la plantilla            
                PdfDocument nuevoPdf = new PdfDocument();
                int total_paginas = List_table.Count;
                int numero_pagina = 1;

                // Copiar cada página de la plantilla al nuevo documento
                for (int i = 0; i < List_table.Count; i++)
                {
                    PdfPage plantilla = templatePdf.Pages[0]; // Siempre usamos la primera página como base
                    PdfPage nuevaPagina = nuevoPdf.AddPage(plantilla);

                    using (XGraphics gfx = XGraphics.FromPdfPage(nuevaPagina))
                    {
                        //Genero los datos para el encabezado
                        DateTime hoy = DateTime.Now;
                        string fecha = hoy.ToString("dd-MM-yyyy");
                        string validez = hoy.AddDays(valido).ToString("dd-MM-yyyy");

                        // Superponer encabezado en la parte superior                
                        this.Pdf_Maker.AgregarEncabezado(gfx, bzClient, titulo, Num_, fecha, validez, nuevaPagina.Width, 90);

                        this.Pdf_Maker.AgregarDatosCliente(gfx, cliente, 160);

                        this.Pdf_Maker.AgregarTabla(gfx, List_table[i], 40, 215, "Arial", 10);

                        this.Pdf_Maker.AgregarParrafo(gfx, numero_pagina.ToString(), new XFont("Arial", 12), new XPoint((nuevaPagina.Width - 50), (nuevaPagina.Height - 20)));

                        // Si es la última página, agregamos observaciones y totales
                        if (i == total_paginas - 1)
                        {
                            double SubTotal = subtotal;
                            double _iva = 0;
                            double _Total = subtotal;

                            if (iva > 0.0)
                            {
                                SubTotal = (subtotal * (1 - iva));
                                _iva = (subtotal * iva);
                                _Total = (SubTotal + _iva);
                            }
                            //Coloco el footer de la factura
                            string sbTtl = $"SUBTOTAL: $ {SubTotal.ToString("0.##")}";
                            string IVA = (iva > 0.0) ? $"IVA: $ {(_iva).ToString("0.##")}" : "";
                            string Total = $"TOTAL: $ {_Total.ToString("0.##")}";

                            string obser = $"Obser.: {observacion}";
                            int Y = ((int)nuevaPagina.Height - 120);
                            int X = ((int)nuevaPagina.Width - 230);
                            //coloco el subtotal
                            this.Pdf_Maker.AgregarParrafo(gfx, sbTtl, new XFont("Arial", 12), new XPoint(X, Y));
                            //coloco el IVA
                            this.Pdf_Maker.AgregarParrafo(gfx, IVA, new XFont("Arial", 12), new XPoint((X + 44), (Y * 1.03)));
                            //coloco el total
                            this.Pdf_Maker.AgregarParrafo(gfx, Total, new XFont("Arial", 12), new XPoint((X + 25), (Y * 1.06)));
                            //Coloco validez y observaciones
                            this.Pdf_Maker.AgregarParrafo(gfx, obser, new XFont("Arial", 12), new XPoint(40, (Y * 1.03)));

                        }//cierra el if
                                                
                        
                    }//cierra el using

                    numero_pagina++;

                }//cierra el for
                /*
                    foreach (PdfPage page in templatePdf.Pages)
                {
                    PdfPage nuevaPagina = nuevoPdf.AddPage(page);
                    XGraphics gfx = XGraphics.FromPdfPage(nuevaPagina);

                    //Genero los datos para el encabezado
                    DateTime hoy = DateTime.Now;
                    string fecha = hoy.ToString("dd-MM-yyyy");
                    string validez = hoy.AddDays(valido).ToString("dd-MM-yyyy");

                    // Superponer encabezado en la parte superior                
                    this.Pdf_Maker.AgregarEncabezado(gfx, bzClient, titulo, Num_,fecha, validez, nuevaPagina.Width, 90);

                    this.Pdf_Maker.AgregarDatosCliente(gfx, cliente, 160);

                    this.Pdf_Maker.AgregarTabla(gfx, detalle, 40, 215, "Arial", 10);

                    //Coloco el footer de la factura
                    string sbTtl = $"SUBTOTAL:$ {subtotal}";
                    string IVA = $"IVA:$ {(subtotal * iva).ToString("0.##")}";
                    string Total = $"TOTAL:$ {(subtotal * (1 + iva)).ToString("0.##")}";
                    
                    string obser = $"Obser.: {observacion}";
                    int Y = ((int)nuevaPagina.Height - 120);
                    int X = ((int)nuevaPagina.Width - 230);
                    //coloco el subtotal
                    this.Pdf_Maker.AgregarParrafo(gfx, sbTtl, new XFont("Arial", 12), new XPoint(X, Y));
                    //coloco el IVA
                    this.Pdf_Maker.AgregarParrafo(gfx, IVA, new XFont("Arial", 12), new XPoint((X + 44), (Y * 1.03)));
                    //coloco el total
                    this.Pdf_Maker.AgregarParrafo(gfx, Total, new XFont("Arial", 12), new XPoint((X + 25), (Y * 1.06)));
                    //Coloco validez y observaciones
                    this.Pdf_Maker.AgregarParrafo(gfx, obser, new XFont("Arial", 12), new XPoint(40, (Y * 1.03)));
                    this.Pdf_Maker.AgregarParrafo(gfx, numero_pagina.ToString(), new XFont("Arial", 12), new XPoint((nuevaPagina.Width - 50), (nuevaPagina.Height - 20)));

                    numero_pagina++;
                }//cierra el foreach
                */

                //verifico que exista la carpeta donde debo guardar el archivo
                string path_folder = Path.Combine(this.output, bzClient.EMPRESA, documento);
                if (!Directory.Exists(path_folder))
                {
                    Directory.CreateDirectory(path_folder); //Si no existe creo la carpeta
                }
                //Genero el path de retorno
                string path_bz = Path.Combine(bzClient.EMPRESA, documento, $"{string.Concat(titulo," ", Num_)}.pdf");

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

        public string Get_path_Factura(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException("path");
                }

                string full_path = this.Get_path_presupuesto(path);
                                
                return full_path;                


            }catch(Exception ex)
            {
                throw ex;
            }

        }//cierra el metodo Get_path_Factura

        public string Make_cobranza_pdf(BzClient bzClient,int numero,Cliente cliente, List<Dictionary<string, object>> detail)
        {
            try
            {

                string documento = "Recibo de cobranza";

                if (File.Exists(Path.Combine(this.root, bzClient.EMPRESA, this._configuration["Path:cobranza:template"])))
                {
                    this.template = Path.Combine(this.root, bzClient.EMPRESA, this._configuration["Path:cobranza:template"]);
                }
                else
                {
                    this.template = Path.Combine(this.root, this._configuration["Path:template"]);
                }

                //Convierto el detalle en un DataTable para poder trabajar con Pdf_Maker
                DataTable detalle = this.Pdf_Maker.ConvertToDataTable(detail);               

                //Defino el titulo del documento
                string titulo = documento.ToUpper();
                string Num_ = $"N° {string.Format("{0:0000}", numero)}";

                // Cargar la plantilla en memoria
                PdfDocument templatePdf = PdfReader.Open(this.template, PdfDocumentOpenMode.Import);

                // Crear un nuevo documento PDF basado en la plantilla            
                PdfDocument nuevoPdf = new PdfDocument();               
                
                    PdfPage plantilla = templatePdf.Pages[0]; // Siempre usamos la primera página como base
                    PdfPage nuevaPagina = nuevoPdf.AddPage(plantilla);

                    using (XGraphics gfx = XGraphics.FromPdfPage(nuevaPagina))
                    {
                        //Genero los datos para el encabezado
                        DateTime hoy = DateTime.Now;
                        string fecha = hoy.ToString("dd-MM-yyyy");                        

                        // Superponer encabezado en la parte superior                
                        this.Pdf_Maker.AgregarEncabezado(gfx, bzClient, titulo, Num_, fecha, "-", nuevaPagina.Width, 90);

                        this.Pdf_Maker.AgregarDatosCliente(gfx, cliente, 160);

                        this.Pdf_Maker.AgregarTabla(gfx, detalle, 40, 215, "Arial", 10);                        

                    }//cierra el using                    
                                    
                
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


    }//cierra la clase

}//cierra el namespace
