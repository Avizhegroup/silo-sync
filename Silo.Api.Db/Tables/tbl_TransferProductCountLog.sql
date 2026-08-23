CREATE TABLE [dbo].[tbl_TransferProductCountLog](
	[fld_Id] [int] IDENTITY(1,1) NOT NULL,
	[fld_OldSerial] [nvarchar](256) NULL,
	[fld_NewSerial] [nvarchar](256) NULL,
	[fld_Count] [decimal](18, 2) NULL,
	[fld_ProductCode] [nvarchar](256) NULL,
	[fld_Date] [datetime] NULL,
	[fld_UserId] [nvarchar](128) NULL
) ON [PRIMARY]