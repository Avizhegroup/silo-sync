CREATE TABLE [dbo].[tbl_DocumentLog](
	[fld_LogId] [int] IDENTITY(1,1) NOT NULL,
	[fld_LogDocumentKey] [nvarchar](max) NULL,
	[fld_LogDocumentType] [nvarchar](10) NULL,
	[fld_LogDocumentStatus] [int] NULL,
	[fld_LogDateTime] [datetime] NULL,
	[fld_LogShamsiDate] [nvarchar](10) NULL,
	[fld_LogUserId] [nvarchar](128) NULL,
    [fld_LogEventType] [int] NULL,
    [fld_LogDescription] [nvarchar](max) NULL
)