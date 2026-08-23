CREATE TABLE [dbo].[tbl_UOM](
	[fld_UOMId] [int] NULL,
	[fld_UOMTitle] [nvarchar](50) NULL,
	[fld_UOMLevel] [int] NULL,
	[IsBaseUnit] [bit] NULL,
	[IsActionalUnit] [bit] NULL,
	[IsGenerateSerial] [bit] NULL
) ON [PRIMARY]