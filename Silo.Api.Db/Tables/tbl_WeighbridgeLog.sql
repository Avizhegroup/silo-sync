CREATE TABLE [dbo].[tbl_WeighbridgeLog] (
    [fld_WeighbridgeLogId]    INT   IDENTITY (1, 1) NOT NULL,
    [fld_WeighbridgeLogWeighbridgeCode] NVARCHAR (256) NULL,
    [fld_WeighbridgeLogWeight] DECIMAL (18, 2) NULL,
    [fld_WeighbridgeLogDateTime] DateTime  NULL,
    [fld_WeighbridgeLogShamsiDate] NVARCHAR (10) NULL
);

