CREATE TABLE [dbo].[tbl_Station](
	[fld_StationId] [int] IDENTITY(1,1) NOT NULL,
	[fld_StationCode] NVARCHAR(128) NULL,
	[fld_StationName] [nvarchar](512) NULL,
	[fld_StationType] [int] NULL,
	[fld_StationActionType] [int] NULL,
	[fld_StationStatus] [int] NULL,
	[fld_StationReaders] [nvarchar](max) NULL,
	[fld_StationDescription] [nvarchar](1024) NULL,
	[fld_StationSettings] [nvarchar](max) NULL, 
    [fld_StationFromDestination] NVARCHAR(50) NULL, 
    [fld_StationToDestination] NVARCHAR(50) NULL, 
    [fld_StationMacAddress] NVARCHAR(50) NULL 
);

