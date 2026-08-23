CREATE TABLE [dbo].[tbl_UserTokens](
	[fld_Id] [int] IDENTITY(1,1) NOT NULL,
	[fld_TokenValue] [nvarchar](1000) NOT NULL,
	[fld_TokenUserId] [nvarchar](128) NOT NULL,
	[fld_TokenHasExpired] [bit] NOT NULL
) ON [PRIMARY]
GO