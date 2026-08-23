CREATE TABLE [dbo].[tbl_ActionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[fld_ActionTypeId] [int] NULL,
	[fld_ActionTypeFromDestinationType] [nvarchar](50) NULL,
	[fld_ActionTypeToTypeDestinationType] [nvarchar](50) NULL,
	[fld_ActionTypeTitle] [nvarchar](50) NULL, 
    [fld_ActionTypeChangeDocStatus] NVARCHAR(50) NULL, 
    [fld_ActionTypePermitedDocStatus] NVARCHAR(50) NULL, 
    [fld_ActionTypeActiveControls] NVARCHAR(MAX) NULL, 
    [fld_ActionTypeRfidPower] INT NULL, 
    [fld_ActionTypeChangeTagLocation] INT NULL, 
    [fld_ActionTypeProductType] NVARCHAR(50) NULL
) ON [PRIMARY]