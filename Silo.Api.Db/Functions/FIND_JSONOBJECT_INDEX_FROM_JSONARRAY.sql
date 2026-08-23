-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	This method is uses to find index
-- =============================================
CREATE FUNCTION [dbo].[FIND_JSONOBJECT_INDEX_FROM_JSONARRAY] 
(
	-- Add the parameters for the function here
	@InputJsonArray NVARCHAR(MAX) , 
	@InputJsonObjectName NVARCHAR(MAX)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ReturnJsonPath  NVARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	

	DECLARE @JSON_Path  NVARCHAR(MAX)
	DECLARE @Index INT
	SET @Index = 0
	WHILE (@Index<50)
	BEGIN
	SET @JSON_Path =  N'$[i]."j"'
	SET @JSON_Path = REPLACE(@JSON_Path,'i',CONVERT(VARCHAR,@Index))
	SET @JSON_Path = REPLACE(@JSON_Path,'j',@InputJsonObjectName)
		IF JSON_VALUE(@InputJsonArray,@JSON_Path) IS NOT NULL
		BEGIN
		SET @ReturnJsonPath = @JSON_Path
		BREAK
		END
		SET @Index  = @Index  + 1
		IF @Index >48
		SET @ReturnJsonPath = N'$[0].NULL'
	END

	-- Return the result of the function
	RETURN @ReturnJsonPath

END
