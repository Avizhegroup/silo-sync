CREATE	Function [dbo].[JalaliDateToGeorgianDate]
( 
	@SHD varchar(10) ,@TM varchar(5)
)
Returns nvarchar(23)
As Begin


Declare	@Year 		Int,
	@Mon 		Int,
	@Day 		Int,
	@ShamsiDay 	Int,
	@M_SH_DayDiff	Int,
	@Div33 		Int,
	@Mod33 		Int,
	@KabCount 	Int,
	@NormalCount 	Int,
	@YearDayCount 	Int,
	@MonDayCount 	Int,
	@Div400 	Int,
	@Mod400 	Int,
	@Div100 	Int,
	@Mod100 	Int,
	@Div4 		Int,
	@Mod4 		Int,
	@Div1 		Int,
	@Mod1 		Int,
	@Value		Int,
	@Mon_Day 	Int,
	@FebDayCount 	Int
 
Select	@M_SH_DayDiff	= 226894, -- Days difference between SHamsi and Milladi Calender 
	@SHD		= Replace(@SHD , '-' , '/'),
	@Year		= Cast( SubString( @SHD,1,CharIndex ('/' , @SHD) - 1) As Int),
	@SHD		= SubString( @SHD , CharIndex ('/' , @SHD) + 1 , Len(@SHD)),
	@Mon		= Cast( SubString( @SHD,1,CharIndex ('/' ,@SHD) - 1) As Int),
	@SHD		= SubString( @SHD , CharIndex ('/' ,@SHD) + 1,Len(@SHD)),
	@Day		= Cast(@SHD As Int),
	@Div33		= (@Year - 1) / 33,
	@Mod33		= (@Year - 1) % 33,
	@KabCount	= Case
				When @Mod33<21	Then	(@Mod33 + 3) / 4
				Else			((@Mod33 + 3 - 21) / 4) + 5
			  End,
	@NormalCount	= @Mod33 - @KabCount,
	@YearDayCount	= @Div33 * (33 * 365 + 8) + @KabCount * 366 + @NormalCount * 365,
	@MonDayCount	= Case
				When @Mon<7 Then	(@Mon-1)*31
				Else 			6 * 31 + (@Mon - 7) * 30
			  End,
	@ShamsiDay	= @YearDayCount + @MonDayCount + @Day,
	@Value 		= @ShamsiDay + @M_SH_DayDiff,
	@Div400		= (@Value - 1) / (4 * (25 * 1461 - 1) + 1),
	@Mod400		= (@Value - 1) % (4 * (25 * 1461 - 1) + 1),
	@Div100		= @Mod400 / (25 * 1461 - 1),
	@Mod100		= @Mod400 % (25 * 1461 - 1),
	@Div4		= @Mod100 / 1461,
	@Mod4		= @Mod100 % 1461,
	@Div1		= Case
				When @Mod4 < 1095 Then	@Mod4/365
				Else 			3
			  End,
	@Mod1		= Case
				When @Mod4 < 1095 Then 	@Mod4 % 365 + 1
				Else 			@Mod4-1095+1
			  End,
	@Year		= @Div400 * 400 + @Div100 * 100 + @Div4 * 4 + @Div1 + 1,
	@FebDayCount	= Case
				When (@Year%4=0)and((@Year%100<>0)or(@Year%400=0))	Then	29
				Else								28
			  End,
	@Mon_Day	= Case 
		                  When @Mod1 <= 31 				Then 1 * 100 + @Mod1
		                  When @Mod1 <= 31 + @FebDayCount 		Then 2 * 100 + @Mod1 - 31
		                  When @Mod1 <= 2 * 31 + @FebDayCount 		Then 3 * 100 + @Mod1 - 31 - @FebDayCount
		                  When @Mod1 <= 2 * 31 + @FebDayCount + 30 	Then 4 * 100 + @Mod1 - 2 * 31 - @FebDayCount
		                  When @Mod1 <= 3 * 31 + @FebDayCount + 30 	Then 5 * 100 + @Mod1 - 2 * 31 - @FebDayCount - 30
		                  When @Mod1 <= 3 * 31 + @FebDayCount + 2 * 30 	Then 6 * 100 + @Mod1 - 3 * 31 - @FebDayCount - 30
		                  When @Mod1 <= 4 * 31 + @FebDayCount + 2 * 30 	Then 7 * 100 + @Mod1 - 3 * 31 - @FebDayCount - 2 * 30
		                  When @Mod1 <= 5 * 31 + @FebDayCount + 2 * 30 	Then 8 * 100 + @Mod1 - 4 * 31 - @FebDayCount - 2 * 30
		                  When @Mod1 <= 5 * 31 + @FebDayCount + 3 * 30 	Then 9 * 100 + @Mod1 - 5 * 31 - @FebDayCount - 2 * 30
		                  When @Mod1 <= 6 * 31 + @FebDayCount + 3 * 30 	Then 10 * 100 + @Mod1 - 5 * 31 - @FebDayCount - 3 * 30
		                  When @Mod1 <= 6 * 31 + @FebDayCount + 4 * 30 	Then 11 * 100 + @Mod1 - 6 * 31 - @FebDayCount - 3 * 30
		                  When @Mod1 <= 7 * 31 + @FebDayCount + 4 * 30 	Then 12 * 100 + @Mod1 - 6 * 31 - @FebDayCount - 4 * 30
	                    End,
	@Mon		= @Mon_Day / 100,
	@Day		= @Mon_Day % 100

Return 	( Cast((	Cast(@Year As Varchar(4)) + '-' +
	Replicate('0' , 2 - Len(@Mon)) + Cast(@Mon As  Varchar(2)) + '-' +
	Replicate('0' , 2 - Len(@Day)) + Cast(@Day As  Varchar(2))+' '+@TM+':00.001')  
	As nvarchar(23))
	) 
end
 