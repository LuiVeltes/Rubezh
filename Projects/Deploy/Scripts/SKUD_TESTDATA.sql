USE [Firesec]
GO

DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO

INSERT INTO [dbo].[Person] VALUES ('������','����','������','12-21-75','12-21-05','����');
INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), '���','�����','Comment1', 0);
INSERT INTO [dbo].[Person] VALUES ('������','����','��������','03-18-86','12-21-05','������');
INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), '������','��������','Comment2', 0);
INSERT INTO [dbo].[Person] VALUES ('�������','�����','���������',NULL,'12-21-05','���');
INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), '����������','�����������','Comment3', 0);

GO