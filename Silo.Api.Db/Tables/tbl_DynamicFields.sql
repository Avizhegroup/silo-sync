CREATE TABLE [dbo].[tbl_DynamicFields] (
    [fld_DynamicFieldId]            INT           IDENTITY (1, 1) NOT NULL,
    [fld_DynamicFieldTitle]         NVARCHAR (50) NULL,
    [fld_DynamicFieldType]          INT           NULL,
    [fld_IsSystematicField]         BIT           NULL,
    [fld_IsHeaderKey]               BIT           NULL,
    [fld_DynamicFieldUser]          NVARCHAR (50) NULL,
    [fld_DynamicFieldDateTime]      DATETIME      NULL,
    [fld_DynamicFieldRelatedTitle1] NVARCHAR (50) NULL,
    [fld_DynamicFieldRelatedTitle2] NVARCHAR (50) NULL,
    [fld_DynamicFieldRelatedTitle3] NVARCHAR (50) NULL,
    [fld_DynamicFieldActionType]    INT NULL,
    [fld_DynamicFieldShowColumn]    BIT DEFAULT ((0)) NULL,
    [fld_DynamicFieldShowColumnForAction] BIT DEFAULT ((0)) NULL,
    [fld_DynamicFieldDocGroupAggregate] BIT NULL DEFAULT ((0)),
    [fld_DynamicFieldValueType]     INT NULL,
	[fld_DynamicFieldDefaultValue]  NVARCHAR(128) NULL,
	[fld_DynamicFieldValueOptions]  NVARCHAR(max) NULL, 
    [fld_DynamicFieldRequirement] BIT NULL DEFAULT 0, 
    [fld_DynamicFieldOrder] INT NULL, 
    [fld_DynamicFieldSectionId] INT NULL, 
    [fld_DynamicFieldIsReadOnly] BIT NULL DEFAULT 0
);

