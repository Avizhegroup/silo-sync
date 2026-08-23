CREATE TABLE [dbo].[tbl_MenuLink](
	[fld_MenuLinkId] [int] NOT NULL,
	[fld_MenuLinkTitle] [nvarchar](256) NULL,
	[fld_MenuLinkParentId] [int] NULL,
	[fld_MenuLinkLevel] [int] NULL,
	[fld_MenuLinkUrl] [nvarchar](256) NULL,
	[fld_MenuLinkShown] [bit] NULL,
	[fld_MenuLinkIconName] [nvarchar](256) NULL, 
    [fld_MenuLinkIsUserDedicated] BIT NULL DEFAULT 0);
