USE [Firesec]
GO

DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
DELETE FROM [dbo].[Position];
DELETE FROM [dbo].[Group];
DELETE FROM [dbo].[Department];
GO

INSERT INTO [dbo].[Position] (Value) VALUES ('��������')
INSERT INTO [dbo].[Position] (Value) VALUES ('��������')
INSERT INTO [dbo].[Position] (Value) VALUES ('�����')
INSERT INTO [dbo].[Position] (Value) VALUES ('������������')
INSERT INTO [dbo].[Position] (Value) VALUES ('�������������')
INSERT INTO [dbo].[Position] (Value) VALUES ('�������')
GO

INSERT INTO [dbo].[Group] (Value) VALUES ('���������')
INSERT INTO [dbo].[Group] (Value) VALUES ('��������� ���')
INSERT INTO [dbo].[Group] (Value) VALUES ('������������')
INSERT INTO [dbo].[Group] (Value) VALUES ('�����������')
GO

INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('�����������', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('������ ����������', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('�������� ������������', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('��������������� ������������', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('������ ������������', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('������������ ���', NULL) 
GO

DECLARE @DepId int

SELECT @DepId = MIN(Id) FROM [dbo].[Department]
INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('������','����','������',NULL,NULL,NULL,NULL,'01-01-01',NULL,'777',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'1',NULL,@DepId,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('������','����','��������',NULL,NULL,NULL,NULL,'03-12-18',NULL,'666',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'2',NULL,@DepId,NULL,NULL,NULL,NULL,0);

SELECT @DepId = MAX(Id) FROM [dbo].[Department]
INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('�������','����','��������',NULL,NULL,NULL,NULL,'12-12-05',NULL,'999',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'3',NULL,@DepId,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('�������','�����','���������',NULL,NULL,NULL,NULL,'12-12-05',NULL,'999',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'3',NULL,NULL,NULL,NULL,NULL,NULL,0);

--INSERT INTO [dbo].[Person] VALUES ('������','����','������','12-21-75','12-21-05','����');
--INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), '���','�����','Comment1', 0);
--INSERT INTO [dbo].[Person] VALUES ('������','����','��������','03-18-86','12-21-05','������');
--INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), '������','��������','Comment2', 0);
--INSERT INTO [dbo].[Person] VALUES ('�������','�����','���������',NULL,'12-21-05','���');
--INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), '����������','�����������','Comment3', 0);

GO