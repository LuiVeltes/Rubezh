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
	SELECT * 
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p on e.PersonId = p.Id
END


-- FOR TEST ONLY - DEBUG
DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO
INSERT INTO [dbo].[Person] values ('������','����','������','12-21-75','12-21-05','����');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), '���','�����','');
INSERT INTO [dbo].[Person] values ('������','����','��������','03-18-86','12-21-05','������');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), '������','��������','');
INSERT INTO [dbo].[Person] values ('�������','�����','���������','09-07-97','12-21-05','���');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), '����������','�����������','');