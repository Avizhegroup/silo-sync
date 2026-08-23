-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetExitProductAnalyseAge]
(
	-- Add the parameters for the function here
	@TagRegisterDate nvarchar(20),@TagExitDate nvarchar(10)
)
RETURNS  nvarchar(20)
AS
BEGIN
Declare	@ExitDate	datetime =CAST(dbo.JalaliDateToGeorgianDate(@TagExitDate,'00:00') as datetime);
	-- Declare the return variable here
Declare	@SHD	nvarchar(20) =  CASE WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < @ExitDate) AND 
                         (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) + '/' + SUBSTRING(@TagRegisterDate, 7, 2), 
                         SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 1, @ExitDate))) 
                         THEN N'تا یک ماه' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 1, 
                         @ExitDate)) AND (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 3, 
                         @ExitDate))) THEN N'یک تا سه ماه' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 3, 
                         @ExitDate)) AND (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 6, 
                         @ExitDate))) THEN N'سه تا شش ماه' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 6, 
                         @ExitDate)) AND (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 12, 
                         @ExitDate))) THEN N'شش ماه تا یک سال' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 12, 
                         @ExitDate))) THEN N'بالای یک سال' END  
 

	-- Return the result of the function
	RETURN 	@SHD

END
