-- SQL Script: 01_create_structure_utm_market.sql
-- Description: Script para crear la estructura de tablas Producto, Venta y Detalle de Venta
--              para la base de datos 'test_UTM_ABCO' en Microsoft SQL Server 2022 Express.
--              Incluye tipos de datos optimizados, claves primarias, claves foráneas,
--              restricciones CHECK y soporte para idempotencia.

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Usar la base de datos especificada
USE [utmMarker];
GO

BEGIN TRANSACTION;
BEGIN TRY

    -- Tabla Producto
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[Producto] (
            ProductoID    INT IDENTITY(1,1) PRIMARY KEY,
            Nombre        NVARCHAR(100) NOT NULL,
            SKU           VARCHAR(20) UNIQUE NOT NULL,
            Marca         NVARCHAR(50),
            Precio        DECIMAL(19,4) NOT NULL CONSTRAINT CHK_Producto_Precio_Positivo CHECK (Precio >= 0),
            Stock         INT NOT NULL CONSTRAINT CHK_Producto_Stock_NoNegativo CHECK (Stock >= 0)
        );
        PRINT 'Tabla [dbo].[Producto] creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'Tabla [dbo].[Producto] ya existe. No se realizaron cambios.';
    END;

    -- Tabla Customers
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[Customers] (
            Id            INT IDENTITY(1,1) PRIMARY KEY,
            FullName      NVARCHAR(100) NOT NULL,
            Email         NVARCHAR(255) UNIQUE NOT NULL,
            IsActive      BIT NOT NULL DEFAULT 1
        );
        PRINT 'Tabla [dbo].[Customers] creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'Tabla [dbo].[Customers] ya existe. No se realizaron cambios.';
    END;

    -- Tabla Venta
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Venta]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[Venta] (
            VentaID       INT IDENTITY(1,1) PRIMARY KEY,
            Folio         VARCHAR(20) UNIQUE NOT NULL,
            FechaVenta    DATETIME NOT NULL DEFAULT GETDATE(),
            TotalArticulos INT NOT NULL CONSTRAINT CHK_Venta_TotalArticulos_Positivo CHECK (TotalArticulos >= 0),
            TotalVenta    DECIMAL(19,4) NOT NULL CONSTRAINT CHK_Venta_TotalVenta_Positivo CHECK (TotalVenta >= 0),
            Estatus       TINYINT NOT NULL CONSTRAINT CHK_Venta_Estatus_Valido CHECK (Estatus IN (1, 2, 3))
            -- Comentarios para Estatus:
            -- 1: Pendiente - La venta ha sido iniciada pero no finalizada.
            -- 2: Completada - La venta ha sido procesada y finalizada exitosamente.
            -- 3: Cancelada - La venta ha sido anulada.
        );
        PRINT 'Tabla [dbo].[Venta] creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'Tabla [dbo].[Venta] ya existe. No se realizaron cambios.';
    END;

    -- Tabla Detalle de Venta
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleVenta]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[DetalleVenta] (
            DetalleID     INT IDENTITY(1,1) PRIMARY KEY,
            VentaID       INT NOT NULL,
            ProductoID    INT NOT NULL,
            PrecioUnitario DECIMAL(19,4) NOT NULL CONSTRAINT CHK_DetalleVenta_PrecioUnitario_Positivo CHECK (PrecioUnitario >= 0),
            Cantidad      INT NOT NULL CONSTRAINT CHK_DetalleVenta_Cantidad_Positiva CHECK (Cantidad >= 0),
            TotalDetalle  DECIMAL(19,4) NOT NULL CONSTRAINT CHK_DetalleVenta_TotalDetalle_Positivo CHECK (TotalDetalle >= 0),

            -- Definición de Claves Foráneas
            CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (VentaID) REFERENCES [dbo].[Venta](VentaID) ON DELETE CASCADE,
            CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (ProductoID) REFERENCES [dbo].[Producto](ProductoID) ON DELETE NO ACTION
        );
        PRINT 'Tabla [dbo].[DetalleVenta] creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'Tabla [dbo].[DetalleVenta] ya existe. No se realizaron cambios.';
    END;

    -- Creación de índices para optimización de consultas en FKs (opcional, pero buena práctica)
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DetalleVenta_VentaID' AND object_id = OBJECT_ID(N'[dbo].[DetalleVenta]'))
    BEGIN
        CREATE INDEX IX_DetalleVenta_VentaID ON [dbo].[DetalleVenta] (VentaID);
        PRINT 'Índice IX_DetalleVenta_VentaID creado.';
    END;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DetalleVenta_ProductoID' AND object_id = OBJECT_ID(N'[dbo].[DetalleVenta]'))
    BEGIN
        CREATE INDEX IX_DetalleVenta_ProductoID ON [dbo].[DetalleVenta] (ProductoID);
        PRINT 'Índice IX_DetalleVenta_ProductoID creado.';
    END;

    COMMIT TRANSACTION;
    PRINT 'Script de creación de estructura ejecutado exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT;
    SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
