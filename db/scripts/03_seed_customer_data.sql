-- SQL Script: 03_seed_customer_data.sql
-- Description: Script para poblar la tabla Customers con datos de ejemplo.

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE [utmMarker];
GO

BEGIN TRANSACTION;
BEGIN TRY

    -- Insertar clientes de ejemplo
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers] WHERE Email = 'juan.perez@example.com')
    BEGIN
        INSERT INTO [dbo].[Customers] (FullName, Email, IsActive)
        VALUES ('Juan Perez', 'juan.perez@example.com', 1);
        PRINT 'Cliente Juan Perez insertado.';
    END
    ELSE
    BEGIN
        PRINT 'Cliente Juan Perez ya existe. No se insertó.';
    END;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers] WHERE Email = 'maria.gonzalez@example.com')
    BEGIN
        INSERT INTO [dbo].[Customers] (FullName, Email, IsActive)
        VALUES ('Maria Gonzalez', 'maria.gonzalez@example.com', 1);
        PRINT 'Cliente Maria Gonzalez insertado.';
    END
    ELSE
    BEGIN
        PRINT 'Cliente Maria Gonzalez ya existe. No se insertó.';
    END;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers] WHERE Email = 'pedro.ramirez@example.com')
    BEGIN
        INSERT INTO [dbo].[Customers] (FullName, Email, IsActive)
        VALUES ('Pedro Ramirez', 'pedro.ramirez@example.com', 0); -- Inactivo
        PRINT 'Cliente Pedro Ramirez insertado.';
    END
    ELSE
    BEGIN
        PRINT 'Cliente Pedro Ramirez ya existe. No se insertó.';
    END;

    COMMIT TRANSACTION;
    PRINT 'Script de poblamiento de clientes ejecutado exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT;
    SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
