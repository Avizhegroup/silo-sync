CREATE TABLE [dbo].[tbl_DocumentStatus](
	[fld_DocumentStatusId] [int] NOT NULL,
	[fld_DocumentStatusTitle] [nvarchar](256) NULL,
	[fld_DocumentStatusIsUpdatePermitted] [bit] NULL,
	[fld_DocumentStatusIsCartablePermitted] [bit] NULL
)