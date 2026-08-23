CREATE TABLE [dbo].[tbl_DataMiningElements] (
    [fld_DataMiningElementsId]         INT            IDENTITY (1, 1) NOT NULL,
    [fld_DataMiningElementsTitle]      NVARCHAR (150)  NULL,
    [fld_DataMiningElementsCommand]    NVARCHAR (MAX) NULL,
    [fld_DataMiningElementsDesc]       NVARCHAR (MAX) NULL,
    [fld_DataMiningElementsParameters] NVARCHAR (MAX) NULL,
    [fld_DataMiningElementsType]       INT            NULL,
    [fld_DataMiningElementsUsageType]  INT            NULL,

    CONSTRAINT [IX_tbl_DataMiningElements] UNIQUE NONCLUSTERED ([fld_DataMiningElementsId] ASC)
);

