CREATE TABLE [dbo].[tbl_Role] (
    [Id]             NVARCHAR (128) NOT NULL,
    [NormalizedName] NVARCHAR (512) NOT NULL,
    [Name]           NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.tbl_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

