CREATE TABLE [dbo].[tbl_ReportFormat](
	[fld_ReportFormatId] [int] IDENTITY(1,1) NOT NULL,
	[fld_ReportFormatType] [int] NULL,
	[fld_ReportFormatUserId] [nvarchar](128) NULL,
	[fld_ReportFormatPath] [nvarchar](256) NULL,
	[fld_ReportFormatName] [nvarchar](256) NULL,
	[fld_ReportFormatDetails] [nvarchar](max) NULL
)