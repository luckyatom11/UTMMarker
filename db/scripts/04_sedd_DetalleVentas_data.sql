SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Database: test_UTM_ABCO

-- Paso 8: Script para poblar la tabla 'DetalleVenta'
-- Ubicación de la salida: /db/scripts/04_sedd_ventas_data.sql

-- Esquema de referencia para DetalleVenta:
-- CREATE TABLE [dbo].[DetalleVenta] (
--         DetalleID     INT IDENTITY(1,1) PRIMARY KEY,
--         VentaID       INT NOT NULL,
--         ProductoID    INT NOT NULL,
--         PrecioUnitario DECIMAL(19,4) NOT NULL CONSTRAINT CHK_DetalleVenta_PrecioUnitario_Positivo CHECK (PrecioUnitario >= 0),
--         Cantidad      INT NOT NULL CONSTRAINT CHK_DetalleVenta_Cantidad_Positiva CHECK (Cantidad >= 0),
--         TotalDetalle  DECIMAL(19,4) NOT NULL CONSTRAINT CHK_DetalleVenta_TotalDetalle_Positivo CHECK (TotalDetalle >= 0),
--         CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (VentaID) REFERENCES [dbo].[Venta](VentaID) ON DELETE CASCADE,
--         CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (ProductoID) REFERENCES [dbo].[Producto](ProductoID) ON DELETE NO ACTION
--     );

PRINT 'Iniciando el script para insertar datos en DetalleVenta...';

-- IMPORTANTE: Este script asume que los registros en las tablas 'Venta' y 'Producto'
-- con los IDs referenciados (VentaID del 1 al 5 y ProductoID del 1 al 5) ya existen.
-- Asegúrese de ejecutar los scripts de seeding para 'Producto' y 'Venta' primero.

-- Idempotencia: Eliminar datos existentes para evitar duplicados si se ejecuta varias veces
-- Considerar que la eliminación de DetalleVenta puede estar restringida por claves foráneas si VentaID o ProductoID no existen.
-- Para este script de seeding, asumimos que Venta y Producto ya tienen datos o que los IDs generados serán válidos.
-- Si esto se ejecuta en un entorno de desarrollo, a menudo se desea una tabla limpia.
-- En un entorno de producción, se preferiría una lógica de MERGE o INSERT IF NOT EXISTS basada en una clave única si existiera.
-- Dado que DetalleID es IDENTITY, insertaremos nuevos registros.
-- Si quisiéramos idempotencia estricta para el *contenido*, necesitaríamos una clave única para DetalleVenta
-- (e.g., combinación de VentaID y ProductoID si un producto solo puede aparecer una vez por venta).
-- Para este ejercicio, simplemente insertaremos nuevos registros asumiendo que los IDs de Venta y Producto existirán.

-- Insertar 20 registros de ejemplo en DetalleVenta
BEGIN TRANSACTION;
BEGIN TRY
    -- Ejemplo 1
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 1 AND ProductoID = 1 AND PrecioUnitario = 12.50 AND Cantidad = 2)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (1, 1, 12.50, 2, 25.00);
    
    -- Ejemplo 2
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 1 AND ProductoID = 2 AND PrecioUnitario = 5.00 AND Cantidad = 3)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (1, 2, 5.00, 3, 15.00);
    
    -- Ejemplo 3
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 2 AND ProductoID = 3 AND PrecioUnitario = 20.00 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (2, 3, 20.00, 1, 20.00);
    
    -- Ejemplo 4
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 2 AND ProductoID = 4 AND PrecioUnitario = 8.75 AND Cantidad = 4)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (2, 4, 8.75, 4, 35.00);
    
    -- Ejemplo 5
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 3 AND ProductoID = 5 AND PrecioUnitario = 30.00 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (3, 5, 30.00, 1, 30.00);

    -- Ejemplo 6
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 3 AND ProductoID = 1 AND PrecioUnitario = 12.50 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (3, 1, 12.50, 1, 12.50);

    -- Ejemplo 7
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 4 AND ProductoID = 2 AND PrecioUnitario = 5.00 AND Cantidad = 5)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (4, 2, 5.00, 5, 25.00);

    -- Ejemplo 8
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 4 AND ProductoID = 3 AND PrecioUnitario = 20.00 AND Cantidad = 2)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (4, 3, 20.00, 2, 40.00);

    -- Ejemplo 9
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 5 AND ProductoID = 4 AND PrecioUnitario = 8.75 AND Cantidad = 3)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (5, 4, 8.75, 3, 26.25);

    -- Ejemplo 10
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 5 AND ProductoID = 5 AND PrecioUnitario = 30.00 AND Cantidad = 2)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (5, 5, 30.00, 2, 60.00);
    
    -- Más ejemplos para llegar a 20
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 1 AND ProductoID = 3 AND PrecioUnitario = 20.00 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (1, 3, 20.00, 1, 20.00);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 2 AND ProductoID = 1 AND PrecioUnitario = 12.50 AND Cantidad = 3)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (2, 1, 12.50, 3, 37.50);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 3 AND ProductoID = 2 AND PrecioUnitario = 5.00 AND Cantidad = 2)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (3, 2, 5.00, 2, 10.00);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 4 AND ProductoID = 5 AND PrecioUnitario = 30.00 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (4, 5, 30.00, 1, 30.00);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 5 AND ProductoID = 1 AND PrecioUnitario = 12.50 AND Cantidad = 4)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (5, 1, 12.50, 4, 50.00);
    
    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 1 AND ProductoID = 4 AND PrecioUnitario = 8.75 AND Cantidad = 2)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (1, 4, 8.75, 2, 17.50);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 2 AND ProductoID = 5 AND PrecioUnitario = 30.00 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (2, 5, 30.00, 1, 30.00);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 3 AND ProductoID = 3 AND PrecioUnitario = 20.00 AND Cantidad = 2)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (3, 3, 20.00, 2, 40.00);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 4 AND ProductoID = 1 AND PrecioUnitario = 12.50 AND Cantidad = 1)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (4, 1, 12.50, 1, 12.50);

    IF NOT EXISTS (SELECT 1 FROM [dbo].[DetalleVenta] WHERE VentaID = 5 AND ProductoID = 2 AND PrecioUnitario = 5.00 AND Cantidad = 3)
    INSERT INTO [dbo].[DetalleVenta] (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle) VALUES (5, 2, 5.00, 3, 15.00);

    COMMIT TRANSACTION;
    PRINT 'Datos insertados exitosamente en DetalleVenta.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
