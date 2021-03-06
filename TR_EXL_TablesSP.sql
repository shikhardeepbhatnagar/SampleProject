
/****** Object:  StoredProcedure [dbo].[UpdateTemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[UpdateTemplatesWorkbookInfo_EXL]
GO
/****** Object:  StoredProcedure [dbo].[InsertTemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[InsertTemplatesWorkbookInfo_EXL]
GO
/****** Object:  StoredProcedure [dbo].[GetAllWorksheetInfoTR_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[GetAllWorksheetInfoTR_EXL]
GO
/****** Object:  StoredProcedure [dbo].[GetAllParentCategoriesTR_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[GetAllParentCategoriesTR_EXL]
GO
/****** Object:  StoredProcedure [dbo].[GetAllChildCategoriesTR_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[GetAllChildCategoriesTR_EXL]
GO
/****** Object:  StoredProcedure [dbo].[DMLCategoryDetailsTR_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[DMLCategoryDetailsTR_EXL]
GO
/****** Object:  StoredProcedure [dbo].[DeleteTemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP PROCEDURE [dbo].[DeleteTemplatesWorkbookInfo_EXL]
GO
ALTER TABLE [dbo].[TemplateWorksheetInfo_EXL] DROP CONSTRAINT [FK_TemplateWorksheetInfo_EXL_TemplatesWorkbookInfo_EXL]
GO
ALTER TABLE [dbo].[TemplatesWorkbookInfo_EXL] DROP CONSTRAINT [FK_TemplatesWorkbookInfo_EXL_TemplateCategoryMaster_EXL]
GO
ALTER TABLE [dbo].[TemplateCategoryMaster_EXL] DROP CONSTRAINT [FK_TemplateCategoryMaster_Exl_TemplateCategoryMaster_Exl]
GO
/****** Object:  Table [dbo].[TemplateWorksheetInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP TABLE [dbo].[TemplateWorksheetInfo_EXL]
GO
/****** Object:  Table [dbo].[TemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP TABLE [dbo].[TemplatesWorkbookInfo_EXL]
GO
/****** Object:  Table [dbo].[TemplateCategoryMaster_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP TABLE [dbo].[TemplateCategoryMaster_EXL]
GO
/****** Object:  UserDefinedTableType [dbo].[udt_TR_TemplateWorksheetInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
DROP TYPE [dbo].[udt_TR_TemplateWorksheetInfo_EXL]
GO
/****** Object:  UserDefinedTableType [dbo].[udt_TR_TemplateWorksheetInfo_EXL]    Script Date: 12/1/2021 11:05:35 AM ******/
CREATE TYPE [dbo].[udt_TR_TemplateWorksheetInfo_EXL] AS TABLE(
	[Id] [int] NOT NULL,
	[TemplateWorkbookId] [int] NOT NULL,
	[WorksheetName] [varchar](255) NOT NULL,
	[SystemWorksheetName] [varchar](255) NULL,
	[InsertedAt] [datetime] NOT NULL,
	[InsertedBy] [varchar](100) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[TemplateCategoryMaster_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TemplateCategoryMaster_EXL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [varchar](255) NOT NULL,
	[CategoryIconName] [varchar](100) NULL,
	[CategoryParentId] [int] NULL,
	[Status] [bit] NOT NULL,
	[InsertedAt] [datetime] NULL,
	[InsertedBy] [varchar](100) NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[UpdatedBy] [varchar](100) NOT NULL,
 CONSTRAINT [PK_TemplateCategoryMaster_Exl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TemplatesWorkbookInfo_EXL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [varchar](255) NOT NULL,
	[OrigFileName] [varchar](255) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[SystemFileName] [varchar](100) NOT NULL,
	[Description] [varchar](max) NULL,
	[FileSizeInKB] [int] NOT NULL,
	[WorksheetCount] [int] NOT NULL,
	[IsPreviewAvailable] [bit] NOT NULL,
	[InsertedAt] [datetime] NULL,
	[InsertedBy] [varchar](100) NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[UpdatedBy] [varchar](100) NOT NULL,
 CONSTRAINT [PK_TemplatesWorkbookInfo_EXL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TemplateWorksheetInfo_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[TemplateWorksheetInfo_EXL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateWorkbookId] [int] NOT NULL,
	[WorksheetName] [varchar](255) NOT NULL,
	[SystemWorksheetName] [varchar](100) NULL,
	[InsertedAt] [datetime] NULL,
	[InsertedBy] [varchar](100) NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[UpdatedBy] [varchar](100) NOT NULL,
 CONSTRAINT [TemplateWorksheetsInfo_EXL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[TemplateCategoryMaster_EXL]  WITH CHECK ADD  CONSTRAINT [FK_TemplateCategoryMaster_Exl_TemplateCategoryMaster_Exl] FOREIGN KEY([CategoryParentId])
REFERENCES [dbo].[TemplateCategoryMaster_EXL] ([Id])
GO
ALTER TABLE [dbo].[TemplateCategoryMaster_EXL] CHECK CONSTRAINT [FK_TemplateCategoryMaster_Exl_TemplateCategoryMaster_Exl]
GO
ALTER TABLE [dbo].[TemplatesWorkbookInfo_EXL]  WITH CHECK ADD  CONSTRAINT [FK_TemplatesWorkbookInfo_EXL_TemplateCategoryMaster_EXL] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[TemplateCategoryMaster_EXL] ([Id])
GO
ALTER TABLE [dbo].[TemplatesWorkbookInfo_EXL] CHECK CONSTRAINT [FK_TemplatesWorkbookInfo_EXL_TemplateCategoryMaster_EXL]
GO
ALTER TABLE [dbo].[TemplateWorksheetInfo_EXL]  WITH CHECK ADD  CONSTRAINT [FK_TemplateWorksheetInfo_EXL_TemplatesWorkbookInfo_EXL] FOREIGN KEY([TemplateWorkbookId])
REFERENCES [dbo].[TemplatesWorkbookInfo_EXL] ([Id])
GO
ALTER TABLE [dbo].[TemplateWorksheetInfo_EXL] CHECK CONSTRAINT [FK_TemplateWorksheetInfo_EXL_TemplatesWorkbookInfo_EXL]
GO
/****** Object:  StoredProcedure [dbo].[DeleteTemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <03/04/2021>
-- Description:	<To perform Delete operations on Template TR EXL >
-- =============================================
--EXEC [DeleteTemplatesWorkbookInfo_EXL]
CREATE PROCEDURE [dbo].[DeleteTemplatesWorkbookInfo_EXL]
@templateWorkbookId int

AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;   

	IF EXISTS(SELECT * FROM [dbo].[TemplatesWorkbookInfo_EXL] WHERE Id = @templateWorkbookId)
	BEGIN
		DELETE FROM [dbo].[TemplateWorksheetInfo_EXL] WHERE TemplateWorkbookId=@templateWorkbookId		
		DELETE FROM [dbo].[TemplatesWorkbookInfo_EXL] WHERE Id = @templateWorkbookId
	END
END

GO
/****** Object:  StoredProcedure [dbo].[DMLCategoryDetailsTR_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <03/04/2021>
-- Description:	<To perform DML operations on category TR EXL >
-- =============================================
--EXEC [SaveCatagoryDetails]
CREATE PROCEDURE [dbo].[DMLCategoryDetailsTR_EXL]
@categoryId int = null,
@categoryName varchar(255) = null,
@categoryIconName varchar(255) = null,
@categoryParentId int = null,
@status bit = null,
@insertedAt datetime = null,
@insertedBy varchar(100) = null,
@updatedAt datetime = null,
@updatedBy varchar(100) = null,
@lastUpdatedAt datetime = null,
@statementType varchar(100)

AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON; 
	DECLARE @ReturnCode INT
	IF @statementType = 'Insert'
	BEGIN
		IF(@CategoryParentId IS NULL)
		BEGIN  
			IF NOT EXISTS(SELECT Id FROM [dbo].[TemplateCategoryMaster_EXL] WHERE UPPER(CategoryName) = UPPER(@categoryName) AND CategoryParentId IS NULL)
			BEGIN
			INSERT INTO [dbo].[TemplateCategoryMaster_EXL] (CategoryName, CategoryIconName, CategoryParentId,[Status],InsertedAt,InsertedBy,UpdatedAt,UpdatedBy) 
			VALUES(@categoryName, @categoryIconName, @CategoryParentId, @status, @insertedAt, @insertedBy, @insertedAt, @insertedBy)
			SELECT @@IDENTITY
			END
		END
		ELSE
		BEGIN  
			IF NOT EXISTS(SELECT Id FROM [dbo].[TemplateCategoryMaster_EXL] WHERE UPPER(CategoryName) = UPPER(@categoryName) AND CategoryParentId = @categoryParentId)
			BEGIN
		  		INSERT INTO [dbo].[TemplateCategoryMaster_EXL](CategoryName,CategoryIconName,CategoryParentId,[Status],InsertedAt,InsertedBy,UpdatedAt,UpdatedBy)
				VALUES(@CategoryName, @categoryIconName, @CategoryParentId, @status, @insertedAt, @insertedBy, @insertedAt, @insertedBy) 
				SELECT @@IDENTITY 
			END 
		END		
	END		
	ELSE IF @statementType = 'Update'
	BEGIN  
		IF EXISTS(SELECT Id FROM [dbo].[TemplateCategoryMaster_EXL] WHERE Id= @categoryId)
		BEGIN
	  IF NOT EXISTS(SELECT Id FROM  [dbo].[TemplateCategoryMaster_EXL] WHERE UPPER(CategoryName) = UPPER(@categoryName) AND CategoryParentId = @categoryParentId)	 
				BEGIN
					UPDATE [dbo].[TemplateCategoryMaster_EXL] 
					SET
					CategoryName = @categoryName,
					CategoryIconName = @categoryIconName,
					CategoryParentID = @categoryParentID,
					[Status] = @status,
					UpdatedAt = @updatedAt,
					UpdatedBy = @updatedBy
					WHERE  Id = @categoryId
				END 
			ELSE IF((SELECT Id from  [dbo].[TemplateCategoryMaster_EXL] WHERE UPPER(CategoryName) = UPPER(@categoryName)) = @categoryId) 
				BEGIN
					UPDATE [dbo].[TemplateCategoryMaster_EXL] 
					SET
					CategoryName = @categoryName,
					CategoryIconName = @categoryIconName,
					CategoryParentID = @categoryParentID,
					[Status] = @status,
					UpdatedAt = @updatedAt,
					UpdatedBy = @updatedBy
					WHERE Id = @categoryId
				END
			ELSE
			  BEGIN
			   SET @ReturnCode = 4
			   Select @ReturnCode as ReturnCode
			  END
		END 
	END	
	ELSE IF @statementType = 'Delete'  
		BEGIN  
			IF EXISTS(SELECT Id from  [dbo].[TemplateCategoryMaster_EXL] WHERE Id=@CategoryId AND CategoryName=@categoryName)
				BEGIN
				
				DECLARE @DELETED TABLE(Id INT)
				INSERT INTO @DELETED values (@categoryId);

					WITH cte AS 
						(
						SELECT a.Id
						FROM  [dbo].[TemplateCategoryMaster_EXL] a
						WHERE CategoryParentId = @categoryId
						UNION ALL
						SELECT a.Id FROM [dbo].[TemplateCategoryMaster_EXL] a JOIN cte c ON a.CategoryParentId = c.Id
						AND c.Id != @categoryId
						)
				INSERT INTO @DELETED SELECT u.Id FROM cte As u;

				DELETE TWS FROM [dbo].[TemplateWorksheetInfo_EXL] TWS
				INNER JOIN [dbo].[TemplatesWorkbookInfo_EXL] TWB ON TWS.TemplateWorkbookId = TWB.Id
				WHERE CategoryId IN (SELECT Id FROM @DELETED)	
				 

				--DELETE FROM [dbo].[TemplateWorksheetInfo_EXL] WHERE TemplateWorkbookId IN (SELECT Id FROM [dbo].[TemplatesWorkbookInfo_EXL] 
				--WHERE CategoryId IN (SELECT Id FROM @DELETED))	
				DELETE FROM [dbo].[TemplatesWorkbookInfo_EXL] WHERE CategoryId IN (SELECT Id FROM @DELETED)									
				DELETE FROM [dbo].[TemplateCategoryMaster_EXL] WHERE Id IN (SELECT Id FROM @DELETED WHERE Id != @CategoryId)
				DELETE FROM [dbo].[TemplateCategoryMaster_EXL] WHERE Id = @CategoryId
				END
			ELSE
				BEGIN
				SET @ReturnCode = 3
				Select @ReturnCode as ReturnCode
				END
		END	
END


GO
/****** Object:  StoredProcedure [dbo].[GetAllChildCategoriesTR_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <11/24/2021>
-- Description:	<To get all the child categories and template data EXL>
-- =============================================
--EXEC [GetAllChildCategoriesTR]
CREATE PROCEDURE [dbo].[GetAllChildCategoriesTR_EXL]
@categoryID INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	WITH cte AS 
 (
  SELECT a.Id,a.CategoryName,a.CategoryParentId,a.UpdatedAt
  FROM TemplateCategoryMaster_EXL a
  WHERE CategoryParentID = @CategoryID
  UNION ALL
  SELECT a.Id, a.CategoryName,a.CategoryParentId,a.UpdatedAt FROM TemplateCategoryMaster_EXL a JOIN cte c ON a.CategoryParentID = c.Id
  and c.Id != @CategoryID
  ) 
  SELECT cte.Id AS CategoryID,cte.CategoryName,cte.CategoryParentId,cte.UpdatedAt AS UpdatedAtCategory,TWI.[Id] AS TemplateWorkbookId,TWI.TemplateName,TWI.OrigFileName,
  TWI.SystemFileName,TWI.[Description],TWI.[FileSizeInKB],TWI.[WorksheetCount],TWI.[IsPreviewAvailable],TWI.UpdatedAt AS UpdatedAtTemplate 
  FROM cte Left JOIN [dbo].[TemplatesWorkbookInfo_EXL] TWI ON TWI.CategoryID = cte.Id
  ORDER BY cte.Id
END






GO
/****** Object:  StoredProcedure [dbo].[GetAllParentCategoriesTR_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <11/24/2021>
-- Description:	<To get the Excel template data>
-- =============================================
--EXEC [GetAllParentCategoriesTR_EXL]
CREATE PROCEDURE [dbo].[GetAllParentCategoriesTR_EXL]
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;  
	 
	SELECT 
	 [Id]
    ,[CategoryName]
    ,[CategoryIconName]
    ,[CategoryParentId]
    ,[Status]
	FROM [dbo].[TemplateCategoryMaster_EXL] With (NOLOCK)
	WHERE [CategoryParentId] IS NULL
	ORDER BY [Id]
	
END




GO
/****** Object:  StoredProcedure [dbo].[GetAllWorksheetInfoTR_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <11/24/2021>
-- Description:	<To get all the worksheet template data EXL>
-- =============================================
--EXEC [GetAllWorksheetInfoTR_EXL]
CREATE PROCEDURE [dbo].[GetAllWorksheetInfoTR_EXL]
@templateWorkbookId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;   
	SELECT [Id]
		  ,[TemplateWorkbookId]
		  ,[WorksheetName]
		  ,[SystemWorksheetName]
		  ,[UpdatedAt] 
		   FROM [dbo].[TemplateWorksheetInfo_EXL]
		   WHERE TemplateWorkbookId = @templateWorkbookId
		   ORDER BY Id

END


GO
/****** Object:  StoredProcedure [dbo].[InsertTemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <03/04/2021>
-- Description:	<To perform insert operations on Template TR EXL >
-- =============================================
--EXEC [InsertTemplatesWorkbookInfo_EXL]
CREATE PROCEDURE [dbo].[InsertTemplatesWorkbookInfo_EXL]
@categoryId int,
@templateName varchar(255),
@origFileName varchar(255),
@systemFileName varchar(255),
@description varchar(MAX),
@fileSizeInKB int,
@worksheetCount int,
@isPreviewAvailable bit,
@insertedAt datetime,
@insertedBy varchar(100),
@WorksheetInfo udt_TR_TemplateWorksheetInfo_EXL readonly 
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;   

	IF EXISTS(SELECT * FROM [dbo].[TemplateCategoryMaster_EXL] where Id=@categoryId)
	BEGIN
		IF NOT EXISTS(SELECT * FROM [dbo].[TemplatesWorkbookInfo_EXL] where TemplateName=@templateName AND CategoryId=@categoryId) 
		BEGIN
			INSERT INTO [dbo].[TemplatesWorkbookInfo_EXL] 
			(TemplateName,OrigFileName,CategoryId,SystemFileName,[Description],FileSizeInKB,WorksheetCount,IsPreviewAvailable,InsertedAt,InsertedBy,UpdatedAt,UpdatedBy) 
			 VALUES 
			(@templateName,@origFileName, @categoryId, @systemFileName, @description,@fileSizeInKB,@worksheetCount,@isPreviewAvailable, @insertedAt, @insertedBy, @insertedAt, @insertedBy)

			INSERT INTO [dbo].[TemplateWorksheetInfo_EXL]
			(TemplateWorkbookId,WorksheetName,SystemWorksheetName,InsertedBy,InsertedAt,UpdatedBy,UpdatedAt)
			SELECT
			@@IDENTITY,
			WorksheetName,
			SystemWorksheetName,			
			InsertedBy,
			InsertedAt,
			UpdatedBy,
			UpdatedAt
			FROM @WorksheetInfo
		END
	END
END

GO
/****** Object:  StoredProcedure [dbo].[UpdateTemplatesWorkbookInfo_EXL]    Script Date: 12/1/2021 11:05:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Nitesh Sachan>
-- Create date: <03/04/2021>
-- Description:	<To perform update operations on Template TR EXL >
-- =============================================
--EXEC [InsertTemplatesWorkbookInfo_EXL]
CREATE PROCEDURE [dbo].[UpdateTemplatesWorkbookInfo_EXL]
@categoryId int,
@templateWorkbookId int,
@templateName varchar(255),
@origFileName varchar(255),
@systemFileName varchar(255),
@description varchar(MAX),
@fileSizeInKB int,
@worksheetCount int,
@isPreviewAvailable bit,
@updatedAt datetime,
@updatedBy varchar(100),
@isWorksheetInfoNeedToUpdate BIT = 0,
@WorksheetInfo udt_TR_TemplateWorksheetInfo_EXL readonly 
AS
DECLARE @ReturnCode INT
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;   


		IF EXISTS(SELECT * FROM [dbo].[TemplatesWorkbookInfo_EXL] where Id=@templateWorkbookId AND CategoryId=@categoryId) 
		BEGIN
			IF NOT EXISTS(SELECT * FROM [dbo].[TemplatesWorkbookInfo_EXL] where TemplateName=@templateName AND CategoryId=@categoryId AND Id != @templateWorkbookId) 
			BEGIN
				UPDATE [dbo].[TemplatesWorkbookInfo_EXL]
					SET
					TemplateName=@templateName,
					OrigFileName=@origFileName,
					CategoryId=@categoryId,
					SystemFileName = @systemFileName,
					[Description] = @description,
					FileSizeInKB = @fileSizeInKB,
					WorksheetCount = @worksheetCount,
					IsPreviewAvailable = @isPreviewAvailable,
					UpdatedAt=@UpdatedAt,
					UpdatedBy=@UpdatedBy
					WHERE Id=@templateWorkbookId			

					IF(@isWorksheetInfoNeedToUpdate = 1)
					BEGIN
						DELETE FROM [dbo].[TemplateWorksheetInfo_EXL] WHERE TemplateWorkbookId=@templateWorkbookId			
						INSERT INTO [dbo].[TemplateWorksheetInfo_EXL]
						(TemplateWorkbookId,WorksheetName,SystemWorksheetName,InsertedBy,InsertedAt,UpdatedBy,UpdatedAt)
						SELECT
						@templateWorkbookId,
						WorksheetName,
						SystemWorksheetName,			
						InsertedBy,
						InsertedAt,
						UpdatedBy,
						UpdatedAt
						FROM @WorksheetInfo
					END
				END					
		END		
		ELSE
			BEGIN
				SET @ReturnCode = 4
				Select @ReturnCode as ReturnCode
			END
		
END



GO
