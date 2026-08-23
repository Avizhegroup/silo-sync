CREATE TABLE [dbo].[tbl_ChatSessions](
	[SessionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[SessionData] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
	[SessionMode] [int] NULL, 
    [TokenUsage] NVARCHAR(MAX) NULL, 
    [PriceUsage] DECIMAL(18, 8) NULL
) ON [PRIMARY]