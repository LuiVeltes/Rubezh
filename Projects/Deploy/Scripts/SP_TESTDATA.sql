USE [SKUD]
DECLARE @Uid uniqueidentifier;

TRUNCATE TABLE [dbo].[Holiday]
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, '����� ���', 'Transferred', '2013-31-12', '2013-28-12'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, '8 �����', 'Holiday', '2014-08-03'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, '������ ����� ���', 'Redused', '2014-13-01', NULL, 2

TRUNCATE TABLE [dbo].[Document]
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, '��������1', '��������1', '2013-01-01', '2013-07-01'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, '��������2', '��������2', '2014-08-01', '2013-25-01'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, '��������3', '��������3', '2014-30-01', '2013-05-02'

delete from [dbo].[Interval]
delete from [dbo].[NamedInterval]
delete from [dbo].[Day]
delete from [dbo].[ScheduleScheme]
--������
DECLARE @GuardNamedIntervalUid uniqueidentifier;
SET @GuardNamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @GuardNamedIntervalUid, '������'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '13:00', 'Day', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '13:45', '17:45', 'Day', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '18:30', '22:30', 'Day', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '23:15', '03:15', 'DayNight', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '04:00', '08:00', 'Night', @GuardNamedIntervalUid
DECLARE @GuardScheduleSchemeUid uniqueidentifier;
SET @GuardScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @GuardScheduleSchemeUid, '������', 'Shift', 4
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @GuardNamedIntervalUid, @GuardScheduleSchemeUid, 0
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @GuardScheduleSchemeUid, 1
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @GuardScheduleSchemeUid, 2
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @GuardScheduleSchemeUid, 3
--������
DECLARE @Montage1NamedIntervalUid uniqueidentifier;
SET @Montage1NamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @Montage1NamedIntervalUid, '���������1'
DECLARE @Montage2NamedIntervalUid uniqueidentifier;
SET @Montage2NamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @Montage2NamedIntervalUid, '���������2'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '07:30', '11:00', 'Day', @Montage1NamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '11:30', '16:00', 'Day', @Montage1NamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '11:30', 'Day', @Montage2NamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '12:00', '16:30', 'Day', @Montage2NamedIntervalUid
DECLARE @MontageScheduleSchemeUid uniqueidentifier;
SET @MontageScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @MontageScheduleSchemeUid, '������', 'Week', 7
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 0
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage2NamedIntervalUid, @MontageScheduleSchemeUid, 1
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 2
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage2NamedIntervalUid, @MontageScheduleSchemeUid, 3
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 4
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @MontageScheduleSchemeUid, 5
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @MontageScheduleSchemeUid, 6
--����������
DECLARE @WeeklyNamedIntervalUid uniqueidentifier;
SET @WeeklyNamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @WeeklyNamedIntervalUid, '����������'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '12:00', 'Day', @WeeklyNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '13:00', '17:00', 'Day', @WeeklyNamedIntervalUid
DECLARE @WeeklyScheduleSchemeUid uniqueidentifier;
SET @WeeklyScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @WeeklyScheduleSchemeUid, '����������', 'Week', 7
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 0
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 1
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 2
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 3
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 4
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @WeeklyScheduleSchemeUid, 5
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @WeeklyScheduleSchemeUid, 6

delete from [dbo].[Schedule]
DECLARE @GuardScheduleUid uniqueidentifier;
SET @GuardScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @GuardScheduleUid, '������', @GuardScheduleSchemeUid
DECLARE @MontageScheduleUid uniqueidentifier;
SET @MontageScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @MontageScheduleUid , '������', @MontageScheduleSchemeUid 
DECLARE @ITScheduleUid uniqueidentifier;
SET @ITScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @ITScheduleUid , 'IT', @WeeklyScheduleSchemeUid 
DECLARE @ConstructorshipScheduleUid uniqueidentifier;
SET @ConstructorshipScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @ConstructorshipScheduleUid , '������������', @WeeklyScheduleSchemeUid  
DECLARE @GovernanceScheduleUid uniqueidentifier;
SET @GovernanceScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @GovernanceScheduleUid , '�����������', @WeeklyScheduleSchemeUid

delete from [dbo].[RegisterDevice]
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @GuardScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @MontageScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @ITScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @ConstructorshipScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, null, @GovernanceScheduleUid

delete from [dbo].[Position]
DECLARE @GuardPositionUid uniqueidentifier;
SET @GuardPositionUid = NEWID();
EXEC [dbo].[SavePosition] @GuardPositionUid, '��������', '�������������������'
DECLARE @MainGuardPositionUid uniqueidentifier;
SET @MainGuardPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainGuardPositionUid , '��. ��������', '������� �������������������'
DECLARE @MontagePositionUid uniqueidentifier;
SET @MontagePositionUid = NEWID();
EXEC [dbo].[SavePosition] @MontagePositionUid , '���������', '�������� �������'
DECLARE @MainMontagePositionUid uniqueidentifier;
SET @MainMontagePositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainMontagePositionUid , '��. ���������', '������� �������� �������'
DECLARE @ProgrammerPositionUid uniqueidentifier;
SET @ProgrammerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ProgrammerPositionUid , '�����������', '����������� ���������� ��� ���'
DECLARE @MainProgrammerPositionUid uniqueidentifier;
SET @MainProgrammerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainProgrammerPositionUid , '��. �����������', '������� ����������� ���������� ��� ���'
DECLARE @TesterPositionUid uniqueidentifier;
SET @TesterPositionUid = NEWID();
EXEC [dbo].[SavePosition] @TesterPositionUid , '�����������', '���������� ���������� ��� ���'
DECLARE @MainTesterPositionUid uniqueidentifier;
SET @MainTesterPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainTesterPositionUid , '��. �����������', '������� ���������� ���������� ��� ���'
DECLARE @ConstructorPositionUid uniqueidentifier;
SET @ConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ConstructorPositionUid , '�����������', '�������-�����������'
DECLARE @MainConstructorPositionUid uniqueidentifier;
SET @MainConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainConstructorPositionUid , '��. �����������', '������� �������-�����������'
DECLARE @ProgrammistConstructorPositionUid uniqueidentifier;
SET @ProgrammistConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ProgrammistConstructorPositionUid , '�����������-�����������', '�������������� � ��������� ��������'
DECLARE @AdministratorPositionUid uniqueidentifier;
SET @AdministratorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @AdministratorPositionUid , '�������������', '������� �������������'
DECLARE @DirectorPositionUid uniqueidentifier;
SET @DirectorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @DirectorPositionUid , '��������', '������������ ��������'

delete from [dbo].[Employee]
DECLARE @Guard1EmployeeUid uniqueidentifier;
SET @Guard1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Guard1EmployeeUid , '������', '��������', '������', @GuardPositionUid, null , @GuardScheduleUid, '12-05-2005'
DECLARE @Guard2EmployeeUid uniqueidentifier;
SET @Guard2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Guard2EmployeeUid , '����', '���������', '����������', @GuardPositionUid, null , @GuardScheduleUid, '12-05-2006'
DECLARE @MainGuardEmployeeUid uniqueidentifier;
SET @MainGuardEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainGuardEmployeeUid , '����', '���������', '����������', @MainGuardPositionUid, null , @GuardScheduleUid, '12-05-2007'
DECLARE @Montage1EmployeeUid uniqueidentifier;
SET @Montage1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Montage1EmployeeUid , '����', '���������', '������', @MontagePositionUid, null , @MontageScheduleUid, '12-05-2008'
DECLARE @Montage2EmployeeUid uniqueidentifier;
SET @Montage2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Montage2EmployeeUid , '������', '��������', '������������', @MontagePositionUid, null , @MontageScheduleUid, '12-05-2009'
DECLARE @MainMontageEmployeeUid uniqueidentifier;
SET @MainMontageEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainMontageEmployeeUid , '������', '���������', '����������', @MainMontagePositionUid, null , @MontageScheduleUid, '12-05-2010'
DECLARE @Programmer1EmployeeUid uniqueidentifier;
SET @Programmer1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Programmer1EmployeeUid , '����', '��������', '�������', @ProgrammerPositionUid, null , @ITScheduleUid, '12-05-2011'
DECLARE @Programmer2EmployeeUid uniqueidentifier;
SET @Programmer2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Programmer2EmployeeUid , '����', '�������', '���������', @ProgrammerPositionUid, null , @ITScheduleUid, '12-05-2012'
DECLARE @MainProgrammistEmployeeUid uniqueidentifier;
SET @MainProgrammistEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainProgrammistEmployeeUid , '����', '��������', '���������', @MainProgrammerPositionUid, null , @ITScheduleUid, '12-05-2013'
DECLARE @Tester1EmployeeUid uniqueidentifier;
SET @Tester1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Tester1EmployeeUid , '�����', '����������', '��������', @TesterPositionUid, null , @ITScheduleUid, '12-06-2013'
DECLARE @Tester2EmployeeUid uniqueidentifier;
SET @Tester2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Tester1EmployeeUid , '������', '���������', '�������', @TesterPositionUid, null , @ITScheduleUid, '12-07-2013'
DECLARE @MainTesterEmployeeUid uniqueidentifier;
SET @MainTesterEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainTesterEmployeeUid , '������', '����������', '���������', @MainTesterPositionUid, null , @ITScheduleUid, '12-08-2013'
DECLARE @Constructor1EmployeeUid uniqueidentifier;
SET @Constructor1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Constructor1EmployeeUid , '�����', '���������', '��������', @ConstructorPositionUid, null , @ConstructorshipScheduleUid, '12-09-2013'
DECLARE @Constructor2EmployeeUid uniqueidentifier;
SET @Constructor2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Constructor2EmployeeUid , '�����', '���������', '����������', @ConstructorPositionUid, null , @ConstructorshipScheduleUid, '12-10-2013'
DECLARE @MainConstructorEmployeeUid uniqueidentifier;
SET @MainConstructorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainConstructorEmployeeUid , '�����', '���������', '����������', @MainConstructorPositionUid, null , @ConstructorshipScheduleUid, '12-11-2013'
DECLARE @AdministratorEmployeeUid uniqueidentifier;
SET @AdministratorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @AdministratorEmployeeUid , '�����', '����������', '�������', @AdministratorPositionUid, null , @GovernanceScheduleUid, '12-12-2013'
DECLARE @DirectorEmployeeUid uniqueidentifier;
SET @DirectorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @DirectorEmployeeUid , '������', '���������', '���������', @DirectorEmployeeUid , null , @GovernanceScheduleUid, '13-12-2013'
DECLARE @ProgrammistConstructorEmployeeUid uniqueidentifier;
SET @ProgrammistConstructorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @ProgrammistConstructorEmployeeUid , '�������', '�������', '������', @ProgrammistConstructorPositionUid , null , @ConstructorshipScheduleUid, '13-12-2001'

delete from [dbo].[Department]
DECLARE @MainDepartmentUid uniqueidentifier;
SET @MainDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @MainDepartmentUid , '��� "�����"', '����������� ����� 3 ������� ���������', null , @DirectorEmployeeUid, @AdministratorEmployeeUid
DECLARE @GovernanceDepartmentUid uniqueidentifier;
SET @GovernanceDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @GovernanceDepartmentUid , '�����������', '��������������� � ������ �����������', @MainDepartmentUid , @AdministratorEmployeeUid, @DirectorEmployeeUid 
DECLARE @GuardDepartmentUid uniqueidentifier;
SET @GuardDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @GuardDepartmentUid , '������', '���������� � ������������', @MainDepartmentUid , @MainGuardEmployeeUid, @Guard1EmployeeUid 
DECLARE @MontageDepartmentUid uniqueidentifier;
SET @MontageDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @MontageDepartmentUid , '������', '���������� � �����������', @MainDepartmentUid , @MainMontageEmployeeUid, @Montage1EmployeeUid 
DECLARE @EngeneeringDepartmentUid uniqueidentifier;
SET @EngeneeringDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @EngeneeringDepartmentUid , '��������', '��������� � ���� �����������', @MainDepartmentUid , @ProgrammistConstructorEmployeeUid 
DECLARE @ITDepartmentUid uniqueidentifier;
SET @ITDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ITDepartmentUid , 'IT', '����������������� � ������������', @EngeneeringDepartmentUid , @MainProgrammistEmployeeUid, @MainTesterEmployeeUid 
DECLARE @ProgrammerDepartmentUid uniqueidentifier;
SET @ProgrammerDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ProgrammerDepartmentUid , '������������', '�����������������', @ITDepartmentUid , @MainProgrammistEmployeeUid, @Programmer1EmployeeUid 
DECLARE @TesterDepartmentUid uniqueidentifier;
SET @TesterDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @TesterDepartmentUid , '������������', '���� ���������� �����������', @ITDepartmentUid , @MainTesterEmployeeUid , @Tester1EmployeeUid 
DECLARE @ConstructorshipDepartmentUid uniqueidentifier;
SET @ConstructorshipDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ConstructorshipDepartmentUid , '������������', '����� ��������� ����������', @EngeneeringDepartmentUid , @MainConstructorEmployeeUid , @Constructor1EmployeeUid

UPDATE [dbo].[Employee] SET [DepartmentUid]=@GovernanceDepartmentUid WHERE [Uid]=@DirectorEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GovernanceDepartmentUid WHERE [Uid]=@AdministratorEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GuardDepartmentUid WHERE [Uid]=@Guard1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GuardDepartmentUid WHERE [Uid]=@Guard2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GuardDepartmentUid WHERE [Uid]=@MainGuardEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@MontageDepartmentUid WHERE [Uid]=@Montage1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@MontageDepartmentUid WHERE [Uid]=@Montage2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@MontageDepartmentUid WHERE [Uid]=@MainMontageEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@EngeneeringDepartmentUid WHERE [Uid]=@ProgrammistConstructorEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ProgrammerDepartmentUid WHERE [Uid]=@Programmer1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ProgrammerDepartmentUid WHERE [Uid]=@Programmer2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ProgrammerDepartmentUid WHERE [Uid]=@MainProgrammistEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@TesterDepartmentUid WHERE [Uid]=@Tester1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@TesterDepartmentUid WHERE [Uid]=@Tester2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@TesterDepartmentUid WHERE [Uid]=@MainTesterEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ConstructorshipDepartmentUid WHERE [Uid]=@Constructor1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ConstructorshipDepartmentUid WHERE [Uid]=@Constructor2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ConstructorshipDepartmentUid WHERE [Uid]=@MainConstructorEmployeeUid