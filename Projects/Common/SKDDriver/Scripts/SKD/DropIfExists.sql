USE [master]
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'SKD')
BEGIN
	ALTER DATABASE [SKD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [SKD]
END
