USE [master]
GO

IF EXISTS(SELECT name FROM sys.databases WHERE name = 'Firesec')
BEGIN
	ALTER DATABASE [Firesec] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [Firesec]
END
GO

CREATE DATABASE [Firesec] 
GO

USE [Firesec]
GO

