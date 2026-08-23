CREATE TABLE [dbo].[tbl_UserLogin] (
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (MAX) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_tbl_UserLogin] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

