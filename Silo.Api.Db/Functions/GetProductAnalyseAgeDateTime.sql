-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetProductAnalyseAgeDateTime]
(
	-- Add the parameters for the function here
	@TagRegisterDate datetime 
)
RETURNS  nvarchar(20)
AS
BEGIN
	-- Declare the return variable here
Declare	@SHD	nvarchar(20) =  CASE WHEN ((@TagRegisterDate  < GETDATE()) AND 
                         (@TagRegisterDate >= DATEADD(month, - 1, GETDATE()))) 
                         THEN N'1' WHEN ((@TagRegisterDate < DATEADD(month, - 1, 
                         GETDATE())) AND (@TagRegisterDate >= DATEADD(month, - 3, 
                         GETDATE()))) THEN N'13' WHEN ((@TagRegisterDate < DATEADD(month, - 3, 
                         GETDATE())) AND (@TagRegisterDate >= DATEADD(month, - 6, 
                         GETDATE()))) THEN N'36' WHEN ((@TagRegisterDate < DATEADD(month, - 6, 
                         GETDATE())) AND (@TagRegisterDate >= DATEADD(month, - 12, 
                         GETDATE()))) THEN N'612' WHEN ((@TagRegisterDate < DATEADD(month, - 12, 
                         GETDATE()))) THEN N'1200' END  
 

	-- Return the result of the function
	RETURN 	@SHD

END
