-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: LaCazuelaChapina
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Combo`
--

DROP TABLE IF EXISTS `Combo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Combo` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Codigo` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Tipo` enum('familiar','eventos','estacional') COLLATE utf8mb4_unicode_ci NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Descripcion` text COLLATE utf8mb4_unicode_ci,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  `Editable` tinyint(1) NOT NULL DEFAULT '0',
  `VigenciaInicio` datetime DEFAULT NULL,
  `VigenciaFin` datetime DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Codigo` (`Codigo`),
  KEY `IX_Combo_Tipo` (`Tipo`),
  KEY `IX_Combo_Activo` (`Activo`),
  CONSTRAINT `CK_Combo_Precio` CHECK ((`Precio` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Combo`
--

LOCK TABLES `Combo` WRITE;
/*!40000 ALTER TABLE `Combo` DISABLE KEYS */;
INSERT INTO `Combo` VALUES (1,'Fiesta Patronal','FIESTA_PATRONAL','familiar',150.00,'Docena surtida de tamales + 2 jarros familiares',1,0,NULL,NULL,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(2,'Madrugada del 24','MADRUGADA_24','eventos',420.00,'3 docenas de tamales + 4 jarros + termo de barro conmemorativo',1,0,NULL,NULL,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(3,'Combo Estacional','ESTACIONAL','estacional',0.00,'Varía según temporada',1,1,NULL,NULL,'2025-10-29 05:07:18','2025-10-29 05:07:18');
/*!40000 ALTER TABLE `Combo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ComboDetalle`
--

DROP TABLE IF EXISTS `ComboDetalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ComboDetalle` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ComboId` int NOT NULL,
  `ProductoId` int NOT NULL,
  `Cantidad` int NOT NULL,
  `ConfiguracionJson` json DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ComboDetalle_ComboId` (`ComboId`),
  KEY `IX_ComboDetalle_ProductoId` (`ProductoId`),
  CONSTRAINT `FK_ComboDetalle_Combo` FOREIGN KEY (`ComboId`) REFERENCES `Combo` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComboDetalle_Producto` FOREIGN KEY (`ProductoId`) REFERENCES `Producto` (`Id`),
  CONSTRAINT `CK_ComboDetalle_Cantidad` CHECK ((`Cantidad` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ComboDetalle`
--

LOCK TABLES `ComboDetalle` WRITE;
/*!40000 ALTER TABLE `ComboDetalle` DISABLE KEYS */;
INSERT INTO `ComboDetalle` VALUES (1,1,3,1,'{\"masa\": \"Maíz Amarillo\", \"picante\": \"Suave\", \"relleno\": \"Recado Rojo de Cerdo\", \"envoltura\": \"Hoja de Plátano\"}'),(2,1,5,2,'{\"tipo\": \"Atol de Elote\", \"topping\": \"Canela\", \"endulzante\": \"Panela\"}'),(3,2,3,3,'{\"masa\": \"Maíz Amarillo\", \"picante\": \"Chapín\", \"relleno\": \"Recado Rojo de Cerdo\", \"envoltura\": \"Hoja de Plátano\"}'),(4,2,5,4,'{\"tipo\": \"Cacao Batido\", \"topping\": \"Malvaviscos\", \"endulzante\": \"Panela\"}');
/*!40000 ALTER TABLE `ComboDetalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EndulzanteBebida`
--

DROP TABLE IF EXISTS `EndulzanteBebida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EndulzanteBebida` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EndulzanteBebida`
--

LOCK TABLES `EndulzanteBebida` WRITE;
/*!40000 ALTER TABLE `EndulzanteBebida` DISABLE KEYS */;
INSERT INTO `EndulzanteBebida` VALUES (1,'Panela',0.00,1),(2,'Miel',1.00,1),(3,'Sin Azúcar',0.00,1);
/*!40000 ALTER TABLE `EndulzanteBebida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EnvolturaTamal`
--

DROP TABLE IF EXISTS `EnvolturaTamal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EnvolturaTamal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EnvolturaTamal`
--

LOCK TABLES `EnvolturaTamal` WRITE;
/*!40000 ALTER TABLE `EnvolturaTamal` DISABLE KEYS */;
INSERT INTO `EnvolturaTamal` VALUES (1,'Hoja de Plátano',0.00,1),(2,'Tusa de Maíz',0.50,1);
/*!40000 ALTER TABLE `EnvolturaTamal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EstadisticaPicante`
--

DROP TABLE IF EXISTS `EstadisticaPicante`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EstadisticaPicante` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PicanteTamalId` int NOT NULL,
  `CantidadVendida` int NOT NULL DEFAULT '0',
  `Fecha` date NOT NULL,
  `SucursalId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EstadisticaPicante_PicanteTamal` (`PicanteTamalId`),
  KEY `IX_EstadisticaPicante_Fecha` (`Fecha`),
  KEY `IX_EstadisticaPicante_SucursalId` (`SucursalId`),
  CONSTRAINT `FK_EstadisticaPicante_PicanteTamal` FOREIGN KEY (`PicanteTamalId`) REFERENCES `PicanteTamal` (`Id`),
  CONSTRAINT `FK_EstadisticaPicante_Sucursal` FOREIGN KEY (`SucursalId`) REFERENCES `Sucursal` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EstadisticaPicante`
--

LOCK TABLES `EstadisticaPicante` WRITE;
/*!40000 ALTER TABLE `EstadisticaPicante` DISABLE KEYS */;
INSERT INTO `EstadisticaPicante` VALUES (1,1,8,'2025-10-28',1),(2,2,12,'2025-10-28',1),(3,3,15,'2025-10-28',1),(4,1,5,'2025-10-29',1),(5,3,10,'2025-10-29',1);
/*!40000 ALTER TABLE `EstadisticaPicante` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LoteCoccion`
--

DROP TABLE IF EXISTS `LoteCoccion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LoteCoccion` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `VaporeraId` int NOT NULL,
  `ProductoId` int NOT NULL,
  `Cantidad` int NOT NULL,
  `Inicio` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FinEstimado` timestamp NOT NULL,
  `FinReal` timestamp NULL DEFAULT NULL,
  `Estado` enum('en_proceso','completado','cancelado') COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_LoteCoccion_Producto` (`ProductoId`),
  KEY `IX_LoteCoccion_VaporeraId` (`VaporeraId`),
  KEY `IX_LoteCoccion_Estado` (`Estado`),
  CONSTRAINT `FK_LoteCoccion_Producto` FOREIGN KEY (`ProductoId`) REFERENCES `Producto` (`Id`),
  CONSTRAINT `FK_LoteCoccion_Vaporera` FOREIGN KEY (`VaporeraId`) REFERENCES `Vaporera` (`Id`),
  CONSTRAINT `CK_LoteCoccion_Cantidad` CHECK ((`Cantidad` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LoteCoccion`
--

LOCK TABLES `LoteCoccion` WRITE;
/*!40000 ALTER TABLE `LoteCoccion` DISABLE KEYS */;
INSERT INTO `LoteCoccion` VALUES (1,2,3,48,'2025-10-29 03:07:33','2025-10-29 06:07:33',NULL,'en_proceso'),(2,1,3,36,'2025-10-29 00:07:33','2025-10-29 03:07:33','2025-10-29 02:07:33','completado');
/*!40000 ALTER TABLE `LoteCoccion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MasaTamal`
--

DROP TABLE IF EXISTS `MasaTamal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MasaTamal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MasaTamal`
--

LOCK TABLES `MasaTamal` WRITE;
/*!40000 ALTER TABLE `MasaTamal` DISABLE KEYS */;
INSERT INTO `MasaTamal` VALUES (1,'Maíz Amarillo',0.00,1),(2,'Maíz Blanco',0.50,1),(3,'Arroz',1.00,1);
/*!40000 ALTER TABLE `MasaTamal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MateriaPrima`
--

DROP TABLE IF EXISTS `MateriaPrima`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MateriaPrima` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Categoria` enum('masa','proteina','grano','endulzante','especia','hoja','empaque','combustible') COLLATE utf8mb4_unicode_ci NOT NULL,
  `UnidadMedida` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `StockActual` decimal(10,2) NOT NULL DEFAULT '0.00',
  `StockMinimo` decimal(10,2) NOT NULL DEFAULT '0.00',
  `CostoPromedio` decimal(10,2) NOT NULL DEFAULT '0.00',
  `PuntoCritico` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_MateriaPrima_Categoria` (`Categoria`),
  KEY `IX_MateriaPrima_PuntoCritico` (`PuntoCritico`),
  CONSTRAINT `CK_MateriaPrima_StockActual` CHECK ((`StockActual` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MateriaPrima`
--

LOCK TABLES `MateriaPrima` WRITE;
/*!40000 ALTER TABLE `MateriaPrima` DISABLE KEYS */;
INSERT INTO `MateriaPrima` VALUES (1,'Harina de Maíz Amarillo','masa','kg',690.00,100.00,8.50,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(2,'Harina de Maíz Blanco','masa','kg',400.00,80.00,9.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(3,'Harina de Arroz','masa','kg',200.00,50.00,12.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(4,'Carne de Cerdo','proteina','kg',195.00,30.00,35.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(5,'Pollo','proteina','kg',118.00,25.00,28.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(6,'Chipilín','grano','kg',50.00,10.00,15.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(7,'Recado Rojo','especia','kg',30.00,5.00,45.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(8,'Recado Negro','especia','kg',25.00,5.00,50.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(9,'Chile Guaque','especia','kg',20.00,3.00,65.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(10,'Sal','especia','kg',100.00,20.00,3.50,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(11,'Hojas de Plátano','hoja','unidad',1500.00,200.00,0.50,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(12,'Tusa de Maíz','hoja','unidad',800.00,150.00,0.30,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(13,'Granos de Elote','grano','kg',200.00,40.00,12.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(14,'Maíz para Atole','grano','kg',300.00,60.00,8.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(15,'Harina de Pinol','grano','kg',150.00,30.00,18.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(16,'Cacao en Grano','grano','kg',100.00,20.00,85.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(17,'Panela','endulzante','kg',250.00,50.00,15.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(18,'Miel de Abeja','endulzante','litro',80.00,15.00,95.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(19,'Malvaviscos','especia','kg',40.00,10.00,28.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(20,'Canela en Raja','especia','kg',30.00,5.00,120.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(21,'Chocolate para Rallar','especia','kg',25.00,5.00,95.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(22,'Bolsas Plásticas Pequeñas','empaque','unidad',5000.00,1000.00,0.15,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(23,'Bolsas Plásticas Grandes','empaque','unidad',3000.00,500.00,0.25,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(24,'Vasos 12oz Desechables','empaque','unidad',2000.00,400.00,0.80,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(25,'Jarros Plásticos 1L','empaque','unidad',1000.00,200.00,3.50,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(26,'Termos de Barro','empaque','unidad',50.00,10.00,85.00,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(27,'Gas Propano','combustible','kg',200.00,40.00,7.50,0,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(28,'Leña','combustible','kg',500.00,100.00,2.50,0,'2025-10-29 05:07:18','2025-10-29 05:07:18');
/*!40000 ALTER TABLE `MateriaPrima` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`%`*/ /*!50003 TRIGGER `TR_MateriaPrima_NotificarPuntoCritico` AFTER UPDATE ON `MateriaPrima` FOR EACH ROW BEGIN
    -- Solo notificar cuando cambia de NO crítico a crítico
    IF NEW.PuntoCritico = TRUE AND OLD.PuntoCritico = FALSE THEN
        -- Insertar notificación para todos los gerentes y admins
        INSERT INTO Notificacion (UsuarioId, Tipo, Titulo, Mensaje)
        SELECT 
            u.Id,
            'inventario',
            'Alerta: Stock Crítico',
            CONCAT('La materia prima "', NEW.Nombre, '" ha alcanzado el punto crítico. Stock actual: ', NEW.StockActual, ' ', NEW.UnidadMedida)
        FROM Usuario u
        WHERE u.Rol IN ('admin', 'gerente')
          AND u.Activo = TRUE;
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `MovimientoInventario`
--

DROP TABLE IF EXISTS `MovimientoInventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MovimientoInventario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MateriaPrimaId` int NOT NULL,
  `Tipo` enum('entrada','salida','merma') COLLATE utf8mb4_unicode_ci NOT NULL,
  `Cantidad` decimal(10,2) NOT NULL,
  `CostoUnitario` decimal(10,2) NOT NULL DEFAULT '0.00',
  `CostoTotal` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Motivo` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `UsuarioId` int NOT NULL,
  `Fecha` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_MovimientoInventario_Usuario` (`UsuarioId`),
  KEY `IX_MovimientoInventario_MateriaPrimaId` (`MateriaPrimaId`),
  KEY `IX_MovimientoInventario_Fecha` (`Fecha`),
  KEY `IX_MovimientoInventario_Tipo` (`Tipo`),
  CONSTRAINT `FK_MovimientoInventario_MateriaPrima` FOREIGN KEY (`MateriaPrimaId`) REFERENCES `MateriaPrima` (`Id`),
  CONSTRAINT `FK_MovimientoInventario_Usuario` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuario` (`Id`),
  CONSTRAINT `CK_MovimientoInventario_Cantidad` CHECK ((`Cantidad` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MovimientoInventario`
--

LOCK TABLES `MovimientoInventario` WRITE;
/*!40000 ALTER TABLE `MovimientoInventario` DISABLE KEYS */;
INSERT INTO `MovimientoInventario` VALUES (1,1,'entrada',200.00,8.50,1700.00,'Compra inicial',1,'2025-10-19 05:07:18'),(2,4,'entrada',50.00,35.00,1750.00,'Compra semanal',1,'2025-10-22 05:07:18'),(3,11,'entrada',500.00,0.50,250.00,'Compra de hojas',1,'2025-10-24 05:07:18'),(4,1,'salida',10.00,8.50,85.00,'Producción de tamales',3,'2025-10-28 05:07:18'),(5,4,'salida',5.00,35.00,175.00,'Producción de tamales',3,'2025-10-28 05:07:18'),(6,5,'merma',2.00,28.00,56.00,'Pollo en mal estado',2,'2025-10-26 05:07:18');
/*!40000 ALTER TABLE `MovimientoInventario` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`%`*/ /*!50003 TRIGGER `TR_MovimientoInventario_ActualizarStock` AFTER INSERT ON `MovimientoInventario` FOR EACH ROW BEGIN
    DECLARE nuevo_stock DECIMAL(10,2);
    
    -- Calcular nuevo stock según el tipo de movimiento
    IF NEW.Tipo = 'entrada' THEN
        SET nuevo_stock = (SELECT StockActual FROM MateriaPrima WHERE Id = NEW.MateriaPrimaId) + NEW.Cantidad;
    ELSE -- 'salida' o 'merma'
        SET nuevo_stock = (SELECT StockActual FROM MateriaPrima WHERE Id = NEW.MateriaPrimaId) - NEW.Cantidad;
    END IF;
    
    -- Actualizar stock y punto crítico
    UPDATE MateriaPrima
    SET StockActual = nuevo_stock,
        PuntoCritico = IF(nuevo_stock <= StockMinimo, TRUE, FALSE)
    WHERE Id = NEW.MateriaPrimaId;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `Notificacion`
--

DROP TABLE IF EXISTS `Notificacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Notificacion` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UsuarioId` int NOT NULL,
  `Tipo` enum('venta','coccion','inventario') COLLATE utf8mb4_unicode_ci NOT NULL,
  `Titulo` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Mensaje` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Leida` tinyint(1) NOT NULL DEFAULT '0',
  `Fecha` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_Notificacion_UsuarioId` (`UsuarioId`),
  KEY `IX_Notificacion_Leida` (`Leida`),
  KEY `IX_Notificacion_Fecha` (`Fecha`),
  CONSTRAINT `FK_Notificacion_Usuario` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuario` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Notificacion`
--

LOCK TABLES `Notificacion` WRITE;
/*!40000 ALTER TABLE `Notificacion` DISABLE KEYS */;
INSERT INTO `Notificacion` VALUES (1,2,'inventario','Stock Bajo','El stock de Harina de Arroz está por debajo del mínimo',0,'2025-10-29 05:07:33'),(2,3,'venta','Nueva Venta','Se ha registrado una nueva venta por Q 150.00',1,'2025-10-29 03:07:33'),(3,2,'coccion','Lote Completado','El lote de 36 tamales ha finalizado su cocción',1,'2025-10-29 02:07:33');
/*!40000 ALTER TABLE `Notificacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OrdenCompra`
--

DROP TABLE IF EXISTS `OrdenCompra`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrdenCompra` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProveedorId` int NOT NULL,
  `UsuarioId` int NOT NULL,
  `Total` decimal(10,2) NOT NULL,
  `Estado` enum('pendiente','recibida','cancelada') COLLATE utf8mb4_unicode_ci NOT NULL,
  `FechaOrden` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaEntrega` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_OrdenCompra_Usuario` (`UsuarioId`),
  KEY `IX_OrdenCompra_ProveedorId` (`ProveedorId`),
  KEY `IX_OrdenCompra_Estado` (`Estado`),
  CONSTRAINT `FK_OrdenCompra_Proveedor` FOREIGN KEY (`ProveedorId`) REFERENCES `Proveedor` (`Id`),
  CONSTRAINT `FK_OrdenCompra_Usuario` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OrdenCompra`
--

LOCK TABLES `OrdenCompra` WRITE;
/*!40000 ALTER TABLE `OrdenCompra` DISABLE KEYS */;
INSERT INTO `OrdenCompra` VALUES (1,1,2,5000.00,'recibida','2025-10-14 05:07:18','2025-10-19 05:07:18');
/*!40000 ALTER TABLE `OrdenCompra` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `OrdenCompraDetalle`
--

DROP TABLE IF EXISTS `OrdenCompraDetalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrdenCompraDetalle` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OrdenCompraId` int NOT NULL,
  `MateriaPrimaId` int NOT NULL,
  `Cantidad` decimal(10,2) NOT NULL,
  `PrecioUnitario` decimal(10,2) NOT NULL,
  `Subtotal` decimal(10,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_OrdenCompraDetalle_MateriaPrima` (`MateriaPrimaId`),
  KEY `IX_OrdenCompraDetalle_OrdenCompraId` (`OrdenCompraId`),
  CONSTRAINT `FK_OrdenCompraDetalle_MateriaPrima` FOREIGN KEY (`MateriaPrimaId`) REFERENCES `MateriaPrima` (`Id`),
  CONSTRAINT `FK_OrdenCompraDetalle_OrdenCompra` FOREIGN KEY (`OrdenCompraId`) REFERENCES `OrdenCompra` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_OrdenCompraDetalle_Cantidad` CHECK ((`Cantidad` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `OrdenCompraDetalle`
--

LOCK TABLES `OrdenCompraDetalle` WRITE;
/*!40000 ALTER TABLE `OrdenCompraDetalle` DISABLE KEYS */;
INSERT INTO `OrdenCompraDetalle` VALUES (1,1,1,200.00,8.50,1700.00),(2,1,2,150.00,9.00,1350.00),(3,1,3,100.00,12.00,1200.00),(4,1,17,50.00,15.00,750.00);
/*!40000 ALTER TABLE `OrdenCompraDetalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PicanteTamal`
--

DROP TABLE IF EXISTS `PicanteTamal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PicanteTamal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PicanteTamal`
--

LOCK TABLES `PicanteTamal` WRITE;
/*!40000 ALTER TABLE `PicanteTamal` DISABLE KEYS */;
INSERT INTO `PicanteTamal` VALUES (1,'Sin Chile',0.00,1),(2,'Suave',0.00,1),(3,'Chapín',0.50,1);
/*!40000 ALTER TABLE `PicanteTamal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PreferenciaBebida`
--

DROP TABLE IF EXISTS `PreferenciaBebida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PreferenciaBebida` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TipoBebidaId` int NOT NULL,
  `FranjaHoraria` enum('madrugada','manana','tarde','noche') COLLATE utf8mb4_unicode_ci NOT NULL,
  `CantidadVendida` int NOT NULL DEFAULT '0',
  `Fecha` date NOT NULL,
  `SucursalId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_PreferenciaBebida_TipoBebida` (`TipoBebidaId`),
  KEY `IX_PreferenciaBebida_Fecha` (`Fecha`),
  KEY `IX_PreferenciaBebida_SucursalId` (`SucursalId`),
  CONSTRAINT `FK_PreferenciaBebida_Sucursal` FOREIGN KEY (`SucursalId`) REFERENCES `Sucursal` (`Id`),
  CONSTRAINT `FK_PreferenciaBebida_TipoBebida` FOREIGN KEY (`TipoBebidaId`) REFERENCES `TipoBebida` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PreferenciaBebida`
--

LOCK TABLES `PreferenciaBebida` WRITE;
/*!40000 ALTER TABLE `PreferenciaBebida` DISABLE KEYS */;
INSERT INTO `PreferenciaBebida` VALUES (1,1,'madrugada',15,'2025-10-28',1),(2,1,'manana',25,'2025-10-28',1),(3,4,'tarde',12,'2025-10-28',1),(4,2,'noche',8,'2025-10-28',1),(5,1,'madrugada',20,'2025-10-29',1);
/*!40000 ALTER TABLE `PreferenciaBebida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Producto`
--

DROP TABLE IF EXISTS `Producto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Producto` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Tipo` enum('tamal','bebida') COLLATE utf8mb4_unicode_ci NOT NULL,
  `PrecioBase` decimal(10,2) NOT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_Producto_Tipo` (`Tipo`),
  KEY `IX_Producto_Activo` (`Activo`),
  CONSTRAINT `CK_Producto_PrecioBase` CHECK ((`PrecioBase` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Producto`
--

LOCK TABLES `Producto` WRITE;
/*!40000 ALTER TABLE `Producto` DISABLE KEYS */;
INSERT INTO `Producto` VALUES (1,'Tamal Unitario','tamal',8.00,1,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(2,'Media Docena Tamales','tamal',45.00,1,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(3,'Docena Tamales','tamal',85.00,1,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(4,'Bebida Vaso 12oz','bebida',15.00,1,'2025-10-29 05:07:18','2025-10-29 05:07:18'),(5,'Bebida Jarro 1L','bebida',35.00,1,'2025-10-29 05:07:18','2025-10-29 05:07:18');
/*!40000 ALTER TABLE `Producto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Proveedor`
--

DROP TABLE IF EXISTS `Proveedor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Proveedor` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Contacto` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Telefono` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Direccion` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Proveedor`
--

LOCK TABLES `Proveedor` WRITE;
/*!40000 ALTER TABLE `Proveedor` DISABLE KEYS */;
INSERT INTO `Proveedor` VALUES (1,'Distribuidora La Esperanza','Roberto Morales','5555-1234','ventas@laesperanza.gt','Zona 12, Ciudad de Guatemala',1),(2,'Granos del Campo','María Pérez','5555-5678','info@granodelcampo.gt','Chimaltenango',1),(3,'Carnicería El Buen Sabor','José García','5555-9012','pedidos@buensabor.gt','Zona 5, Ciudad de Guatemala',1),(4,'Empaque Total','Ana Rodríguez','5555-3456','ventas@empaquetotal.gt','Zona 4, Mixco',1);
/*!40000 ALTER TABLE `Proveedor` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RecetaProducto`
--

DROP TABLE IF EXISTS `RecetaProducto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RecetaProducto` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProductoId` int NOT NULL,
  `MateriaPrimaId` int NOT NULL,
  `CantidadNecesaria` decimal(10,2) NOT NULL,
  `Unidad` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RecetaProducto_ProductoId` (`ProductoId`),
  KEY `IX_RecetaProducto_MateriaPrimaId` (`MateriaPrimaId`),
  CONSTRAINT `FK_RecetaProducto_MateriaPrima` FOREIGN KEY (`MateriaPrimaId`) REFERENCES `MateriaPrima` (`Id`),
  CONSTRAINT `FK_RecetaProducto_Producto` FOREIGN KEY (`ProductoId`) REFERENCES `Producto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_RecetaProducto_CantidadNecesaria` CHECK ((`CantidadNecesaria` > 0))
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RecetaProducto`
--

LOCK TABLES `RecetaProducto` WRITE;
/*!40000 ALTER TABLE `RecetaProducto` DISABLE KEYS */;
INSERT INTO `RecetaProducto` VALUES (1,1,1,0.15,'kg'),(2,1,4,0.08,'kg'),(3,1,7,0.01,'kg'),(4,1,11,1.00,'unidad'),(5,1,22,1.00,'unidad'),(6,1,27,0.02,'kg'),(7,2,1,0.90,'kg'),(8,2,4,0.48,'kg'),(9,2,7,0.06,'kg'),(10,2,11,6.00,'unidad'),(11,2,23,1.00,'unidad'),(12,2,27,0.12,'kg'),(13,3,1,1.80,'kg'),(14,3,4,0.96,'kg'),(15,3,7,0.12,'kg'),(16,3,11,12.00,'unidad'),(17,3,23,1.00,'unidad'),(18,3,27,0.24,'kg'),(19,4,13,0.10,'kg'),(20,4,17,0.03,'kg'),(21,4,24,1.00,'unidad'),(22,4,27,0.01,'kg'),(23,5,13,0.35,'kg'),(24,5,17,0.10,'kg'),(25,5,25,1.00,'unidad'),(26,5,27,0.03,'kg');
/*!40000 ALTER TABLE `RecetaProducto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RellenoTamal`
--

DROP TABLE IF EXISTS `RellenoTamal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RellenoTamal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RellenoTamal`
--

LOCK TABLES `RellenoTamal` WRITE;
/*!40000 ALTER TABLE `RellenoTamal` DISABLE KEYS */;
INSERT INTO `RellenoTamal` VALUES (1,'Recado Rojo de Cerdo',0.00,1),(2,'Negro de Pollo',0.50,1),(3,'Chipilín Vegetariano',0.00,1),(4,'Mezcla Estilo Chuchito',1.00,1);
/*!40000 ALTER TABLE `RellenoTamal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ReporteDiario`
--

DROP TABLE IF EXISTS `ReporteDiario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ReporteDiario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SucursalId` int NOT NULL,
  `Fecha` date NOT NULL,
  `VentasTotal` decimal(10,2) NOT NULL DEFAULT '0.00',
  `TamalesVendidos` int NOT NULL DEFAULT '0',
  `BebidasVendidas` int NOT NULL DEFAULT '0',
  `UtilidadBruta` decimal(10,2) NOT NULL DEFAULT '0.00',
  `TamalMasVendido` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BebidaMasVendida` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DesperdicioTotal` decimal(10,2) NOT NULL DEFAULT '0.00',
  `GeneradoAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_ReporteDiario_SucursalFecha` (`SucursalId`,`Fecha`),
  KEY `IX_ReporteDiario_Fecha` (`Fecha`),
  CONSTRAINT `FK_ReporteDiario_Sucursal` FOREIGN KEY (`SucursalId`) REFERENCES `Sucursal` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ReporteDiario`
--

LOCK TABLES `ReporteDiario` WRITE;
/*!40000 ALTER TABLE `ReporteDiario` DISABLE KEYS */;
INSERT INTO `ReporteDiario` VALUES (1,1,'2025-10-28',250.00,17,8,120.00,'Recado Rojo de Cerdo','Atol de Elote',56.00,'2025-10-29 05:07:33'),(2,1,'2025-10-29',150.00,12,2,75.00,'Recado Rojo de Cerdo','Atol de Elote',0.00,'2025-10-29 05:07:33'),(3,2,'2025-10-27',420.00,36,4,200.00,'Recado Rojo de Cerdo','Cacao Batido',0.00,'2025-10-29 05:07:33');
/*!40000 ALTER TABLE `ReporteDiario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Sucursal`
--

DROP TABLE IF EXISTS `Sucursal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Sucursal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Direccion` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Telefono` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Activa` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  CONSTRAINT `CK_Sucursal_Nombre` CHECK ((char_length(`Nombre`) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Sucursal`
--

LOCK TABLES `Sucursal` WRITE;
/*!40000 ALTER TABLE `Sucursal` DISABLE KEYS */;
INSERT INTO `Sucursal` VALUES (1,'Sucursal Centro','Zona 1, Ciudad de Guatemala','2232-1234',1,'2025-10-29 05:07:18'),(2,'Sucursal Zona 10','Zona 10, Ciudad de Guatemala','2366-5678',1,'2025-10-29 05:07:18'),(3,'Sucursal Antigua','Antigua Guatemala, Sacatepéquez','7832-9012',1,'2025-10-29 05:07:18');
/*!40000 ALTER TABLE `Sucursal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TipoBebida`
--

DROP TABLE IF EXISTS `TipoBebida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TipoBebida` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TipoBebida`
--

LOCK TABLES `TipoBebida` WRITE;
/*!40000 ALTER TABLE `TipoBebida` DISABLE KEYS */;
INSERT INTO `TipoBebida` VALUES (1,'Atol de Elote',0.00,1),(2,'Atole Shuco',0.50,1),(3,'Pinol',0.50,1),(4,'Cacao Batido',1.50,1);
/*!40000 ALTER TABLE `TipoBebida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ToppingBebida`
--

DROP TABLE IF EXISTS `ToppingBebida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ToppingBebida` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CostoAdicional` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ToppingBebida`
--

LOCK TABLES `ToppingBebida` WRITE;
/*!40000 ALTER TABLE `ToppingBebida` DISABLE KEYS */;
INSERT INTO `ToppingBebida` VALUES (1,'Malvaviscos',0.50,1),(2,'Canela',0.25,1),(3,'Ralladura de Cacao',0.75,1);
/*!40000 ALTER TABLE `ToppingBebida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Usuario`
--

DROP TABLE IF EXISTS `Usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Usuario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Email` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PasswordHash` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Rol` enum('admin','vendedor','gerente') COLLATE utf8mb4_unicode_ci NOT NULL,
  `SucursalId` int NOT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`),
  KEY `IX_Usuario_SucursalId` (`SucursalId`),
  KEY `IX_Usuario_Rol` (`Rol`),
  CONSTRAINT `FK_Usuario_Sucursal` FOREIGN KEY (`SucursalId`) REFERENCES `Sucursal` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Usuario`
--

LOCK TABLES `Usuario` WRITE;
/*!40000 ALTER TABLE `Usuario` DISABLE KEYS */;
INSERT INTO `Usuario` VALUES (1,'Juan Pérez','admin@cazuela.gt','$2y$10$hash_placeholder_admin','admin',1,1,'2025-10-29 05:07:18'),(2,'María López','maria@cazuela.gt','$2y$10$hash_placeholder_maria','gerente',1,1,'2025-10-29 05:07:18'),(3,'Carlos García','carlos@cazuela.gt','$2y$10$hash_placeholder_carlos','vendedor',2,1,'2025-10-29 05:07:18'),(4,'Ana Martínez','ana@cazuela.gt','$2y$10$hash_placeholder_ana','vendedor',3,1,'2025-10-29 05:07:18'),(5,'jose andres solorzano garcia','solorzanog.ja97@gmail.com','','vendedor',1,1,'2025-10-31 15:46:46');
/*!40000 ALTER TABLE `Usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `VW_InventarioActual`
--

DROP TABLE IF EXISTS `VW_InventarioActual`;
/*!50001 DROP VIEW IF EXISTS `VW_InventarioActual`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `VW_InventarioActual` AS SELECT 
 1 AS `Id`,
 1 AS `Nombre`,
 1 AS `Categoria`,
 1 AS `StockActual`,
 1 AS `UnidadMedida`,
 1 AS `StockMinimo`,
 1 AS `CostoPromedio`,
 1 AS `PuntoCritico`,
 1 AS `EstadoStock`,
 1 AS `ValorInventario`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `VW_TopProductosVendidos`
--

DROP TABLE IF EXISTS `VW_TopProductosVendidos`;
/*!50001 DROP VIEW IF EXISTS `VW_TopProductosVendidos`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `VW_TopProductosVendidos` AS SELECT 
 1 AS `Id`,
 1 AS `Nombre`,
 1 AS `Tipo`,
 1 AS `NumeroVentas`,
 1 AS `CantidadTotal`,
 1 AS `VentaTotal`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `VW_UtilidadesPorProducto`
--

DROP TABLE IF EXISTS `VW_UtilidadesPorProducto`;
/*!50001 DROP VIEW IF EXISTS `VW_UtilidadesPorProducto`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `VW_UtilidadesPorProducto` AS SELECT 
 1 AS `Id`,
 1 AS `Nombre`,
 1 AS `Tipo`,
 1 AS `PrecioBase`,
 1 AS `CostoProduccion`,
 1 AS `UtilidadUnitaria`,
 1 AS `MargenPorcentaje`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `VW_VentasCompletas`
--

DROP TABLE IF EXISTS `VW_VentasCompletas`;
/*!50001 DROP VIEW IF EXISTS `VW_VentasCompletas`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `VW_VentasCompletas` AS SELECT 
 1 AS `VentaId`,
 1 AS `NumeroOrden`,
 1 AS `Fecha`,
 1 AS `Sucursal`,
 1 AS `Vendedor`,
 1 AS `Subtotal`,
 1 AS `Descuento`,
 1 AS `Total`,
 1 AS `Estado`,
 1 AS `DetalleId`,
 1 AS `Producto`,
 1 AS `Cantidad`,
 1 AS `PrecioUnitario`,
 1 AS `SubtotalDetalle`,
 1 AS `ConfiguracionJson`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `Vaporera`
--

DROP TABLE IF EXISTS `Vaporera`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Vaporera` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SucursalId` int NOT NULL,
  `Estado` enum('disponible','en_uso','mantenimiento') COLLATE utf8mb4_unicode_ci NOT NULL,
  `TemperaturaActual` decimal(5,2) DEFAULT NULL,
  `UltimoLote` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Codigo` (`Codigo`),
  KEY `IX_Vaporera_SucursalId` (`SucursalId`),
  KEY `IX_Vaporera_Estado` (`Estado`),
  CONSTRAINT `FK_Vaporera_Sucursal` FOREIGN KEY (`SucursalId`) REFERENCES `Sucursal` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Vaporera`
--

LOCK TABLES `Vaporera` WRITE;
/*!40000 ALTER TABLE `Vaporera` DISABLE KEYS */;
INSERT INTO `Vaporera` VALUES (1,'VAP-001',1,'disponible',0.00,NULL),(2,'VAP-002',1,'en_uso',95.50,NULL),(3,'VAP-003',2,'disponible',0.00,NULL),(4,'VAP-004',3,'mantenimiento',0.00,NULL);
/*!40000 ALTER TABLE `Vaporera` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Venta`
--

DROP TABLE IF EXISTS `Venta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Venta` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `NumeroOrden` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SucursalId` int NOT NULL,
  `UsuarioId` int NOT NULL,
  `Subtotal` decimal(10,2) NOT NULL,
  `Descuento` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Total` decimal(10,2) NOT NULL,
  `Estado` enum('pendiente','completada','cancelada') COLLATE utf8mb4_unicode_ci NOT NULL,
  `Sincronizada` tinyint(1) NOT NULL DEFAULT '1',
  `Fecha` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `NumeroOrden` (`NumeroOrden`),
  KEY `FK_Venta_Usuario` (`UsuarioId`),
  KEY `IX_Venta_Fecha` (`Fecha`),
  KEY `IX_Venta_SucursalId` (`SucursalId`),
  KEY `IX_Venta_Estado` (`Estado`),
  KEY `IX_Venta_NumeroOrden` (`NumeroOrden`),
  CONSTRAINT `FK_Venta_Sucursal` FOREIGN KEY (`SucursalId`) REFERENCES `Sucursal` (`Id`),
  CONSTRAINT `FK_Venta_Usuario` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Venta`
--

LOCK TABLES `Venta` WRITE;
/*!40000 ALTER TABLE `Venta` DISABLE KEYS */;
INSERT INTO `Venta` VALUES (1,'SUC1-20251029-0001',1,3,100.00,0.00,100.00,'completada',1,'2025-10-28 05:07:18','2025-10-29 05:07:18'),(3,'SUC2-20251029-0001',2,4,420.00,0.00,420.00,'completada',1,'2025-10-27 05:07:33','2025-10-29 05:07:33');
/*!40000 ALTER TABLE `Venta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VentaDetalle`
--

DROP TABLE IF EXISTS `VentaDetalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VentaDetalle` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `VentaId` int NOT NULL,
  `ProductoId` int DEFAULT NULL,
  `ComboId` int DEFAULT NULL,
  `Cantidad` int NOT NULL,
  `PrecioUnitario` decimal(10,2) NOT NULL,
  `Subtotal` decimal(10,2) NOT NULL,
  `ConfiguracionJson` json DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VentaDetalle_VentaId` (`VentaId`),
  KEY `IX_VentaDetalle_ProductoId` (`ProductoId`),
  KEY `IX_VentaDetalle_ComboId` (`ComboId`),
  CONSTRAINT `FK_VentaDetalle_Combo` FOREIGN KEY (`ComboId`) REFERENCES `Combo` (`Id`),
  CONSTRAINT `FK_VentaDetalle_Producto` FOREIGN KEY (`ProductoId`) REFERENCES `Producto` (`Id`),
  CONSTRAINT `FK_VentaDetalle_Venta` FOREIGN KEY (`VentaId`) REFERENCES `Venta` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_VentaDetalle_Cantidad` CHECK ((`Cantidad` > 0)),
  CONSTRAINT `CK_VentaDetalle_ProductoOCombo` CHECK ((((`ProductoId` is not null) and (`ComboId` is null)) or ((`ProductoId` is null) and (`ComboId` is not null))))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VentaDetalle`
--

LOCK TABLES `VentaDetalle` WRITE;
/*!40000 ALTER TABLE `VentaDetalle` DISABLE KEYS */;
INSERT INTO `VentaDetalle` VALUES (1,1,1,NULL,5,8.00,40.00,'{\"masa\": \"Maíz Amarillo\", \"picante\": \"Chapín\", \"relleno\": \"Recado Rojo de Cerdo\", \"envoltura\": \"Hoja de Plátano\"}'),(2,1,4,NULL,4,15.00,60.00,'{\"tipo\": \"Atol de Elote\", \"topping\": \"Canela\", \"endulzante\": \"Panela\"}'),(3,1,NULL,1,1,150.00,150.00,NULL),(4,3,NULL,2,1,420.00,420.00,NULL);
/*!40000 ALTER TABLE `VentaDetalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'LaCazuelaChapina'
--
/*!50003 DROP FUNCTION IF EXISTS `FN_CalcularPrecioConAtributos` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` FUNCTION `FN_CalcularPrecioConAtributos`(
    p_PrecioBase DECIMAL(10,2),
    p_ConfiguracionJson JSON
) RETURNS decimal(10,2)
    READS SQL DATA
    DETERMINISTIC
BEGIN
    DECLARE v_PrecioFinal DECIMAL(10,2);
    
    -- Por ahora retorna el precio base
    -- En implementación real se parsearía el JSON y sumarían costos adicionales
    SET v_PrecioFinal = p_PrecioBase;
    
    RETURN v_PrecioFinal;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_GenerarNumeroOrden` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `SP_GenerarNumeroOrden`(
    IN p_SucursalId INT,
    OUT p_NumeroOrden VARCHAR(50)
)
BEGIN
    DECLARE v_Fecha VARCHAR(8);
    DECLARE v_Secuencia INT;
    
    SET v_Fecha = DATE_FORMAT(NOW(), '%Y%m%d');
    
    SELECT IFNULL(MAX(CAST(SUBSTRING(NumeroOrden, -4) AS UNSIGNED)), 0) + 1
    INTO v_Secuencia
    FROM Venta
    WHERE SucursalId = p_SucursalId
      AND DATE(Fecha) = CURDATE();
    
    SET p_NumeroOrden = CONCAT('SUC', p_SucursalId, '-', v_Fecha, '-', LPAD(v_Secuencia, 4, '0'));
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_GenerarReporteDiario` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `SP_GenerarReporteDiario`(
    IN p_SucursalId INT,
    IN p_Fecha DATE
)
BEGIN
    DECLARE v_VentasTotal DECIMAL(10,2);
    DECLARE v_TamalesVendidos INT;
    DECLARE v_BebidasVendidas INT;
    DECLARE v_UtilidadBruta DECIMAL(10,2);
    DECLARE v_DesperdicioTotal DECIMAL(10,2);
    
    -- Calcular ventas totales
    SELECT IFNULL(SUM(Total), 0)
    INTO v_VentasTotal
    FROM Venta
    WHERE SucursalId = p_SucursalId
      AND DATE(Fecha) = p_Fecha
      AND Estado = 'completada';
    
    -- Contar tamales y bebidas vendidos
    SELECT 
        IFNULL(SUM(CASE WHEN p.Tipo = 'tamal' THEN vd.Cantidad ELSE 0 END), 0),
        IFNULL(SUM(CASE WHEN p.Tipo = 'bebida' THEN vd.Cantidad ELSE 0 END), 0)
    INTO v_TamalesVendidos, v_BebidasVendidas
    FROM VentaDetalle vd
    INNER JOIN Venta v ON vd.VentaId = v.Id
    INNER JOIN Producto p ON vd.ProductoId = p.Id
    WHERE v.SucursalId = p_SucursalId
      AND DATE(v.Fecha) = p_Fecha
      AND v.Estado = 'completada';
    
    -- Calcular utilidad (simplificado - 45% margen)
    SET v_UtilidadBruta = v_VentasTotal * 0.45;
    
    -- Desperdicio del día
    SELECT IFNULL(SUM(CostoTotal), 0)
    INTO v_DesperdicioTotal
    FROM MovimientoInventario
    WHERE Tipo = 'merma'
      AND DATE(Fecha) = p_Fecha;
    
    -- Insertar o actualizar reporte
    INSERT INTO ReporteDiario (
        SucursalId, Fecha, VentasTotal, TamalesVendidos, BebidasVendidas, 
        UtilidadBruta, TamalMasVendido, BebidaMasVendida, DesperdicioTotal
    )
    VALUES (
        p_SucursalId, p_Fecha, v_VentasTotal, v_TamalesVendidos, v_BebidasVendidas,
        v_UtilidadBruta, 'Recado Rojo de Cerdo', 'Atol de Elote', v_DesperdicioTotal
    )
    ON DUPLICATE KEY UPDATE
        VentasTotal = v_VentasTotal,
        TamalesVendidos = v_TamalesVendidos,
        BebidasVendidas = v_BebidasVendidas,
        UtilidadBruta = v_UtilidadBruta,
        DesperdicioTotal = v_DesperdicioTotal,
        GeneradoAt = CURRENT_TIMESTAMP;
    
    SELECT 'Reporte generado exitosamente' AS Mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_VerificarInventarioParaVenta` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `SP_VerificarInventarioParaVenta`(
    IN p_ProductoId INT,
    IN p_Cantidad INT,
    OUT p_Disponible BOOLEAN,
    OUT p_MensajeError VARCHAR(500)
)
BEGIN
    DECLARE v_Terminado BOOLEAN DEFAULT FALSE;
    DECLARE v_MateriaPrimaId INT;
    DECLARE v_CantidadNecesaria DECIMAL(10,2);
    DECLARE v_StockActual DECIMAL(10,2);
    DECLARE v_NombreMateria VARCHAR(200);
    
    DECLARE cur CURSOR FOR
        SELECT rp.MateriaPrimaId, rp.CantidadNecesaria * p_Cantidad, mp.StockActual, mp.Nombre
        FROM RecetaProducto rp
        INNER JOIN MateriaPrima mp ON rp.MateriaPrimaId = mp.Id
        WHERE rp.ProductoId = p_ProductoId;
    
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_Terminado = TRUE;
    
    SET p_Disponible = TRUE;
    SET p_MensajeError = NULL;
    
    OPEN cur;
    
    leer_loop: LOOP
        FETCH cur INTO v_MateriaPrimaId, v_CantidadNecesaria, v_StockActual, v_NombreMateria;
        
        IF v_Terminado THEN
            LEAVE leer_loop;
        END IF;
        
        IF v_StockActual < v_CantidadNecesaria THEN
            SET p_Disponible = FALSE;
            SET p_MensajeError = CONCAT('Stock insuficiente de: ', v_NombreMateria);
            LEAVE leer_loop;
        END IF;
    END LOOP;
    
    CLOSE cur;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `VW_InventarioActual`
--

/*!50001 DROP VIEW IF EXISTS `VW_InventarioActual`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `VW_InventarioActual` AS select `mp`.`Id` AS `Id`,`mp`.`Nombre` AS `Nombre`,`mp`.`Categoria` AS `Categoria`,`mp`.`StockActual` AS `StockActual`,`mp`.`UnidadMedida` AS `UnidadMedida`,`mp`.`StockMinimo` AS `StockMinimo`,`mp`.`CostoPromedio` AS `CostoPromedio`,`mp`.`PuntoCritico` AS `PuntoCritico`,(case when (`mp`.`PuntoCritico` = true) then 'CRÍTICO' when (`mp`.`StockActual` <= (`mp`.`StockMinimo` * 1.5)) then 'BAJO' else 'NORMAL' end) AS `EstadoStock`,(`mp`.`StockActual` * `mp`.`CostoPromedio`) AS `ValorInventario` from `MateriaPrima` `mp` where (`mp`.`StockActual` > 0) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `VW_TopProductosVendidos`
--

/*!50001 DROP VIEW IF EXISTS `VW_TopProductosVendidos`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `VW_TopProductosVendidos` AS select `p`.`Id` AS `Id`,`p`.`Nombre` AS `Nombre`,`p`.`Tipo` AS `Tipo`,count(`vd`.`Id`) AS `NumeroVentas`,sum(`vd`.`Cantidad`) AS `CantidadTotal`,sum(`vd`.`Subtotal`) AS `VentaTotal` from ((`Producto` `p` join `VentaDetalle` `vd` on((`p`.`Id` = `vd`.`ProductoId`))) join `Venta` `v` on((`vd`.`VentaId` = `v`.`Id`))) where (`v`.`Estado` = 'completada') group by `p`.`Id`,`p`.`Nombre`,`p`.`Tipo` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `VW_UtilidadesPorProducto`
--

/*!50001 DROP VIEW IF EXISTS `VW_UtilidadesPorProducto`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `VW_UtilidadesPorProducto` AS select `p`.`Id` AS `Id`,`p`.`Nombre` AS `Nombre`,`p`.`Tipo` AS `Tipo`,`p`.`PrecioBase` AS `PrecioBase`,sum((`rp`.`CantidadNecesaria` * `mp`.`CostoPromedio`)) AS `CostoProduccion`,(`p`.`PrecioBase` - sum((`rp`.`CantidadNecesaria` * `mp`.`CostoPromedio`))) AS `UtilidadUnitaria`,(((`p`.`PrecioBase` - sum((`rp`.`CantidadNecesaria` * `mp`.`CostoPromedio`))) / `p`.`PrecioBase`) * 100) AS `MargenPorcentaje` from ((`Producto` `p` join `RecetaProducto` `rp` on((`p`.`Id` = `rp`.`ProductoId`))) join `MateriaPrima` `mp` on((`rp`.`MateriaPrimaId` = `mp`.`Id`))) group by `p`.`Id`,`p`.`Nombre`,`p`.`Tipo`,`p`.`PrecioBase` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `VW_VentasCompletas`
--

/*!50001 DROP VIEW IF EXISTS `VW_VentasCompletas`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `VW_VentasCompletas` AS select `v`.`Id` AS `VentaId`,`v`.`NumeroOrden` AS `NumeroOrden`,`v`.`Fecha` AS `Fecha`,`s`.`Nombre` AS `Sucursal`,`u`.`Nombre` AS `Vendedor`,`v`.`Subtotal` AS `Subtotal`,`v`.`Descuento` AS `Descuento`,`v`.`Total` AS `Total`,`v`.`Estado` AS `Estado`,`vd`.`Id` AS `DetalleId`,ifnull(`p`.`Nombre`,`c`.`Nombre`) AS `Producto`,`vd`.`Cantidad` AS `Cantidad`,`vd`.`PrecioUnitario` AS `PrecioUnitario`,`vd`.`Subtotal` AS `SubtotalDetalle`,`vd`.`ConfiguracionJson` AS `ConfiguracionJson` from (((((`Venta` `v` join `Sucursal` `s` on((`v`.`SucursalId` = `s`.`Id`))) join `Usuario` `u` on((`v`.`UsuarioId` = `u`.`Id`))) left join `VentaDetalle` `vd` on((`v`.`Id` = `vd`.`VentaId`))) left join `Producto` `p` on((`vd`.`ProductoId` = `p`.`Id`))) left join `Combo` `c` on((`vd`.`ComboId` = `c`.`Id`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-10-31 16:52:07
