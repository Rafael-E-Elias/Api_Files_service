# 📄 Servicio de Generación de Facturas en PDF – Microservicio .NET 6

Este microservicio forma parte de la arquitectura de microservicios del sistema de facturación **Bizdata**. Está desarrollado en **.NET 6** y utiliza **PdfSharp-MigraDoc** para generar archivos PDF de facturas basadas en una plantilla.

La API recibe vía POST los datos necesarios, modifica una plantilla PDF precargada con dicha información y guarda el resultado en una carpeta organizada por cliente. Finalmente, devuelve la ruta local del archivo generado.

---

## 🚀 Tecnologías utilizadas

- .NET 6  
- ASP.NET Core Web API  
- [PdfSharp-MigraDoc 6.1.1](https://www.nuget.org/packages/PdfSharp-MigraDoc)  
- Uso de variables de entorno para configurar rutas

---

## 🧩 Estructura del proyecto

bizdata-pdf-service/ 
	├── Class/ 
	                 │ └── Cliente.cs  
	                 │ └── Pdf_Maker.cs  
	├── Controllers/ 
		│ └── DepositoController.cs  
		│ └── FacturaController.cs  
		│ └── OdernPagoController.cs  
		│ └── PresupuestoController.cs  
	├── Service/ 
		│ └── FacturaService.cs 
		│ └── OPagoService.cs
		│ └── PresupuestoService.cs
├── appsettings.json 
├── Program.cs 
├── README.md 
└── bizdata-pdf-service.csproj


---

## 🔧 Cómo ejecutar el proyecto

### 1. Requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Visual Studio 2022 o Visual Studio Code

### 2. Configurar variables de entorno

El servicio utiliza **variables de entorno** para definir rutas. Antes de ejecutar, asegurate de configurar las siguientes:

| Variable                     | Descripción                                                      | Ejemplo                                                      | 
|----------------------|-----------------------------------------------|------------------------------------------|
| `ROOT`         	 | Ruta raíz del proyecto                     	      | `C:\\Bizdata`                                             |
| `TEMPLATE`          | Subruta a la plantilla PDF                 	      | `\\Templates\\factura_base.pdf`          |
| `OUTPUT`              | Subruta base donde se guardan los PDFs   | `C:\\Cliente\\`                                          |

Estas se combinan así:
- Ruta plantilla: `${ROOT}${TEMPLATE}`
- Carpeta destino: `${ROOT}${OUTPUT}\\<NombreCliente>\\`

### 3. Ejecutar el servicio

```bash
dotnet run

Esto levantará el servicio en https://localhost:7080/


Ejemplo de request (POST /Factura)

{
  "BZCLNT": {
    "ID": 8,
    "EMPRESA": "ClientePrueba",
    "SUCURSAL": "Central",
    "NOMBRE": "Rafael Elias",
    "MAIL": "-",
    "TELEFONO": "-",
    "DIRECCION": "-",
    "IMG_PERFIL": ""
  },
  "NUMERO": "0003",
  "CLIENTE": {
    "NOMBRE": "Shakira",
    "IDENT": "99777123",
    "DIRECCION": "Av Rivadavia 1500",
    "TELEFONO": "116633224"
  },
  "DETALLE": [
    {
      "ID": 1,
      "CODIGO": "CHA001",
      "PRODUCTO": "Chaleco Puffer mujer",
      "PRECIO_UN": 9000,
      "CANTIDAD": 10,
      "SUBTOTAL": 90000
    }
  ],
  "OBSERVACION": "CTA CTE:",
  "TOTAL": "90000.00",
  "VALIDO": 10
}

Respuesta esperada

{
    "path": "ClientePrueba\\Factura\\FACTURA N° 0003.pdf"
}


Ejemplo de request (POST /Factura/cobranza)

{
  "BZCLNT": {
    "ID": 8,
    "EMPRESA": "ClientePrueba",
    "SUCURSAL": "Central",
    "NOMBRE": "Rafael Elias",
    "MAIL": "-",
    "TELEFONO": "-",
    "DIRECCION": "-",
    "IMG_PERFIL": ""
  },
 "NUMERO":103,
 "CLIENTE":{
 "NOMBRE":"Cliente Test",
 "IDENT":"1266953",
 "DIRECCION":"Av Libertador 500",
 "TELEFONO":"11223366955"
},
 "DETALLE":[
 {
  "FACTURA":35,
  "TOTAL":1000,
  "PAGO":"Efectivo",
  "MONTO":1000,
 "SALDO":0
 }
]
}

Respuesta esperada

{
    "path": "ClientePrueba\\Recibo de cobranza\\RECIBO DE COBRANZA N° 0103.pdf"
}
