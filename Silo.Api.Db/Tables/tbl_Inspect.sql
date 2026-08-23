CREATE TABLE [dbo].[tbl_Inspect] (
    [fld_InspectId]              INT            IDENTITY (1, 1) NOT NULL,
    [fld_InspectDateTime]        DATETIME       NULL,
    [fld_InspectShamsiDate]      NVARCHAR (10)  NULL,
    [fld_InspectSerial]          NVARCHAR (50)  NULL,
    [fld_InspectUser]            NVARCHAR (50)  NULL,
    [fld_InspectResult]          INT            NULL,
    [fld_InspectElementsResults] NVARCHAR (MAX) NULL
);

