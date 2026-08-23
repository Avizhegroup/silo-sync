CREATE TABLE [dbo].[tbl_PrintFormats]
(
    [fld_Id] INT NOT NULL identity(1,1), 
    [fld_Name] NVARCHAR(256) NOT NULL, 
    [fld_PageTitle] NVARCHAR(256) NOT NULL, 
    [fld_Path] NVARCHAR(MAX) NOT NULL
)
