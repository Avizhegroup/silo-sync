CREATE TABLE [dbo].[tbl_InspectResult](
	[fld_InspectResultId] [bigint] IDENTITY(1,1) NOT NULL,
	[fld_InspectResultInspectId] [int] NULL,
	[fld_InspectResultInspectElementId] [int] NULL,
	[fld_InspectResultValues] [nvarchar](max) NULL
)