USE [Firesec]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] where id = object_id(N'[dbo].[GetAllEmployees]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetAllEmployees]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllEmployees]
AS
BEGIN
	SELECT 
		e.Id,
		e.PersonId,
		p.LastName,
		p.FirstName,
		p.SecondName,
		DateDiff (Year, p.Birthday, getdate()) as Age,
		e.Department,
		e.Position,
		e.Comment
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p on e.PersonId = p.Id
END
GO

-- FOR TEST ONLY - DEBUG
DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO
INSERT INTO [dbo].[Person] values ('������','����','������','12-21-75','12-21-05','����');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), '���','�����','Comment1');
INSERT INTO [dbo].[Person] values ('������','����','��������','03-18-86','12-21-05','������');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), '������','��������','Comment2');
INSERT INTO [dbo].[Person] values ('�������','�����','���������',NULL,'12-21-05','���');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), '����������','�����������','Comment3');