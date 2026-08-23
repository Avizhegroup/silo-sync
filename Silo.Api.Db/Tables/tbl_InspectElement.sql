CREATE TABLE [dbo].[tbl_InspectElement] (
    [fld_InspectElementId]            INT            IDENTITY (1, 1) NOT NULL,
    [fld_InspectElementName]          NVARCHAR (128) NULL,
    [fld_InspectElementType]          INT            NULL,
    [fld_InspectElementValue]         NVARCHAR (512) NULL,
    [fld_InspectElementMinValue]      INT            NULL,
    [fld_InspectElementMaxValue]      INT            NULL,
    [fld_InspectElementIsGatePrevent] BIT            NULL,
    [fld_InspectElementIsActive]      BIT            NULL,
    [fld_InspectElementIsRequired]    BIT            NULL,
    [fld_InspectElementProductTypes]  NVARCHAR (MAX) NULL,
    [fld_InspectElementOptions]       NVARCHAR (MAX) NULL,
    [fld_InspectElementRow]           INT            NULL
);

