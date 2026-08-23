CREATE TABLE [dbo].[tbl_AlarmLog](
	[fld_AlarmLogId] INT IDENTITY(1,1) NOT NULL,
	[fld_AlarmLogDateTime] DateTime NULL,
	[fld_AlarmLogGateNumber] NVARCHAR(50) NULL,
	[fld_AlarmLogType] NVARCHAR(50) NULL,
	[fld_AlarmLogTime] NVARCHAR(5) NULL,
	[fld_AlarmLogTag] NVARCHAR(24) NULL,
	[fld_AlarmLogSerial] NVARCHAR(50) NULL,
	[fld_AlarmLogActionId] INT NULL,
	[fld_AlarmLogUserId] NVARCHAR(50) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_AlarmLog] ADD  CONSTRAINT [DF_tbl_AlarmLog_fld_AlarmLogTag]  DEFAULT ((0)) FOR [fld_AlarmLogTag]
GO
