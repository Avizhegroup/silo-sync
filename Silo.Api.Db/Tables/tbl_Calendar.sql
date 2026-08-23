CREATE TABLE [dbo].[tbl_Calendar] (
    [id]              INT           IDENTITY (1, 1) NOT NULL,
    [DateJalali]      NVARCHAR (50) NULL,
    [DateShamsi]      NVARCHAR (50) NULL,
    [DayName]         NVARCHAR (50) NULL,
    [FlagStartSeason] INT           NULL,
    [FlagStartWeek]   INT           NULL,
    [FlagStartMonth]  INT           NULL,
    [FlagStartYear]   INT           NULL,
    [Year]            INT           NULL,
    [Month]           INT           NULL,
    [Day]             INT           NULL
);

