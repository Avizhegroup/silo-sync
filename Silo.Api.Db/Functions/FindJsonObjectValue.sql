Create FUNCTION [dbo].[FindJsonObjectValue] 
(
	-- Add the parameters for the function here
	@JsonArray NVARCHAR(MAX) , 
	@JsonObjectKey NVARCHAR(MAX),
	@JsonObjectValue NVARCHAR(MAX),
	@JsonObjectValueKey NVARCHAR(MAX)
)
RETURNS nvarchar(256)
AS
BEGIN
	DECLARE @JSON_Path_Base  VARCHAR(MAX) =  N'$[i]."j"';
	DECLARE @JSON_Path  NVARCHAR(MAX);
	DECLARE @Index INT = 0;
	WHILE (@Index<50)
	BEGIN
	SET @JSON_Path = REPLACE(@JSON_Path_Base,'i',@Index);
	SET @JSON_Path = REPLACE(@JSON_Path,'j',@JsonObjectKey);
		IF JSON_VALUE(@JsonArray,@JSON_Path) IS NOT NULL
		BEGIN
			IF JSON_VALUE(@JsonArray,@JSON_Path) = @JsonObjectValue
			BEGIN
			SET @JSON_Path = REPLACE(@JSON_Path_Base,'i',CONVERT(VARCHAR,@Index));
			SET @JSON_Path = REPLACE(@JSON_Path,'j',@JsonObjectValueKey);
			RETURN JSON_VALUE(@JsonArray,@JSON_Path);
			END
		END
		SET @Index  = @Index  + 1
		IF @Index >48
		BEGIN
			BREAK
		END
	END

	-- Return the result of the function
	RETURN NULL;

END
