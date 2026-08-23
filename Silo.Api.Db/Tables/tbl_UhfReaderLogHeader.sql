CREATE TABLE [dbo].[tbl_UHFReaderLogHeader](
	[fld_UHFReaderLogHeaderId] [int] IDENTITY(1,1) NOT NULL,
	[fld_StationCode] [nvarchar](max) NULL,
	[fld_ActionType] [int] NULL,
	[fld_DocumentCode] [nvarchar](450) NULL,
	[fld_TruckCrossId] [bigint] NULL,
	[fld_UHFReaderLogHeaderUserId] [nvarchar](128) NULL,
	[fld_UHFReaderLogHeaderControlType] [int] NULL,
	[fld_CarProperties] [nvarchar](max) NULL,
	[fld_MovementActionId] [int] NULL,
    [fld_HeaderUsedStatus] int NULL,
    [fld_HeaderCreateDateTime] [datetime] NULL,
    ) ON [PRIMARY]
    GO

    ALTER TABLE [dbo].[tbl_UHFReaderLogHeader] ADD  CONSTRAINT [DF_tbl_UHFReaderLogHeader_fld_HeaderCreateDateTime]  DEFAULT (getdate()) FOR [fld_HeaderCreateDateTime]
GO
