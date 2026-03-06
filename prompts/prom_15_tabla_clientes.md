IMPORTANTE:
CADA VEZ QUE FINALICES LA EJECUCION DE UN PASO PIDE CONFIRMACION AL USUARIO PARA PROSEGIR CON EL SIGUIENTE PASO

Paso 1:

tabla de clientes:

contexto:

crea una conexión a la base de datos "[name_data_base]" (la base de datos ya existe, asegúrate de usarlo)
ubicación de la salida: /db/scripts/03_sedd_customer_data.sql

requisitos:

tabla "customer":
columnas: 'customerId', 'FullName', 'Email', 'IsActive'
restricciones: el 'customerId' y 'Email' no pueden duplicarse; 

Asegura que el script sea idempotente usando "IF NOT EXISTS" o verificaciones previas de objetos.

Incluye SET NOCOUNT ON y SET XACT_ABORT ON al inicio del script.
- Usa comentarios detallados que expliquen la arquitectura de cada tabla.
- Asegura que el script sea idempotente (puedas ejecutarlo varias veces sin errores).


Paso 2:
contexto: 
poblaremos la tabla 'customer'

requisitos: 
Esquema de referencia: 
    CREATE TABLE [dbo].[Customers] (
            Id            INT IDENTITY(1,1) PRIMARY KEY,
            FullName      NVARCHAR(100) NOT NULL,
            Email         NVARCHAR(255) UNIQUE NOT NULL,
            IsActive      BIT NOT NULL DEFAULT 1
        );

ejecución: 

genera un script SQL que registre 30 usuarios tomando el esquema de referencia dado en el archivo

notas:
-Usa comentarios detallados que expliquen la arquitectura de cada tabla.
- Asegura que el script sea idempotente (puedas ejecutarlo varias veces sin errores).



Paso 3:
contexto:
necesitamos crear una clase 'Customer' para poder registrar a los distintos usuarios que se estaran registrando en la tabla

requisitos:
crea la clase 'customer' con las propiedades `CustomerId`, `FullName`, `Email` e `IsActive`
rstriccion: el email debe valdiades mediante logica en el setter con la palabra clave 'fiel' de C# 14





Paso 4: 

contexto:
necesitamos crear un contrato para darle el uso a la clase 'Customer'

requisito:
crear el contrato mediante la interfaz 'ICustomerRepository.cs' en la capa core con soporte para:
Busqueda de email: 'GetByEmailAsync(string email)' debe arrojar el email del usuario 
Registro de nuevo cliente: 'AddAsync(customer customer)' debe permitir el registro de un nuevo usuario mediante la peticion de las propiedades de la clase customer


Paso 5:
contexto:
infraestructura de la interfaz

requisito:
crea el repositorio concreto de 'ICustomerRepository.cs' utilizando el mapeo manual con 'sqlDataReader' (aseguremos la compatibilidad total con Native AOT
nota: es obligatorio el uso de primary constructors para la inyeccion de dependencias)


Paso 6: 

contexto: 
realizaremos la implementación de alertas para evitar la perdida de ventas

requisito: 
crearemos la interfaz `ILowStockAlertUseCase.cs` que se ubicara en la capa de Casos de Uso.
en ella implementaremos el caso de uso 'LowStockAlertUseCaseImpl.cs' la cual tendrá las siguientes funciones:
1. El método debe recibir un valor entero que se llamara `threshold` (que será el umbral)
2. Una vez ingresado deberá devolver un `IAsyncEnumerable<Product>`, una lista que contendrá solo aquellos productos cuyo stock sea menor o igual al umbral especificado

especificaciones:
1. El nuevo servicio deberá quedar registrado correctamente en el contenedor de servicios del archivo 'program.cs'
2. El flujo de los datos deberá ser asíncrona y eficiente en memoria 

Paso 7: 

contexto:
visualización de lo vendido en un periodo especifico.

requisito:
agregar una opción en el menú principal en la consola con el nombre "consultar ventas por fechas"

infraestructura:
1. el sistema debe solicitar al usuario una "fecha de inicio" y una "fecha de final" (nota: realiza la validación de la fecha ingresada, que este en un formato valido ('DataTime.TryParse'))
2. la  IU debe consumir el caso de uso 'ILowStockAlertUseCase' enviando un objeto 'SaleFilter' con el rango capturado

visualización:
1. los resultados deben mostrarse en una tabla formateada en consola incluyendo el folio, la fecha y el moto total de la venta

seguridad: 
1. como es una aplicación de consola, recuerda que los servicios de aplicación deben resolverse creando un 'IServiceScope' manual para cada ciclo de ejecución 


Paso 8:
crea una conexión a la base de datos "[name_data_base]" (la base de datos ya existe, asegúrate de usarlo)
ubicación de la salida: /db/scripts/04_sedd_DetalleVentas_data.sql

contexto: 
poblaremos la tabla 'DetalleVenta'

requisitos: 
Esquema de referencia: [estructura_de_la_tabla]



ejecución: 

genera un script SQL que registre 20 detalles de ventas tomando el esquema de referencia dado en el archivo

notas:
-Usa comentarios detallados que expliquen la arquitectura de cada tabla.
- Asegura que el script sea idempotente (puedas ejecutarlo varias veces sin errores).

Paso 9:
crea una conexión a la base de datos "test_UTM_ABCO" (la base de datos ya existe, asegúrate de usarlo)
ubicación de la salida: /db/scripts/05_sedd_ventas_data.sql

contexto: 
poblaremos la tabla 'venta'

requisitos: 
Esquema de referencia: [estructura_de_la_tabla] 



ejecución: 

genera un script SQL que registre 5 ventas tomando el esquema de referencia dado en el archivo

notas:
-Usa comentarios detallados que expliquen la arquitectura de cada tabla.
- Asegura que el script sea idempotente (puedas ejecutarlo varias veces sin errores).

PAso 10:
agrega una opcion en el menu principal donde se ingrese los detalles de venta (un punto de venta), posteriormente, se ingrese de manera automatica la venta en la tabla ventas ingresando los datos que pide dicha tabla 

notas:
1. verifica que los datos ingresados sean validos y correspondan a porductos que se encuentren la base de datos
2. Verifica que cada venta que se haga se descuente de la columna stock de la tabla productos

Paso 11:
ejecuta el programa y pide al usuario que interactue con ella para detectar errores en el codigo 
    



 