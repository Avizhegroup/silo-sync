CREATE TABLE [dbo].[tbl_StructuredExternalConnection]
(
	[fld_SecId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[fld_SecType] NVARCHAR(128) NOT NULL,
    [fld_SecEventId] NVARCHAR(256) NOT NULL, 
    [fld_SecDateTime] DATETIME NOT NULL, 
    [fld_SecData] NVARCHAR(MAX) NOT NULL, 
    [fld_SecMethod] NVARCHAR(256) NOT NULL
)
