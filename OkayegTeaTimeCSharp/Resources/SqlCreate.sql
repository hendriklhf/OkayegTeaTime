-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               10.1.37-MariaDB - mariadb.org binary distribution
-- Server Betriebssystem:        Win32
-- HeidiSQL Version:             11.2.0.6213
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Exportiere Datenbank Struktur für okayegteatime
CREATE DATABASE IF NOT EXISTS `okayegteatime` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */;
USE `okayegteatime`;

-- Exportiere Struktur von Tabelle okayegteatime.bots
CREATE TABLE IF NOT EXISTS `bots` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `OAuth` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Channels` varchar(10000) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.gachi
CREATE TABLE IF NOT EXISTS `gachi` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varbinary(100) NOT NULL,
  `Link` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=124 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.messages
CREATE TABLE IF NOT EXISTS `messages` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  `MessageText` varbinary(500) NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  `Time` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2093906 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.nukes
CREATE TABLE IF NOT EXISTS `nukes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Word` varbinary(250) NOT NULL,
  `TimeoutTime` bigint(20) NOT NULL,
  `ForTime` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.pechkekse
CREATE TABLE IF NOT EXISTS `pechkekse` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Message` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=74 DEFAULT CHARSET=latin1;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.prefix
CREATE TABLE IF NOT EXISTS `prefix` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Prefix` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.quotes
CREATE TABLE IF NOT EXISTS `quotes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `QuoteMessage` varbinary(500) NOT NULL,
  `Submitter` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TargetUser` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.reminder
CREATE TABLE IF NOT EXISTS `reminder` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FromUser` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ToUser` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Message` varbinary(500) NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` bigint(20) NOT NULL DEFAULT '0',
  `ToTime` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=136 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.spotify
CREATE TABLE IF NOT EXISTS `spotify` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `AccessToken` varchar(300) COLLATE utf8mb4_unicode_ci NOT NULL,
  `RefreshToken` varchar(300) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.suggestions
CREATE TABLE IF NOT EXISTS `suggestions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Suggestion` varbinary(500) NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` bigint(20) NOT NULL,
  `Done` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=195 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.users
CREATE TABLE IF NOT EXISTS `users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `MessageText` varbinary(500) DEFAULT NULL,
  `Type` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Time` bigint(20) NOT NULL DEFAULT '0',
  `IsAFK` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'false',
  `Egs` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=21963 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle okayegteatime.yourmom
CREATE TABLE IF NOT EXISTS `yourmom` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MessageText` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=121 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Daten Export vom Benutzer nicht ausgewählt

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
