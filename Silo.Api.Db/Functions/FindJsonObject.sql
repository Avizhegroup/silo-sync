CREATE FUNCTION [dbo].[FindJsonObject] 
(
	-- Add the parameters for the function here
	@JsonArray NVARCHAR(MAX) , 
	@JsonObjectKey NVARCHAR(MAX),
	@JsonObjectValue NVARCHAR(MAX)
)
RETURNS INT
AS
BEGIN
	DECLARE @ReturnJsonPath  NVARCHAR(MAX)
	DECLARE @JSON_Path  NVARCHAR(MAX)
	DECLARE @Index INT
	SET @Index = 0
	WHILE (@Index<50)
	BEGIN
	SET @JSON_Path =  N'$[i]."j"'
	SET @JSON_Path = REPLACE(@JSON_Path,'i',CONVERT(VARCHAR,@Index))
	SET @JSON_Path = REPLACE(@JSON_Path,'j',@JsonObjectKey)
		IF JSON_VALUE(@JsonArray,@JSON_Path) IS NOT NULL
		BEGIN
		SET @ReturnJsonPath = @JSON_Path
			IF JSON_VALUE(@JsonArray,@JSON_Path) != @JsonObjectValue
			BEGIN
			SET @Index = -1;
			BREAK
			END
		END
		SET @Index  = @Index  + 1
		IF @Index >48
		SET @Index = -1;
	END

	-- Return the result of the function
	RETURN @Index;

END
