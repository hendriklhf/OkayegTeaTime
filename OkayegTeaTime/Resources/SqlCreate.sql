/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE IF NOT EXISTS `okayegteatime` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */;
USE `okayegteatime`;
CREATE TABLE IF NOT EXISTS `Channels` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ChannelName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `EmoteInFront` varbinary(100) DEFAULT NULL,
  `Prefix` varbinary(50) DEFAULT NULL,
  `EmoteManagementSub` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
CREATE TABLE IF NOT EXISTS `reminder` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FromUser` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ToUser` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Message` varbinary(2000) NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` bigint(20) NOT NULL DEFAULT 0,
  `ToTime` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2457 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
CREATE TABLE IF NOT EXISTS `spotify` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `AccessToken` varchar(300) COLLATE utf8mb4_unicode_ci NOT NULL,
  `RefreshToken` varchar(300) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` bigint(20) NOT NULL DEFAULT 0,
  `SongRequestEnabled` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
CREATE TABLE IF NOT EXISTS `suggestions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Suggestion` varbinary(2000) NOT NULL,
  `Channel` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` bigint(20) NOT NULL,
  `Done` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=438 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
CREATE TABLE IF NOT EXISTS `Users` (
  `Id` int(11) NOT NULL DEFAULT 0,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `AfkMessage` varbinary(2000) DEFAULT NULL,
  `AfkType` int(11) NOT NULL DEFAULT 0,
  `AfkTime` bigint(20) NOT NULL DEFAULT 0,
  `IsAfk` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
