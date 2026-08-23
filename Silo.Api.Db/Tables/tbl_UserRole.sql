CREATE TABLE [dbo].[tbl_UserRole] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.tbl_UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC)
);

