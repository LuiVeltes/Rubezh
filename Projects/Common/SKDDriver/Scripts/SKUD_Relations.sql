USE [SKD]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[AdditionalColumn]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalColumn_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[AdditionalColumn] NOCHECK CONSTRAINT [FK_AdditionalColumn_Employee]

GO
ALTER TABLE [dbo].[AdditionalColumn]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalColumn_AdditionalColumnType] FOREIGN KEY([AdditionalColumnTypeUID])
REFERENCES [dbo].[AdditionalColumnType] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[AdditionalColumn] NOCHECK CONSTRAINT [FK_AdditionalColumn_AdditionalColumnType] 

GO
ALTER TABLE [dbo].[Day]  WITH NOCHECK ADD  CONSTRAINT [FK_Day_NamedInterval] FOREIGN KEY([NamedIntervalUid])
REFERENCES [dbo].[NamedInterval] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Day] NOCHECK CONSTRAINT [FK_Day_NamedInterval]

GO
ALTER TABLE [dbo].[Day]  WITH NOCHECK ADD  CONSTRAINT [FK_Day_ScheduleScheme] FOREIGN KEY([ScheduleSchemeUid])
REFERENCES [dbo].[ScheduleScheme] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[Day] NOCHECK CONSTRAINT [FK_Day_ScheduleScheme]

GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD  CONSTRAINT [FK_Department_Department1] FOREIGN KEY([ParentDepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Department1]

GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Department1] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Department1]

GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD  CONSTRAINT [FK_Department_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Photo]

GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Position] FOREIGN KEY([PositionUid])
REFERENCES [dbo].[Position] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Position]

GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Employee] FOREIGN KEY([EscortUID])
REFERENCES [dbo].Employee([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Employee]

GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Schedule]

GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD  CONSTRAINT [FK_Employee_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Photo]

GO
ALTER TABLE [dbo].[Position]  WITH NOCHECK ADD  CONSTRAINT [FK_Position_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Position] NOCHECK CONSTRAINT [FK_Position_Photo]

GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH NOCHECK ADD  CONSTRAINT [FK_EmployeeReplacement_Department] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[EmployeeReplacement] NOCHECK CONSTRAINT [FK_EmployeeReplacement_Department]

GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH NOCHECK ADD  CONSTRAINT [FK_EmployeeReplacement_Employee] FOREIGN KEY([EmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[EmployeeReplacement] NOCHECK CONSTRAINT [FK_EmployeeReplacement_Employee]

GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH NOCHECK ADD CONSTRAINT [FK_EmployeeReplacement_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[EmployeeReplacement] NOCHECK CONSTRAINT [FK_EmployeeReplacement_Schedule]

GO
ALTER TABLE [dbo].[Interval]  WITH NOCHECK ADD  CONSTRAINT [FK_Interval_NamedInterval1] FOREIGN KEY([NamedIntervalUid])
REFERENCES [dbo].[NamedInterval] ([Uid])
ON UPDATE CASCADE
ON DELETE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Interval] NOCHECK CONSTRAINT [FK_Interval_NamedInterval1]

GO
ALTER TABLE [dbo].[Phone]  WITH NOCHECK ADD  CONSTRAINT [FK_Phone_Department] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[Phone] NOCHECK CONSTRAINT [FK_Phone_Department]

GO
ALTER TABLE [dbo].[ScheduleZone]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduleZone_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[ScheduleZone] NOCHECK CONSTRAINT [FK_ScheduleZone_Schedule]

GO
ALTER TABLE [dbo].[Schedule]  WITH NOCHECK ADD  CONSTRAINT [FK_Schedule_ScheduleScheme] FOREIGN KEY([ScheduleSchemeUid])
REFERENCES [dbo].[ScheduleScheme] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Schedule] NOCHECK CONSTRAINT [FK_Schedule_ScheduleScheme]

GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD  CONSTRAINT [FK_Department_Employee1] FOREIGN KEY([ContactEmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Employee1]

GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD  CONSTRAINT [FK_Department_Employee2] FOREIGN KEY([AttendantUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Employee2]

GO
ALTER TABLE [dbo].[Journal]  WITH NOCHECK ADD  CONSTRAINT [FK_Journal_Card] FOREIGN KEY([CardUid])
REFERENCES [dbo].[Card] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Journal] NOCHECK CONSTRAINT [FK_Journal_Card]

GO
ALTER TABLE [dbo].[Card]  WITH NOCHECK ADD  CONSTRAINT [FK_Card_Employee] FOREIGN KEY([EmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Card] NOCHECK CONSTRAINT [FK_Card_Employee]

GO
ALTER TABLE [dbo].[Card]  WITH NOCHECK ADD CONSTRAINT [FK_Card_AccessTemplate] FOREIGN KEY([AccessTemplateUid])
REFERENCES [dbo].[AccessTemplate] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Card] NOCHECK CONSTRAINT [FK_Card_AccessTemplate]

GO
ALTER TABLE [dbo].[CardZone]  WITH NOCHECK ADD  CONSTRAINT [FK_CardZone_Card] FOREIGN KEY([ParentUid])
REFERENCES [dbo].[Card] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[CardZone] NOCHECK CONSTRAINT [FK_CardZone_Card]

GO
ALTER TABLE [dbo].[CardZone]  WITH NOCHECK ADD  CONSTRAINT [FK_CardZone_AccessTemplate] FOREIGN KEY([ParentUid])
REFERENCES [dbo].[AccessTemplate] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[CardZone] NOCHECK CONSTRAINT [FK_CardZone_AccessTemplate]

GO
ALTER TABLE [dbo].[AdditionalColumnType]  WITH NOCHECK ADD CONSTRAINT [FK_AdditionalColumnType_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[AdditionalColumnType] NOCHECK CONSTRAINT [FK_AdditionalColumnType_Organisation]

GO
ALTER TABLE [dbo].[Department]  WITH NOCHECK ADD CONSTRAINT [FK_Department_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Organisation]

GO
ALTER TABLE [dbo].[Schedule]  WITH NOCHECK ADD CONSTRAINT [FK_Schedule_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Schedule] NOCHECK CONSTRAINT [FK_Schedule_Organisation]

GO
ALTER TABLE [dbo].[Organisation]  WITH NOCHECK ADD  CONSTRAINT [FK_Organisation_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Organisation] NOCHECK CONSTRAINT [FK_Organisation_Photo]

GO
ALTER TABLE [dbo].[ScheduleScheme]  WITH NOCHECK ADD CONSTRAINT [FK_ScheduleScheme_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[ScheduleScheme] NOCHECK CONSTRAINT [FK_ScheduleScheme_Organisation]

GO
ALTER TABLE [dbo].[Position]  WITH NOCHECK ADD CONSTRAINT [FK_Position_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Position] NOCHECK CONSTRAINT [FK_Position_Organisation]

GO
ALTER TABLE [dbo].[Phone]  WITH NOCHECK ADD CONSTRAINT [FK_Phone_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Phone] NOCHECK CONSTRAINT [FK_Phone_Organisation]

GO
ALTER TABLE [dbo].[NamedInterval]  WITH NOCHECK ADD CONSTRAINT [FK_NamedInterval_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[NamedInterval] NOCHECK CONSTRAINT [FK_NamedInterval_Organisation]

GO
ALTER TABLE [dbo].[Holiday]  WITH NOCHECK ADD CONSTRAINT [FK_Holiday_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Holiday] NOCHECK CONSTRAINT [FK_Holiday_Organisation]

GO
ALTER TABLE [dbo].[EmployeeReplacement]  WITH NOCHECK ADD CONSTRAINT [FK_EmployeeReplacement_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[EmployeeReplacement] NOCHECK CONSTRAINT [FK_EmployeeReplacement_Organisation]

GO
ALTER TABLE [dbo].[Employee]  WITH NOCHECK ADD CONSTRAINT [FK_Employee_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Organisation]

GO
ALTER TABLE [dbo].[Document]  WITH NOCHECK ADD CONSTRAINT [FK_Document_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Document] NOCHECK CONSTRAINT [FK_Document_Organisation]

GO
ALTER TABLE [dbo].[AccessTemplate] WITH NOCHECK ADD CONSTRAINT [FK_AccessTemplate_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[AccessTemplate] NOCHECK CONSTRAINT [FK_AccessTemplate_Organisation]

GO
ALTER TABLE [dbo].[OrganisationZone] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationZone_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[OrganisationZone] NOCHECK CONSTRAINT [FK_OrganisationZone_Organisation]

GO
ALTER TABLE [dbo].[AdditionalColumn]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalColumn_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[AdditionalColumn] NOCHECK CONSTRAINT [FK_AdditionalColumn_Photo]

GO
ALTER TABLE [dbo].[PassJournal]  WITH NOCHECK ADD  CONSTRAINT [FK_PassJournal_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[PassJournal] CHECK CONSTRAINT [FK_PassJournal_Employee]

GO