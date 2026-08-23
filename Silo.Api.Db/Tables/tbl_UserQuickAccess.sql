CREATE TABLE [dbo].[tbl_UserQuickAccess] (
    [fld_UserQuickAccessId]         INT             NOT NULL IDENTITY(1,1),
    [fld_UserQuickAccessUserId]     NVARCHAR(128)   NOT NULL,
    [fld_UserQuickAccessMenuLinkId] INT             NOT NULL,

)
