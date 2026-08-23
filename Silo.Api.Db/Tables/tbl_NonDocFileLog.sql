CREATE TABLE [dbo].[tbl_NonDocFileLog] (
    [fld_NDFLId]   INT  IDENTITY (1, 1) NOT NULL,
    [fld_NDFLName] NVARCHAR (256) NULL,
    [fld_NDFLDateTime] DATETIME       NULL,
    [fld_NDFLType]     INT NULL,
    [fld_NDFLUser]     NVARCHAR (128) NULL,
    [fld_NDFLData]     NVARCHAR (MAX) NULL
);

