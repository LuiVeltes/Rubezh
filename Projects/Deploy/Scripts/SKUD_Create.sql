USE [master]
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'SKUD')
BEGIN
	ALTER DATABASE [SKUD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [SKUD]
END
GO
CREATE DATABASE [SKUD] 
GO
USE [SKUD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Photo](
	[UID] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RemovalDate] [datetime] NOT NULL,	
	[Data] [varbinary](MAX) NULL,
CONSTRAINT [PK_Photo] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE TABLE [dbo].[AdditionalColumnType](
	[UID] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RemovalDate] [datetime] NOT NULL,	
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[DataType] [int] NULL,
	[OrganizationUid] [uniqueidentifier] NULL,
CONSTRAINT [PK_AdditionalColumnType] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
CREATE TABLE [dbo].[AdditionalColumn](
	[UID] [uniqueidentifier] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RemovalDate] [datetime] NOT NULL,	
	[EmployeeUID] [uniqueidentifier] NULL,
	[AdditionalColumnTypeUID] [uniqueidentifier] NULL,
	[TextData] [text] NULL,
	[GraphicsData] [varbinary](MAX) NULL,
CONSTRAINT [PK_AdditionalColumn] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Day](
	[UID] [uniqueidentifier] NOT NULL,
	[NamedIntervalUid] [uniqueidentifier] NULL,
	[ScheduleSchemeUid] [uniqueidentifier] NULL,
	[Number] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Day] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Department](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[PhotoUID] [uniqueidentifier] NULL,
	[ParentDepartmentUid] [uniqueidentifier] NULL,
	[ContactEmployeeUid] [uniqueidentifier] NULL,
	[AttendantUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Department_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Employee](
	[UID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[SecondName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[PhotoUID] [uniqueidentifier] NULL, 
	[PositionUid] [uniqueidentifier] NULL,
	[DepartmentUid] [uniqueidentifier] NULL,
	[ScheduleUid] [uniqueidentifier] NULL,
	[Appointed] [datetime] NOT NULL,
	[Dismissed] [datetime] NOT NULL,
	[Type] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Employee_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[EmployeeReplacement](
	[UID] [uniqueidentifier] NOT NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[EmployeeUid] [uniqueidentifier] NULL,
	[DepartmentUid] [uniqueidentifier] NULL,
	[ScheduleUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_EmployeeReplacement] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Holiday](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Type] [int] NULL,
	[Date] [datetime] NULL,
	[TransferDate] [datetime] NULL,
	[Reduction] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Interval](
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Transition] [int] NULL,
	[UID] [uniqueidentifier] NOT NULL,
	[NamedIntervalUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
CONSTRAINT [PK_Interval_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[NamedInterval](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_NamedInterval_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Phone](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[NumberString] [nvarchar](50) NULL,
	[DepartmentUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Phone] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Position](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Position_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Schedule](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ScheduleSchemeUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Schedule_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleScheme](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Type] [int] NULL,
	[Length] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ScheduleScheme_1] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Document](
	[UID] [uniqueidentifier] NOT NULL,
	[No] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IssueDate] [datetime] NOT NULL,
	[LaunchDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Journal](
	[UID] [uniqueidentifier] NOT NULL,
	[SysemDate] [datetime] NULL,
	[DeviceDate] [datetime] NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[DeviceNo] [int] NULL,
	[IpPort] [nvarchar](50) NULL,
	[CardUid] [uniqueidentifier] NULL,
	[CardSeries] [int] NULL,
	[CardNo] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Card](
	[UID] [uniqueidentifier] NOT NULL,
	[Series] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[EmployeeUid] [uniqueidentifier] NULL,
	[GUDUid] [uniqueidentifier] NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
	[IsAntipass] [bit] NOT NULL,
	[IsInStopList] [bit] NOT NULL,
	[StopReason] [text] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[CardZoneLink](
	[UID] [uniqueidentifier] NOT NULL,
	[ZoneUid] [uniqueidentifier] NOT NULL,
	[ParentUid] [uniqueidentifier] NULL,
	[ParentType] [int] NULL,
	[IsWithEscort] [bit] NOT NULL,
	[IntervalUid] [uniqueidentifier] NULL,
	[IntervalType] [int] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_CardZoneLink] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[GUD](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
	[OrganizationUid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_GUD] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleZoneLink](
	[UID] [uniqueidentifier] NOT NULL,
	[ZoneUid] [uniqueidentifier] NULL,
	[ScheduleUid] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_ScheduleZoneLink] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Organization](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[PhotoUID] [uniqueidentifier] NULL,
	[IsDeleted] [bit] NOT NULL ,
	[RemovalDate] [datetime] NOT NULL ,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE INDEX IntervalUidIndex ON [dbo].[Interval]([Uid])
CREATE INDEX NamedIntervalUidIndex ON NamedInterval([Uid])
CREATE INDEX DayUidIndex ON [Day]([Uid])
CREATE INDEX ScheduleSchemeUidIndex ON ScheduleScheme([Uid])
CREATE INDEX ScheduleUidIndex ON Schedule([Uid])
CREATE INDEX AdditionalColumnUidIndex ON AdditionalColumn([Uid])
CREATE INDEX AdditionalColumnTypeUidIndex ON AdditionalColumnType([Uid])
CREATE INDEX PositionUidIndex ON Position([Uid])
CREATE INDEX EmployeeUidIndex ON Employee([Uid])
CREATE INDEX EmployeeReplacementUidIndex ON EmployeeReplacement([Uid])
CREATE INDEX PhoneUidIndex ON Phone([Uid])
CREATE INDEX DepartmentUidIndex ON Department([Uid])
CREATE INDEX DocumentUidIndex ON [Document]([Uid])
CREATE INDEX HolidayUidIndex ON Holiday([Uid])
CREATE INDEX JournalUidIndex ON Journal([Uid])
CREATE INDEX CardUidIndex ON Card([Uid])
CREATE INDEX CardZoneLinkUidIndex ON CardZoneLink([Uid])
CREATE INDEX ScheduleZoneLinkUidIndex ON ScheduleZoneLink([Uid])
CREATE INDEX OrganizationUidIndex ON Organization([Uid])
CREATE INDEX PhotoUidIndex ON Photo([Uid])
