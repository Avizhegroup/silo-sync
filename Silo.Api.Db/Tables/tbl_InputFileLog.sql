CREATE TABLE [dbo].[tbl_InputFileLog] (
    [fld_InputId]       INT            IDENTITY (1, 1) NOT NULL,
    [fld_InputFileName] NVARCHAR (256) NULL,
    [fld_InputDateTime] DATETIME       NULL,
    [fld_InputType]     NVARCHAR (128) NULL,
    [fld_InputType1]    NVARCHAR (10)  NULL,
    [fld_InputType2]    NVARCHAR (10)  NULL,
    [fld_InputUser]     NVARCHAR (128) NULL,
    [fld_InputData]     NVARCHAR (MAX) NULL
);

