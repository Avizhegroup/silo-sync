CREATE TABLE [dbo].[tbl_CheckingRules] (
    [fld_CheckingRulesId]                INT            IDENTITY (1, 1) NOT NULL,
    [fld_CheckingRulesTitle]             NVARCHAR (50)  NULL,
    [fld_CheckingRulesCommand]           NVARCHAR (MAX) NULL,
    [fld_CheckingRulesStatus]            INT            NULL,
    [fld_CheckingRulesType]              INT            NULL,
    [fld_CheckingRulesStationCode]       NVARCHAR (50)  NULL,
    [fld_CheckingRulesReturnResultTrue]  NVARCHAR (MAX) NULL,
    [fld_CheckingRulesReturnResultFalse] NVARCHAR (MAX) NULL,
    [fld_CheckingRulesRegUser]           NVARCHAR (50)  NULL,
    [fld_CheckingRulesRegDate]           DATETIME       NULL,
    [fld_CheckingRulesResultType]        NVARCHAR (50)  NULL,
    [fld_CheckingRulesDefaultTypes]      NVARCHAR (256) CONSTRAINT [DF__tbl_Check__fld_C__473C8FC7] DEFAULT ((0)) NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_tbl_CheckingRules]
    ON [dbo].[tbl_CheckingRules]([fld_CheckingRulesTitle] ASC);

