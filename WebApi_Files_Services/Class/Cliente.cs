namespace WebApi_Files_Services.Class
{
    public class Cliente
    {
        public string NOMBRE { get; set; }
       
        public string IDENT { get; set; }

        public string DIRECCION { get; set; }

        public string TELEFONO { get; set; }
               

    }//cierra la clase

    public class Proveedor : Cliente
    {
        public new string IDENTIFICACION
        {
            get => base.IDENT;
            set => base.IDENT = value;
        }

    }//cierra la clase


    public class BzClient
    {
        public string EMPRESA { get; set; }
 
        public string NOMBRE { get; set; }

        public string IMG_PERFIL { get; set; }

        public string DIRECCION { get; set; }

        public string TELEFONO { get; set; }

        public string MAIL { get; set; }

    }//cierra la clase
    

    public class Detalle
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

    }//cierra la clase


}//cierra el namespace
