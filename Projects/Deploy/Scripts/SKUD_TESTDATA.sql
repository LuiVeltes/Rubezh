USE [Firesec]
GO

DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('������','����','������',NULL,NULL,NULL,NULL,'12-21-75',NULL,'777',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'1',NULL,NULL,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('������','����','��������',NULL,NULL,NULL,NULL,'03-18-86',NULL,'666',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'2',NULL,NULL,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('�������','�����','���������',NULL,NULL,NULL,NULL,'12-21-05',NULL,'999',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
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