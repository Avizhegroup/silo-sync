CREATE FUNCTION [dbo].[GeorgianDateToJalaliDate]
(
    @inputDate DATE
)
RETURNS VARCHAR(10)
AS
BEGIN
    IF @inputDate IS NULL RETURN NULL;

    DECLARE @gy0 INT = YEAR(@inputDate);
    DECLARE @gm INT  = MONTH(@inputDate);
    DECLARE @gd INT  = DAY(@inputDate);

    DECLARE @jy INT;  -- jalali year accumulator
    DECLARE @gy INT;  -- working gregorian year
    DECLARE @gy2 INT;
    DECLARE @days INT;
    DECLARE @g_d_m INT;
    DECLARE @jm INT;
    DECLARE @jd INT;
    DECLARE @result VARCHAR(10);

    SET @g_d_m =
        CASE @gm
            WHEN 1 THEN 0 WHEN 2 THEN 31 WHEN 3 THEN 59 WHEN 4 THEN 90
            WHEN 5 THEN 120 WHEN 6 THEN 151 WHEN 7 THEN 181 WHEN 8 THEN 212
            WHEN 9 THEN 243 WHEN 10 THEN 273 WHEN 11 THEN 304 WHEN 12 THEN 334
        END;

    IF @gy0 > 1600
    BEGIN
        SET @jy = 979;
        SET @gy = @gy0 - 1600;
    END
    ELSE
    BEGIN
        SET @jy = 0;
        SET @gy = @gy0 - 621;
    END

    IF @gm > 2
        SET @gy2 = @gy + 1;
    ELSE
        SET @gy2 = @gy;

    SET @days = 365 * @gy
                + (@gy2 + 3) / 4
                - (@gy2 + 99) / 100
                + (@gy2 + 399) / 400
                - 80
                + @gd
                + @g_d_m;

    SET @jy = @jy + 33 * (@days / 12053);
    SET @days = @days % 12053;

    SET @jy = @jy + 4 * (@days / 1461);
    SET @days = @days % 1461;

    IF @days > 365
    BEGIN
        SET @jy = @jy + (@days - 1) / 365;
        SET @days = (@days - 1) % 365;
    END

    IF @days < 186
    BEGIN
        SET @jm = 1 + (@days / 31);
        SET @jd = 1 + (@days % 31);
    END
    ELSE
    BEGIN
        SET @jm = 7 + ((@days - 186) / 30);
        SET @jd = 1 + ((@days - 186) % 30);
    END

    SET @result = RIGHT('0000' + CAST(@jy AS VARCHAR(4)), 4) + '/'
                + RIGHT('00'   + CAST(@jm AS VARCHAR(2)), 2) + '/'
                + RIGHT('00'   + CAST(@jd AS VARCHAR(2)), 2);

    RETURN @result;
END
GO


