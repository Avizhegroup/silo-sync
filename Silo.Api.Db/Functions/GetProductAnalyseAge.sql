-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetProductAnalyseAge]
(
	-- Add the parameters for the function here
	@TagRegisterDate nvarchar(20)
)
RETURNS  nvarchar(20)
AS
BEGIN
	-- Declare the return variable here
Declare	@SHD	nvarchar(20) =  CASE WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < GETDATE()) AND 
                         (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) + '/' + SUBSTRING(@TagRegisterDate, 7, 2), 
                         SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 1, GETDATE()))) 
                         THEN N'تا یک ماه' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 1, 
                         GETDATE())) AND (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 3, 
                         GETDATE()))) THEN N'یک تا سه ماه' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 3, 
                         GETDATE())) AND (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 6, 
                         GETDATE()))) THEN N'سه تا شش ماه' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 6, 
                         GETDATE())) AND (CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) >= DATEADD(month, - 12, 
                         GETDATE()))) THEN N'شش ماه تا یک سال' WHEN ((CAST(dbo.JalaliDateToGeorgianDate(SUBSTRING(@TagRegisterDate, 0, 5) + '/' + SUBSTRING(@TagRegisterDate, 5, 2) 
                         + '/' + SUBSTRING(@TagRegisterDate, 7, 2), SUBSTRING(@TagRegisterDate, 9, 2) + ':' + SUBSTRING(@TagRegisterDate, 11, 2)) AS datetime) < DATEADD(month, - 12, 
                         GETDATE()))) THEN N'بالای یک سال' END  
 

	-- Return the result of the function
	RETURN 	@SHD

END
