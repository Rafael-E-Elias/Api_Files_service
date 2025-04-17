using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using System.Runtime.CompilerServices;
using PdfSharp.Drawing;
using static System.Collections.Specialized.BitVector32;
using System.Data;
using System.Reflection;


namespace WebApi_Files_Services.Class
{
    public class Pdf_Maker
    {
        private string plantillaPath; // Ruta de la plantilla  = "plantilla.pdf"
        private string outputPath; // Ruta de salida  = "output.pdf"

        public Pdf_Maker()
        {

        }//cierra el constructor

        public Pdf_Maker(string template_path,string folder_output,string document_name)
        {
            string full_path = $"{document_name}.pdf";
            this.plantillaPath = template_path;
            this.outputPath = Path.Combine(folder_output,full_path);
        }
                       
                       
        /// <summary>
        /// dibuja un parrafo en el punto (point) pasado
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="titulo"></param>
        /// <param name="font"></param>
        /// <param name="point"></param>
        public void AgregarParrafo(XGraphics gfx, string titulo, XFont font, XPoint point)
        {   
            
            gfx.DrawString(titulo, font, XBrushes.Black, point, XStringFormats.Default); 

        }//Cierra el metodo AgregarParrafo

        
        /// <summary>
        /// Dibuja el titulo del documento ejemplo Presupuesto N° 47
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="titulo"></param>
        /// <param name="pageWidth"></param>
        /// <param name="font"></param>
        public void AgregarTitulo(XGraphics gfx, string titulo, double pageWidth, XFont font)
        {
            
            gfx.DrawString(titulo, font, XBrushes.Black, new XPoint((pageWidth / 2), 15), XStringFormats.TopCenter);

        }//cierra el metodo


        /// <summary>
        /// Dibuja un encabezado en la parte superior de la página.
        /// </summary>
        public void AgregarEncabezado(XGraphics gfx,BzClient bzClient, string documento,string Num_,string fecha_hoy, string fecha_venc, double pageWidth,int y)
        {
            XFont font_ = new XFont("Arial", 17, XFontStyleEx.Bold);
            XFont font = new XFont("Arial", 13, XFontStyleEx.Regular);
            XFont fnt = new XFont("Arial", 13, XFontStyleEx.Bold);

            double Y = (pageWidth / 1.3);
            string fecha = $"Fecha: {fecha_hoy}";
            string validez = $"Venci.: {fecha_venc}";
            //Agrego el tipo de documento
            this.AgregarTitulo(gfx, documento, pageWidth, font_);

            //Agrego el numero de documento            
            this.AgregarParrafo(gfx, Num_, fnt, new XPoint(Y, 30));
            //Agrego la fecha del documento
            this.AgregarParrafo(gfx, fecha, fnt,new XPoint(Y, 45));
            //Agrego la fecha de vencimiento
            this.AgregarParrafo(gfx, validez, fnt, new XPoint(Y, 60));


            //agrego los datos de la empresa
            string empr = bzClient.EMPRESA;
            string dir = $"Dir: {bzClient.DIRECCION}";
            string cont = $"Tel: {bzClient.TELEFONO}  -  Mail: {bzClient.MAIL}";

            this.AgregarParrafo(gfx, empr, new XFont("Arial", 20, XFontStyleEx.Regular), new XPoint(20, (y * 1)));
            this.AgregarParrafo(gfx, dir, font, new XPoint(20, (y * 1.2)));
            this.AgregarParrafo(gfx, cont, font, new XPoint(20, (y * 1.4)));

        }//Cierra el metodo AgregarEncabezado


        /// <summary>
        /// Dibuja los datos del cliente al documento desde el punto y
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="cliente"></param>
        public void AgregarDatosCliente(XGraphics gfx, Cliente cliente,int y)
        {
            XFont font = new XFont("Arial", 13, XFontStyleEx.Regular);            

            string nombre = $"Cliente: {cliente.NOMBRE}";
            string ident = $"Ident.: {cliente.IDENT}";
            string tel = $"Dir.: {cliente.DIRECCION}";
            string mail = $"Te.: {cliente.TELEFONO}";

            this.AgregarParrafo(gfx, nombre, font, new XPoint(20, y));
            this.AgregarParrafo(gfx, ident, font, new XPoint(20, (y * 1.1)));
            this.AgregarParrafo(gfx, tel +"  "+mail, font, new XPoint(20, (y * 1.2)));            

        }//cierra el metodo AgregarDatosCliente


        public void AgregarDatosProveedor(XGraphics gfx, Cliente cliente, int y)
        {
            XFont font = new XFont("Arial", 13, XFontStyleEx.Regular);

            string nombre = $"Proveedor: {cliente.NOMBRE}";
            string ident = $"Ident.: {cliente.IDENT}";
            string tel = $"Dir.: {cliente.DIRECCION}";
            string mail = $"Te.: {cliente.TELEFONO}";

            this.AgregarParrafo(gfx, nombre, font, new XPoint(20, y));
            this.AgregarParrafo(gfx, ident, font, new XPoint(20, (y * 1.1)));
            this.AgregarParrafo(gfx, tel + "  " + mail, font, new XPoint(20, (y * 1.2)));

        }//cierra el metodo AgregarDatosCliente


        /// <summary>
        /// Dibuja una tabla en el archivo con los datos pasados en el DataTable
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="datos"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AgregarTabla(XGraphics gfx, DataTable datos, double x, double y,string font,double font_size)
        {
            XFont headerFont = new XFont(font, font_size, XFontStyleEx.Bold);
            XFont font_ = new XFont(font, font_size, XFontStyleEx.Regular);
            double rowHeight = 20;
            double colWidth = 100;

            XBrush headerBackground = new XSolidBrush(XColors.LightGray); // Color de fondo para la cabecera
            XBrush rowBackground1 = new XSolidBrush(XColors.WhiteSmoke);       // Color para filas pares
            XBrush rowBackground2 = new XSolidBrush(XColors.LightBlue); // Color para filas impares

            // Dibujar fondo de la cabecera
            gfx.DrawRectangle(headerBackground, x, y, datos.Columns.Count * colWidth, rowHeight);

            // Dibujar los nombres de las columnas sobre el fondo
            for (int colIndex = 0; colIndex < datos.Columns.Count; colIndex++)
            {
                double colX = x + colIndex * colWidth;
                gfx.DrawString(datos.Columns[colIndex].ColumnName, headerFont, XBrushes.Black, new XPoint(colX + 5, y + 5), XStringFormats.TopLeft);
            }

            y += rowHeight; // Moverse a la primera fila de datos

            // Dibujar las filas de datos con colores alternos
            for (int rowIndex = 0; rowIndex < datos.Rows.Count; rowIndex++)
            {
                // Seleccionar color de fondo (alternar colores)
                XBrush rowBackground = (rowIndex % 2 == 0) ? rowBackground1 : rowBackground2;

                // Dibujar fondo de la fila
                gfx.DrawRectangle(rowBackground, x, y, datos.Columns.Count * colWidth, rowHeight);

                // Dibujar los datos de la fila
                for (int colIndex = 0; colIndex < datos.Columns.Count; colIndex++)
                {
                    double colX = x + colIndex * colWidth;
                    string texto = datos.Rows[rowIndex][colIndex].ToString();
                    gfx.DrawString(texto, font_, XBrushes.Black, new XPoint(colX + 5, y + 5), XStringFormats.TopLeft);
                }

                y += rowHeight; // Moverse a la siguiente fila
            }

        }//Cierra el metodo AgregarTabla


        /// <summary>
        /// Convierte una List de objetos en un DataTable con los datos de los objetos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>        
        public DataTable ConvertToDataTable(List<Dictionary<string, object>> data)
        {
            DataTable table = new DataTable();

            if (data == null || data.Count == 0)
                return table;

            // Crear columnas basadas en las claves del primer diccionario
            foreach (var key in data[0].Keys)
            {
                table.Columns.Add(key, typeof(object));
            }
            
            foreach (var rowDict in data)
            {
                var row = table.NewRow();
                foreach (var key in rowDict.Keys)
                {
                    var value = rowDict[key];
                    string strValue = value.ToString();

                    if (strValue.Length > 15)
                    {
                        row[key] = strValue.Substring(0, 15); // Trunca el valor a 15 caracteres
                    }
                    else
                    {
                        row[key] = value ?? DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }

            return table;

        }//cierra el metodo convertToDataTable


        public List<DataTable> SplitDataTable(DataTable originalTable, int maxRows)
        {
            List<DataTable> tables = new List<DataTable>();
            int totalRows = originalTable.Rows.Count;
            int tableIndex = 0;

            while (tableIndex * maxRows < totalRows)
            {
                DataTable newTable = originalTable.Clone(); // Copia la estructura de la tabla original

                for (int i = tableIndex * maxRows; i < Math.Min((tableIndex + 1) * maxRows, totalRows); i++)
                {
                    newTable.ImportRow(originalTable.Rows[i]); // Agrega las filas hasta el máximo permitido
                }

                tables.Add(newTable);
                tableIndex++;
            }

            return tables;
        }






    }//cierra la clase

}//cierra el manespace
