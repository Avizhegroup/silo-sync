
CREATE TABLE [dbo].[tbl_TablesChangeLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TableName] [sysname] NOT NULL,
	[RecordKey] [nvarchar](100) NULL,
	[ChangeDescription] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL DEFAULT GetDate(),
    [UserId] NVARCHAR(128) NULL) ON [PRIMARY] 
GO