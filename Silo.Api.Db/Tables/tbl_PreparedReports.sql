CREATE TABLE [dbo].[tbl_PreparedReports]
(
    [fld_PRId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [fld_PRTitle] NVARCHAR(128) NOT NULL,
    [fld_PRVariables] NVARCHAR(MAX) NULL,
    [fld_PRDataSources] NVARCHAR(MAX) NULL,
    [fld_PRImages] NVARCHAR(MAX) NULL, 
    [fld_PRUserId] NVARCHAR(128) NOT NULL, 
    [fld_PRReportFileName] NVARCHAR(128) NOT NULL
);
