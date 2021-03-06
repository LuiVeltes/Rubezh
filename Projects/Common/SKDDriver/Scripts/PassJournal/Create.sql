USE master
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'PassJournal_0')
BEGIN
	SET NOEXEC ON
END
GO
CREATE DATABASE PassJournal_0
GO
USE PassJournal_0

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TYPE SqlTime FROM DATETIME

CREATE TYPE SqlDate FROM DATETIME
GO
CREATE RULE TimeOnlyRule AS
	datediff(dd, 0, @DateTime) = 0
GO
CREATE RULE DateOnlyRule AS
	dateAdd(dd, datediff(dd,0,@DateTime), 0) = @DateTime
GO
EXEC sp_bindrule 'TimeOnlyRule', 'SqlTime'
EXEC sp_bindrule 'DateOnlyRule', 'SqlDate'

CREATE TABLE [dbo].[PassJournal](
	[UID] [uniqueidentifier] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[ZoneUID] [uniqueidentifier] NOT NULL,
	[EnterTime] [datetime] NOT NULL,
	[ExitTime] [datetime] NULL,
 CONSTRAINT [PK_PassJournal] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE INDEX PassJournalUIDIndex ON PassJournal([UID])

CREATE TABLE [dbo].[EmployeeDay](
	[UID] [uniqueidentifier] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[IsIgnoreHoliday] [bit] NOT NULL,
	[IsOnlyFirstEnter] [bit] NOT NULL,
	[AllowedLate] int NOT NULL,
	[AllowedEarlyLeave] int NOT NULL,
	[DayIntervalsString] nvarchar(max) NULL,
	[Date] [datetime] NOT NULL
CONSTRAINT [PK_EmployeeDay] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE INDEX EmployeeDayUIDIndex ON EmployeeDay([UID])
CREATE INDEX EmployeeDayEmployeeUIDIndex ON EmployeeDay([EmployeeUID])

CREATE TABLE Patches (Id nvarchar(100) not null)
INSERT INTO Patches (Id) VALUES
	('EmployeeDay')
INSERT INTO Patches (Id) VALUES
	('EmployeeDayIndexes')

SET NOEXEC OFF