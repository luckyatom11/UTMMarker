SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Database: test_UTM_ABCO

-- Paso 9: Script para poblar la tabla 'Venta'
-- Ubicación de la salida: /db/scripts/05_sedd_ventas_data.sql

-- Esquema de referencia para Venta:
-- CREATE TABLE [dbo].[Venta] (
--             VentaID       INT IDENTITY(1,1) PRIMARY KEY,
--             Folio         VARCHAR(20) UNIQUE NOT NULL,
--             FechaVenta    DATETIME NOT NULL DEFAULT GETDATE(),
--             TotalArticulos INT NOT NULL CONSTRAINT CHK_Venta_TotalArticulos_Positivo CHECK (TotalArticulos >= 0),
--             TotalVenta    DECIMAL(19,4) NOT NULL CONSTRAINT CHK_Venta_TotalVenta_Positivo CHECK (TotalVenta >= 0),
--             Estatus       TINYINT NOT NULL CONSTRAINT CHK_Venta_Estatus_Valido CHECK (Estatus IN (1, 2, 3))
--         );

PRINT 'Iniciando el script para insertar datos en Venta...';

BEGIN TRANSACTION;
BEGIN TRY
    -- Venta 1
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Venta] WHERE Folio = 'VENTA-20230101-001')
    INSERT INTO [dbo].[Venta] (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus) VALUES ('VENTA-20230101-001', '2023-01-01 10:00:00', 5, 125.75, 1);
    
    -- Venta 2
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Venta] WHERE Folio = 'VENTA-20230102-002')
    INSERT INTO [dbo].[Venta] (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus) VALUES ('VENTA-20230102-002', '2023-01-02 11:30:00', 3, 50.00, 1);
    
    -- Venta 3
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Venta] WHERE Folio = 'VENTA-20230103-003')
    INSERT INTO [dbo].[Venta] (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus) VALUES ('VENTA-20230103-003', '2023-01-03 14:15:00', 7, 210.50, 2);
    
    -- Venta 4
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Venta] WHERE Folio = 'VENTA-20230104-004')
    INSERT INTO [dbo].[Venta] (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus) VALUES ('VENTA-20230104-004', '2023-01-04 09:45:00', 2, 35.20, 1);
    
    -- Venta 5
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Venta] WHERE Folio = 'VENTA-20230105-005')
    INSERT INTO [dbo].[Venta] (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus) VALUES ('VENTA-20230105-005', '2023-01-05 16:00:00', 4, 88.99, 3);
    
    COMMIT TRANSACTION;
    PRINT 'Datos insertados exitosamente en Venta.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
