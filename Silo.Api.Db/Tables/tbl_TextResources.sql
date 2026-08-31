CREATE TABLE [dbo].[tbl_TextResources]
(
    [fld_TextResourceId]    INT             NOT NULL IDENTITY(1, 1),
    [fld_TextResourceKey]   NVARCHAR(512)   NOT NULL,
    [fld_TextResourceValue] NVARCHAR(MAX)   NULL,

    CONSTRAINT [PK_tbl_TextResources] PRIMARY KEY CLUSTERED ([fld_TextResourceId] ASC)
);