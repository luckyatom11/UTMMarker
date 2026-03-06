-- SQL Script: 02_seed_data_utm_market.sql
-- Description: Script para insertar 250 productos de alta rotación en México (2025)
--              en la tabla [dbo].[Producto] para la base de datos 'test_UTM_ABCO'.
--              Incluye limpieza previa, gestión de identidad, y manejo transaccional.

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE [test_UTM_ABCO];
GO

BEGIN TRANSACTION;
BEGIN TRY

    PRINT 'Iniciando proceso de seeding para la tabla [dbo].[Producto]...';

    -- Limpieza previa: Borrar todos los registros existentes y reiniciar la columna IDENTITY.
    -- Se usa DELETE para evitar problemas con las FK y se reinicia el IDENTITY manualmente.
    PRINT 'Borrando datos de las tablas en orden de dependencia...';
    DELETE FROM [dbo].[DetalleVenta];
    DBCC CHECKIDENT ('[dbo].[DetalleVenta]', RESEED, 0);
    PRINT 'Tabla [dbo].[DetalleVenta] limpiada y reiniciada.';

    DELETE FROM [dbo].[Venta];
    DBCC CHECKIDENT ('[dbo].[Venta]', RESEED, 0);
    PRINT 'Tabla [dbo].[Venta] limpiada y reiniciada.';

    DELETE FROM [dbo].[Producto];
    DBCC CHECKIDENT ('[dbo].[Producto]', RESEED, 0);
    PRINT 'Tabla [dbo].[Producto] limpiada y reiniciada.';

    -- Permitir la inserción explícita de valores en la columna IDENTITY
    SET IDENTITY_INSERT [dbo].[Producto] ON;
    PRINT 'SET IDENTITY_INSERT ON para [dbo].[Producto].';

    -- Bloque de inserciones de productos
INSERT INTO [dbo].[Producto] (ProductoID, Nombre, SKU, Marca, Precio, Stock) VALUES
(1, N'Coca-Cola Original 600ml', '7501055300010', N'Coca-Cola', 18.5000, 150),
(2, N'Pepsi Regular 600ml', '7501031300027', N'Pepsi', 17.0000, 140),
(3, N'Fanta Naranja 600ml', '7501055300034', N'Fanta', 16.5000, 120),
(4, N'Sprite Lima-Lim�n 600ml', '7501055300041', N'Sprite', 16.5000, 130),
(5, N'Agua Ciel Garrafa 10L', '7501055300058', N'Ciel', 35.0000, 50),
(6, N'Jumex Naranja 1L', '7501031300069', N'Jumex', 28.0000, 70),
(7, N'Boing Guayaba 500ml', '7501004100078', N'Boing', 15.0000, 100),
(8, N'Red Bull Lata 250ml', '7501055300085', N'Red Bull', 38.0000, 80),
(9, N'Coca-Cola Zero 600ml', '7501055300092', N'Coca-Cola', 18.5000, 110),
(10, N'Electrolit Uva 612ml', '7501018600108', N'Electrolit', 25.0000, 90),
(11, N'Sabritas Original 40g', '7501000600115', N'Sabritas', 14.0000, 200),
(12, N'Doritos Nacho 40g', '7501000600122', N'Doritos', 14.0000, 180),
(13, N'Cheetos Puffs 30g', '7501000600139', N'Cheetos', 13.0000, 170),
(14, N'Takis Fuego 56g', '7501000600146', N'Takis', 16.0000, 160),
(15, N'Chips Saladas 40g', '7501000600153', N'Barcel', 12.0000, 150),
(16, N'Ruffles Queso 40g', '7501000600160', N'Ruffles', 14.0000, 140),
(17, N'Pop-Tarts Fresa', '7501000600177', N'Kellogg''s', 20.0000, 60),
(18, N'Kinder Delice 39g', '7501000600184', N'Kinder', 18.0000, 90),
(19, N'Mamut 42g', '7501000600191', N'Gamesa', 10.0000, 110),
(20, N'Gansito Marinela 50g', '7501000600207', N'Marinela', 19.0000, 120),
(21, N'Leche Lala Entera 1L', '7501020500214', N'Lala', 25.0000, 80),
(22, N'Yoghurt Lala Fresa 1L', '7501020500221', N'Lala', 32.0000, 60),
(23, N'Queso Panela Nochebuena 200g', '7501020500238', N'Nochebuena', 45.0000, 40),
(24, N'Crema Lala 200ml', '7501020500245', N'Lala', 22.0000, 50),
(25, N'Danonino Fresa 45g', '7501020500252', N'Danone', 8.0000, 100),
(26, N'Activia Ciruela 120g', '7501020500269', N'Activia', 15.0000, 70),
(27, N'Leche Alpura Deslactosada 1L', '7501020500276', N'Alpura', 26.0000, 75),
(28, N'Queso Oaxaca Bafar 200g', '7501020500283', N'Bafar', 48.0000, 35),
(29, N'Yoghurt Griego Fage Natural 150g', '7501020500290', N'Fage', 28.0000, 45),
(30, N'Mantequilla Gloria 90g', '7501020500306', N'Gloria', 19.0000, 65),
(31, N'Pan Blanco Bimbo Grande', '7501000600313', N'Bimbo', 35.0000, 90),
(32, N'Donitas Bimbo Azucaradas', '7501000600320', N'Bimbo', 20.0000, 80),
(33, N'Conchas Bimbo Vainilla', '7501000600337', N'Bimbo', 18.0000, 70),
(34, N'Roles de Canela Marinela', '7501000600344', N'Marinela', 22.0000, 60),
(35, N'Pan Integral Oroweat 12 rebanadas', '7501000600351', N'Oroweat', 42.0000, 50),
(36, N'Mantecadas Bimbo Nuez', '7501000600368', N'Bimbo', 17.0000, 75),
(37, N'Birote Salado (pieza)', '7501000600375', N'Local', 6.0000, 100),
(38, N'Cuernito Horneado (pieza)', '7501000600382', N'Local', 12.0000, 85),
(39, N'Empanada de Pi�a La Esperanza', '7501000600399', N'La Esperanza', 25.0000, 40),
(40, N'Galletas Mar�as Gamesa 170g', '7501000600405', N'Gamesa', 15.0000, 130),
(41, N'Arroz Verde Valle 1Kg', '7501005100412', N'Verde Valle', 28.0000, 100),
(42, N'Frijoles Refritos La Sierra 400g', '7501005100429', N'La Sierra', 18.0000, 90),
(43, N'Aceite Nutrioli 946ml', '7501005100436', N'Nutrioli', 45.0000, 70),
(44, N'Az�car Est�ndar Zulka 1Kg', '7501005100443', N'Zulka', 26.0000, 80),
(45, N'Caf� Soluble Nescaf� Cl�sico 120g', '7501005100450', N'Nescaf�', 70.0000, 60),
(46, N'Pasta Barilla Spaguetti 200g', '7501005100467', N'Barilla', 14.0000, 110),
(47, N'Lentejas La Merced 500g', '7501005100474', N'La Merced', 22.0000, 50),
(48, N'Sal La Fina Yodada 1Kg', '7501005100481', N'La Fina', 10.0000, 120),
(49, N'Sopa Nissin Res Picante', '7501005100498', N'Nissin', 15.0000, 150),
(50, N'At�n Dolores Lata 140g', '7501005100504', N'Dolores', 20.0000, 95);
INSERT INTO [dbo].[Producto] (ProductoID, Nombre, SKU, Marca, Precio, Stock) VALUES
(51, N'Cerveza Corona Extra Lata 355ml', '7501068800511', N'Corona', 20.0000, 180),
(52, N'Jugo del Valle Manzana 300ml', '7501055300528', N'Del Valle', 15.0000, 110),
(53, N'Leche Sello Rojo Deslactosada 1L', '7501031300535', N'Sello Rojo', 27.0000, 70),
(54, N'Caf� Punta del Cielo Americano', '7501020500542', N'Punta del Cielo', 30.0000, 50),
(55, N'Agua Bonafont 1L', '7501055300559', N'Bonafont', 12.0000, 160),
(56, N'Yakult 5 pack', '7501004100566', N'Yakult', 40.0000, 60),
(57, N'Monster Energy Green 473ml', '7501055300573', N'Monster', 35.0000, 90),
(58, N'Arizona T� Helado Lim�n 680ml', '7501031300580', N'Arizona', 22.0000, 80),
(59, N'Sidral Mundet Manzana 600ml', '7501055300597', N'Sidral Mundet', 17.5000, 130),
(60, N'Gatorade Naranja 600ml', '7501055300603', N'Gatorade', 29.0000, 100),
(61, N'Sabritas Flamin Hot 40g', '7501000600610', N'Sabritas', 14.0000, 170),
(62, N'Doritos Inc�gnita 40g', '7501000600627', N'Doritos', 14.0000, 150),
(63, N'Cheetos Torciditos 30g', '7501000600634', N'Cheetos', 13.0000, 160),
(64, N'Rancheritos 40g', '7501000600641', N'Sabritas', 14.0000, 140),
(65, N'Takis Original 56g', '7501000600658', N'Takis', 16.0000, 130),
(66, N'Chips Fuego 40g', '7501000600665', N'Barcel', 13.0000, 120),
(67, N'Prispas Saladas', '7501000600672', N'Bimbo', 10.0000, 100),
(68, N'Galletas Oreo Cl�sicas 6pzs', '7501000600689', N'Oreo', 18.0000, 110),
(69, N'Nucita Trisabor 30g', '7501000600696', N'Nucita', 12.0000, 90),
(70, N'Paleta Payaso Grande', '7501000600702', N'Ricolino', 18.0000, 80),
(71, N'Leche Santa Clara Entera 1L', '7501020500719', N'Santa Clara', 26.0000, 75),
(72, N'Yoghurt Yoplait Batido Fresa 145g', '7501020500726', N'Yoplait', 14.0000, 90),
(73, N'Queso Cotija Bafar 200g', '7501020500733', N'Bafar', 50.0000, 30),
(74, N'Media Crema Nestl� 190g', '7501020500740', N'Nestl�', 19.0000, 60),
(75, N'Leche Condensada La Lechera 375g', '7501020500757', N'La Lechera', 38.0000, 40),
(76, N'Yoghurt Griego Chobani Fresa 150g', '7501020500764', N'Chobani', 30.0000, 40),
(77, N'Queso Philadelphia Barra 190g', '7501020500771', N'Philadelphia', 65.0000, 25),
(78, N'Leche evaporada Carnation Clavel 360g', '7501020500788', N'Carnation', 20.0000, 55),
(79, N'Crema �cida Alpura 200ml', '7501020500795', N'Alpura', 24.0000, 50),
(80, N'Danone Bebible Fresa 240g', '7501020500801', N'Danone', 16.0000, 80),
(81, N'Pan de Caja Wonder Blanco Grande', '7501000600818', N'Wonder', 33.0000, 85),
(82, N'Tortillinas T�a Rosa 12 pzas', '7501000600825', N'T�a Rosa', 28.0000, 70),
(83, N'Cuernitos Bimbo Salados', '7501000600832', N'Bimbo', 20.0000, 65),
(84, N'Pan Tostado Bimbo Cl�sico', '7501000600849', N'Bimbo', 25.0000, 55),
(85, N'Roles Glaseados Marinela', '7501000600856', N'Marinela', 24.0000, 50),
(86, N'Pan para Hamburguesa Bimbo 8 pzas', '7501000600863', N'Bimbo', 30.0000, 60),
(87, N'Pan de Muerto (temporada)', '7501000600870', N'Local', 30.0000, 40),
(88, N'Baguette (pieza)', '7501000600887', N'Local', 25.0000, 30),
(89, N'Brownie de Chocolate', '7501000600894', N'Local', 35.0000, 35),
(90, N'Galletas de Animalitos Gamesa 200g', '7501000600900', N'Gamesa', 12.0000, 140),
(91, N'Huevo San Juan Blanco 12 pzas', '7501005100917', N'San Juan', 42.0000, 80),
(92, N'Harina de Trigo Selecta 1Kg', '7501005100924', N'Selecta', 18.0000, 70),
(93, N'Salchichas FUD Viena 400g', '7501005100931', N'FUD', 35.0000, 60),
(94, N'Sopa de Pasta La Moderna Codo 200g', '7501005100948', N'La Moderna', 8.0000, 150),
(95, N'Chiles en Vinagre La Coste�a Lata 220g', '7501005100955', N'La Coste�a', 15.0000, 100),
(96, N'Mayonesa McCormick 190g', '7501005100962', N'McCormick', 25.0000, 70),
(97, N'Mermelada Smucker''s Fresa 270g', '7501005100979', N'Smucker''s', 38.0000, 45),
(98, N'Gelatina D''Gari Fresa 120g', '7501005100986', N'D''Gari', 10.0000, 110),
(99, N'Chocolate en Polvo Choco Milk 350g', '7501005100993', N'Choco Milk', 40.0000, 60),
(100, N'Cereal Zucaritas Kellogg''s 290g', '7501005101006', N'Kellogg''s', 45.0000, 50);
INSERT INTO [dbo].[Producto] (ProductoID, Nombre, SKU, Marca, Precio, Stock) VALUES
(101, N'Cerveza Pacifico Clara Lata 355ml', '7501068801019', N'Pacifico', 20.0000, 150),
(102, N'Agua mineral Topo Chico 600ml', '7501055301026', N'Topo Chico', 18.0000, 110),
(103, N'Jugo de Naranja Pasteurized 1L', '7501031301033', N'Florida 7', 30.0000, 60),
(104, N'Leche Evaporada Carnation 360g', '7501020501040', N'Carnation', 20.0000, 70),
(105, N'Agua purificada Epura 1.5L', '7501055301057', N'Epura', 15.0000, 140),
(106, N'Yakult Light 5 pack', '7501004101064', N'Yakult', 40.0000, 50),
(107, N'Coca-Cola Vainilla 600ml', '7501055301071', N'Coca-Cola', 19.0000, 80),
(108, N'Powerade Moras 600ml', '7501055301088', N'Powerade', 29.0000, 90),
(109, N'Delaware Punch 600ml', '7501055301095', N'Delaware', 16.0000, 120),
(110, N'Squirt Toronja 600ml', '7501055301101', N'Squirt', 17.0000, 100),
(111, N'Papas Pringles Original 137g', '7501000601116', N'Pringles', 40.0000, 60),
(112, N'Galletas Saladas Gamesa Crackets', '7501000601123', N'Gamesa', 18.0000, 110),
(113, N'Barritas Marinela Fresa', '7501000601130', N'Marinela', 15.0000, 130),
(114, N'Chicharr�n de Cerdo Sabritas', '7501000601147', N'Sabritas', 25.0000, 90),
(115, N'Cacahuates Mafer Salados 180g', '7501000601154', N'Mafer', 35.0000, 70),
(116, N'M&M''s Chocolate 47g', '7501000601161', N'M&M''s', 20.0000, 100),
(117, N'Carlos V Chocolate Barra', '7501000601178', N'Carlos V', 15.0000, 120),
(118, N'Kinder Sorpresa', '7501000601185', N'Kinder', 25.0000, 80),
(119, N'Hershey''s Chocolate con Almendras', '7501000601192', N'Hershey''s', 22.0000, 90),
(120, N'Skittles Original', '7501000601208', N'Skittles', 18.0000, 110),
(121, N'Leche Lala Light 1L', '7501020501215', N'Lala', 25.0000, 80),
(122, N'Yoghurt Lala Entero Natural 1L', '7501020501222', N'Lala', 32.0000, 60),
(123, N'Queso Amarillo Lala Americano 140g', '7501020501239', N'Lala', 30.0000, 50),
(124, N'Crema Lyncott Premium 200ml', '7501020501246', N'Lyncott', 35.0000, 40),
(125, N'Leche de Almendra Silk Original 946ml', '7501020501253', N'Silk', 48.0000, 30),
(126, N'Yoghurt Oikos Griego Fresa 145g', '7501020501260', N'Oikos', 28.0000, 60),
(127, N'Queso Panela Alpura 400g', '7501020501277', N'Alpura', 70.0000, 20),
(128, N'Leche de Soya Ades Original 1L', '7501020501284', N'Ades', 32.0000, 45),
(129, N'Yoghurt bebible Alpura Fresa 250g', '7501020501291', N'Alpura', 18.0000, 70),
(130, N'Media Crema Lala 200g', '7501020501307', N'Lala', 20.0000, 55),
(131, N'Bollos Bimbo 6 pzas', '7501000601314', N'Bimbo', 28.0000, 70),
(132, N'Pan Molido Bimbo Cl�sico 210g', '7501000601321', N'Bimbo', 20.0000, 80),
(133, N'Madalenas Bimbo Naranja', '7501000601338', N'Bimbo', 17.0000, 90),
(134, N'Twinkies Marinela', '7501000601345', N'Marinela', 19.0000, 100),
(135, N'Pan Multigrano Oroweat 12 rebanadas', '7501000601352', N'Oroweat', 45.0000, 45),
(136, N'Galletas Emperador Chocolate 10pzs', '7501000601369', N'Emperador', 18.0000, 110),
(137, N'Donas Glaseadas Krispy Kreme (pieza)', '7501000601376', N'Krispy Kreme', 28.0000, 30),
(138, N'Mini Sincro Bimbo 120g', '7501000601383', N'Bimbo', 15.0000, 60),
(139, N'Panqu� Marmolado Bimbo 250g', '7501000601390', N'Bimbo', 30.0000, 50),
(140, N'Galletas Habaneras Marinela', '7501000601406', N'Marinela', 12.0000, 130),
(141, N'Az�car Mascabado Zulka 1Kg', '7501005101413', N'Zulka', 30.0000, 70),
(142, N'Sal de Mesa Refinada La Fina 500g', '7501005101420', N'La Fina', 8.0000, 150),
(143, N'Aceite de Oliva Extra Virgen Carbonell 250ml', '7501005101437', N'Carbonell', 80.0000, 30),
(144, N'Vinagre Blanco Clemente Jacques 355ml', '7501005101444', N'Clemente Jacques', 15.0000, 90),
(145, N'Lentejas Grandes La Merced 1Kg', '7501005101451', N'La Merced', 40.0000, 60),
(146, N'Garbanzos La Coste�a Lata 400g', '7501005101468', N'La Coste�a', 20.0000, 80),
(147, N'Salsa de Tomate Del Fuerte 210g', '7501005101475', N'Del Fuerte', 12.0000, 100),
(148, N'Cereal Choco Krispis Kellogg''s 290g', '7501005101482', N'Kellogg''s', 45.0000, 55),
(149, N'Caf� Tostado y Molido Legal 250g', '7501005101499', N'Legal', 55.0000, 40),
(150, N'Mermelada McCormick Chabacano 270g', '7501005101505', N'McCormick', 38.0000, 45);
INSERT INTO [dbo].[Producto] (ProductoID, Nombre, SKU, Marca, Precio, Stock) VALUES
(151, N'Cerveza Victoria Lata 355ml', '7501068801514', N'Victoria', 20.0000, 160),
(152, N'Jugo V8 Original 300ml', '7501055301521', N'V8', 16.0000, 90),
(153, N'Leche Nutrileche Semidescremada 1L', '7501031301538', N'Nutrileche', 24.0000, 80),
(154, N'Caf� Punta del Cielo Espresso', '7501020501545', N'Punta del Cielo', 32.0000, 45),
(155, N'Agua Nestle Pureza Vital 1L', '7501055301552', N'Nestle', 12.0000, 150),
(156, N'Jumex Mango Lata 335ml', '7501031301569', N'Jumex', 15.0000, 100),
(157, N'Jarrito Tamarindo 600ml', '7501055301576', N'Jarritos', 16.0000, 130),
(158, N'Amstel Ultra Lata 355ml', '7501068801583', N'Amstel', 22.0000, 140),
(159, N'Coca-Cola Light 600ml', '7501055301590', N'Coca-Cola', 18.5000, 110),
(160, N'Red Cola 600ml', '7501031301606', N'Red Cola', 12.0000, 120),
(161, N'Takis Blue Heat 56g', '7501000601613', N'Takis', 17.0000, 150),
(162, N'Sabritones Chile y Lim�n 45g', '7501000601620', N'Sabritas', 15.0000, 160),
(163, N'Fritos Sal y Lim�n 40g', '7501000601637', N'Fritos', 13.0000, 140),
(164, N'Chipilines Queso 35g', '7501000601644', N'Chipilines', 12.0000, 130),
(165, N'Churrumais Lim�n 40g', '7501000601651', N'Gamesa', 14.0000, 120),
(166, N'Mini Doritos Nacho', '7501000601668', N'Doritos', 10.0000, 100),
(167, N'Galletas Pr�ncipe Chocolate 4pzs', '7501000601675', N'Pr�ncipe', 16.0000, 110),
(168, N'Bubulubu Ricolino', '7501000601682', N'Ricolino', 10.0000, 130),
(169, N'Panditas Ricolino Gomitas', '7501000601699', N'Ricolino', 12.0000, 140),
(170, N'Mazap�n de la Rosa Gigante', '7501000601705', N'De la Rosa', 10.0000, 150),
(171, N'Leche descremada Lala Siluet 1L', '7501020501712', N'Lala', 25.0000, 70),
(172, N'Yoghurt Lala Deslactosado Fresa 1L', '7501020501729', N'Lala', 32.0000, 55),
(173, N'Queso de cabra Lala 100g', '7501020501736', N'Lala', 60.0000, 20),
(174, N'Crema para batir Lyncott 250ml', '7501020501743', N'Lyncott', 40.0000, 30),
(175, N'Leche de Coco Calahua 1L', '7501020501750', N'Calahua', 50.0000, 25),
(176, N'Yoghurt Griego Fage Cero Grasa 150g', '7501020501767', N'Fage', 28.0000, 40),
(177, N'Queso Mozzarella Bafar 200g', '7501020501774', N'Bafar', 48.0000, 30),
(178, N'Leche de Arroz Vita Coco 1L', '7501020501781', N'Vita Coco', 45.0000, 20),
(179, N'Yoghurt Natural sin az�car Fage 150g', '7501020501798', N'Fage', 25.0000, 50),
(180, N'Flan Nestl� Individual', '7501020501804', N'Nestl�', 15.0000, 60),
(181, N'Pan para Hot Dog Bimbo 8 pzas', '7501000601811', N'Bimbo', 28.0000, 75),
(182, N'Bimbollos Bimbo 6 pzas', '7501000601828', N'Bimbo', 30.0000, 65),
(183, N'Pan de Caja Cero Cero Bimbo', '7501000601835', N'Bimbo', 40.0000, 50),
(184, N'Cuernitos de Mantequilla (pieza)', '7501000601842', N'Local', 15.0000, 80),
(185, N'Trenza de Queso (pieza)', '7501000601859', N'Local', 20.0000, 60),
(186, N'Polvorones Bimbo Surtidos', '7501000601866', N'Bimbo', 22.0000, 90),
(187, N'Roles de Manzana Marinela', '7501000601873', N'Marinela', 20.0000, 70),
(188, N'Donas Azucaradas Bimbo 3pzs', '7501000601880', N'Bimbo', 25.0000, 60),
(189, N'Pan de Elote (rebanada)', '7501000601897', N'Local', 30.0000, 40),
(190, N'Galletas Chokis Gamesa 76g', '7501000601903', N'Gamesa', 16.0000, 110),
(191, N'Frijoles Bayos La Sierra Refritos 400g', '7501005101910', N'La Sierra', 18.0000, 90),
(192, N'Ch�charos Herdez Lata 220g', '7501005101927', N'Herdez', 15.0000, 100),
(193, N'Consom� de Pollo Knorr Suiza 8 cubos', '7501005101934', N'Knorr Suiza', 28.0000, 80),
(194, N'Salsa Picante Valentina Etiqueta Negra 370ml', '7501005101941', N'Valentina', 16.0000, 120),
(195, N'Aceite de Girasol Capullo 946ml', '7501005101958', N'Capullo', 40.0000, 65),
(196, N'Miel de Abeja Carlota 300g', '7501005101965', N'Carlota', 60.0000, 35),
(197, N'Harina para Hot Cakes Gamesa Tradicional 500g', '7501005101972', N'Gamesa', 25.0000, 70),
(198, N'Cafe Nescaf� Decaf 50g', '7501005101989', N'Nescaf�', 45.0000, 40),
(199, N'Cereal Special K Kellogg''s 280g', '7501005101996', N'Kellogg''s', 50.0000, 45),
(200, N'Leche Condensada Nestl� La Lechera Light 375g', '7501020502001', N'La Lechera', 38.0000, 30);
INSERT INTO [dbo].[Producto] (ProductoID, Nombre, SKU, Marca, Precio, Stock) VALUES
(201, N'Cerveza Indio Lata 355ml', '7501068802016', N'Indio', 20.0000, 150),
(202, N'Jugo del Valle Pi�a 300ml', '7501055302023', N'Del Valle', 15.0000, 100),
(203, N'Leche Lala Sabor Chocolate 1L', '7501020502030', N'Lala', 28.0000, 70),
(204, N'Caf� Punta del Cielo Descafeinado', '7501020502047', N'Punta del Cielo', 30.0000, 40),
(205, N'Agua Evian 500ml', '7501055302054', N'Evian', 25.0000, 80),
(206, N'Jumex durazno 1L', '7501031302061', N'Jumex', 28.0000, 60),
(207, N'Coca-Cola Cherry 600ml', '7501055302078', N'Coca-Cola', 19.0000, 90),
(208, N'Gatorade Lima Lim�n 600ml', '7501055302085', N'Gatorade', 29.0000, 80),
(209, N'Pepsi Black 600ml', '7501031302092', N'Pepsi', 17.0000, 110),
(210, N'Dr. Pepper 600ml', '7501031302108', N'Dr. Pepper', 17.0000, 100),
(211, N'Sabritas Adobadas 40g', '7501000602119', N'Sabritas', 14.0000, 160),
(212, N'Ruffles Original 40g', '7501000602126', N'Ruffles', 14.0000, 150),
(213, N'Cheetos Colmillos 30g', '7501000602133', N'Cheetos', 13.0000, 140),
(214, N'Takis Huaki 56g', '7501000602140', N'Takis', 16.0000, 130),
(215, N'Doritos Dinamita Chile Lim�n 40g', '7501000602157', N'Doritos', 14.0000, 120),
(216, N'Palomitas Act II Mantequilla 80g', '7501000602164', N'Act II', 20.0000, 100),
(217, N'Galletas Maria Gamesa', '7501000602171', N'Gamesa', 15.0000, 130),
(218, N'ChocoRoles Marinela', '7501000602188', N'Marinela', 19.0000, 110),
(219, N'Pay de Lim�n Marinela', '7501000602195', N'Marinela', 22.0000, 90),
(220, N'Ping�inos Marinela', '7501000602201', N'Marinela', 19.0000, 120),
(221, N'Leche de Coco A de Coco 1L', '7501020502218', N'A de Coco', 45.0000, 25),
(222, N'Yoghurt Lala Kids Fresa 100g', '7501020502225', N'Lala', 12.0000, 90),
(223, N'Queso Crema Philadelphia Light 190g', '7501020502232', N'Philadelphia', 65.0000, 20),
(224, N'Crema Ranchera Bafar 200ml', '7501020502249', N'Bafar', 25.0000, 45),
(225, N'Leche de Vaca Org�nica Lala 1L', '7501020502256', N'Lala', 35.0000, 30),
(226, N'Yoghurt Griego Fage Vainilla 150g', '7501020502263', N'Fage', 28.0000, 35),
(227, N'Queso de Bola Holand�s 250g', '7501020502270', N'Importado', 80.0000, 15),
(228, N'Leche en Polvo Nido FortiGrow 360g', '7501020502287', N'Nido', 70.0000, 20),
(229, N'Yoghurt Bebible Yoplait Durazno 240g', '7501020502294', N'Yoplait', 16.0000, 70),
(230, N'Crema Chantilly Lyncott 250g', '7501020502300', N'Lyncott', 42.0000, 30),
(231, N'Conchas Bimbo Chocolate', '7501000602317', N'Bimbo', 18.0000, 70),
(232, N'Pan de Barra Oroweat Centeno 500g', '7501000602324', N'Oroweat', 40.0000, 40),
(233, N'Roles de Guayaba Marinela', '7501000602331', N'Marinela', 22.0000, 60),
(234, N'Galletas Salmas Sanissimo 10pzs', '7501000602348', N'Sanissimo', 20.0000, 100),
(235, N'Mini Gansitos Marinela', '7501000602355', N'Marinela', 15.0000, 90),
(236, N'Pan de Pasas y Nueces Bimbo 450g', '7501000602362', N'Bimbo', 35.0000, 50),
(237, N'Donas Azucaradas Rellenas (pieza)', '7501000602379', N'Local', 20.0000, 50),
(238, N'Bisquets Bimbo 6 pzas', '7501000602386', N'Bimbo', 30.0000, 60),
(239, N'Muffins de Ar�ndano (pieza)', '7501000602393', N'Local', 25.0000, 45),
(240, N'Galletas Surtido Rico Gamesa 300g', '7501000602409', N'Gamesa', 30.0000, 80),
(241, N'Cereal Kellogg''s Corn Flakes 300g', '7501005102416', N'Kellogg''s', 40.0000, 60),
(242, N'Aceite de Coco Org�nico 300ml', '7501005102423', N'Vita Coco', 70.0000, 30),
(243, N'Frijoles Negros La Coste�a Lata 400g', '7501005102430', N'La Coste�a', 18.0000, 90),
(244, N'Salsa Inglesa Crosse & Blackwell 295ml', '7501005102447', N'Crosse & Blackwell', 20.0000, 70),
(245, N'Harina de Ma�z Maseca 1Kg', '7501005102454', N'Maseca', 15.0000, 110),
(246, N'Gelatina D''Gari Uva 120g', '7501005102461', N'D''Gari', 10.0000, 120),
(247, N'Consom� de Res Knorr Suiza 8 cubos', '7501005102478', N'Knorr Suiza', 28.0000, 75),
(248, N'T� Lipton Sobres Manzanilla 25pzs', '7501005102485', N'Lipton', 35.0000, 50),
(249, N'Az�car Splenda Sobres 50pzs', '7501005102492', N'Splenda', 60.0000, 40),
(250, N'Capsulas de Cafe Nespresso (varias)', '7501005102508', N'Nespresso', 90.0000, 20);

    -- Desactivar la inserci�n expl�cita de valores en la columna IDENTITY
    SET IDENTITY_INSERT [dbo].[Producto] OFF;
    PRINT 'SET IDENTITY_INSERT OFF para [dbo].[Producto].';

    -- Sincronizar el contador de identidad despu�s de las inserciones
    DBCC CHECKIDENT ('dbo.Producto', RESEED, 250);
    PRINT 'DBCC CHECKIDENT ejecutado para [dbo].[Producto] con un valor de 250.';

    COMMIT TRANSACTION;
    PRINT 'Proceso de seeding para [dbo].[Producto] completado exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(MAX), @ErrorSeverity INT, @ErrorState INT;
    SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
