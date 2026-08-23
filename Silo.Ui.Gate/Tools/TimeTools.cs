using System.Globalization;

namespace Silo.Ui.Gate.Tools;
public static class TimeTools
{
    public static class Time
    {
        public static string ConvertMiladi2ShamsiDate(DateTime date)
        {
            if (date.ToString(CultureInfo.InvariantCulture) != "01/01/0001 12:00:00 AM" &&
                date.ToString(CultureInfo.InvariantCulture) != "01/01/0001 12:00:00 ق.ظ")
            {
                var shamsi = new PersianCalendar();
                var ysh = shamsi.GetYear(date);
                var msh = shamsi.GetMonth(date);
                var dsh = shamsi.GetDayOfMonth(date);
                return $"{ysh}/{msh.ToString().PadLeft(2, '0')}/{dsh.ToString().PadLeft(2, '0')}";
            }
            else
            {
                var shamsi = new PersianCalendar();
                var ysh = shamsi.GetYear(DateTime.Now);
                var msh = shamsi.GetMonth(DateTime.Now);
                var dsh = shamsi.GetDayOfMonth(DateTime.Now);
                return $"{ysh}/{msh.ToString().PadLeft(2, '0')}/{dsh.ToString().PadLeft(2, '0')}";
            }
        }




        public static bool CheckDateInBeetweenToDate(string dateMaster, string dateFrom, string dateTo)
        {
            var flag = false;
            var dtFrom = ConvertShamsi2MiladiDate(dateFrom);
            var dtTo = ConvertShamsi2MiladiDate(dateTo);
            var dtMaster = ConvertShamsi2MiladiDate(dateMaster);
            if (dtMaster >= dtFrom && dtMaster <= dtTo)
            {
                flag = true;
            }

            return flag;
        }

        public static DateTime ConvertShamsi2MiladiDate(string date)
        {
            var year = int.Parse(date.Substring(0, 4));
            var month = int.Parse(date.Substring(5, 2));
            var day = int.Parse(date.Substring(8, 2));
            var p = new PersianCalendar();
            return p.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        public static string PersinaNowDate()
        {
            var pc = new PersianCalendar();
            var dt = DateTime.Now;
            var year = pc.GetYear(dt).ToString();
            var mounth = pc.GetMonth(dt).ToString();
            var day = pc.GetDayOfMonth(dt).ToString();
            return year + "/" + mounth.PadLeft(2, '0') + "/" + day.PadLeft(2, '0');
        }


        //public static int ExistWorkDayBetweenToDateShamsi(string fromDate, string toDate)
        //{
        //    try
        //    {
        //        var tempFromDateTime = ConvertShamsi2MiladiDate(fromDate);
        //        var tempToDateTime = ConvertShamsi2MiladiDate(toDate);
        //        int allDay = (Int32)(tempToDateTime - tempFromDateTime).TotalDays;
        //        BusinessLogic businessLogic = new BusinessLogic();


        //        SqlParameter[] countHoliday = businessLogic.SelectRow("SELECT Count(DateShamsi)    FROM tbl_WrkCal WHERE IsHoliday = 1 And DateShamsi>= '" + fromDate + "' And  DateShamsi < '" + toDate + "'");
        //        return allDay - Convert.ToInt32(countHoliday[0].SqlValue.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }

        //}

        public static string ConvertShortTimeToLongTime(string shortTime)
        {
            try
            {
                return shortTime.Remove(8);
            }
            catch
            {
                try
                {
                    return DateTime.Now.TimeOfDay.ToString().Remove(5);
                }
                catch
                {
                    try
                    {
                        return DateTime.Now.ToShortTimeString().Remove(8);
                    }
                    catch
                    {
                        return DateTime.Now.TimeOfDay.ToString();
                        //  MessageBox.Show("تنظیمات ساعت سیستم شما نادرست است. لطفا طبق راهنما اصلاح نمایید.");
                    }
                }
            }
        }

        public static string GetDayNameFromDateTime(DateTime dt)
        {
            var dayName = string.Empty;
            switch (dt.DayOfWeek.ToString())
            {
                case "Sunday":
                    dayName = "یکشنبه";
                    break;
                case "Monday":
                    dayName = "دوشنبه";
                    break;
                case "Tuesday":
                    dayName = "سه شنبه";
                    break;
                case "Wednesday":
                    dayName = "چهارشنبه";
                    break;
                case "Thursday":
                    dayName = "پنج شنبه";
                    break;
                case "Friday":
                    dayName = "جمعه";
                    break;
                case "Saturday":
                    dayName = "شنبه";
                    break;
            }

            return dayName;
        }

        public static string GetDayNumberFromDateTime(DateTime dt)
        {
            var dayName = string.Empty;
            switch (dt.DayOfWeek.ToString())
            {
                case "Sunday":
                    dayName = "1";
                    break;
                case "Monday":
                    dayName = "2";
                    break;
                case "Tuesday":
                    dayName = "3";
                    break;
                case "Wednesday":
                    dayName = "4";
                    break;
                case "Thursday":
                    dayName = "5";
                    break;
                case "Friday":
                    dayName = "6";
                    break;
                case "Saturday":
                    dayName = "0";
                    break;
            }

            return dayName;
        }

        public static int ExistMinBetweenToTime(string timeStart, string timeEnd)
        {
            try
            {
                var tempStart = timeStart.Split(':');
                var tempEnd = timeEnd.Split(':');

                return Convert.ToInt32(Convert.ToInt32(tempEnd[1]) - Convert.ToInt32(tempStart[1]) + (Convert.ToInt32(tempEnd[0]) - Convert.ToInt32(tempStart[0])) * 60);
            }
            catch
            {
                return 0;
            }
        }

        public static string AddMounthToShamsiDate(string shamsiDate)
        {
            var dateTemp = ConvertShamsi2MiladiDate(shamsiDate);
            dateTemp = dateTemp.AddMonths(1);
            var newDate = ConvertMiladi2ShamsiDate(dateTemp);
            return newDate;
        }

        public static string ConvertMinToHour(int minute)
        {
            var hour = minute / 60;
            var tempMinute = minute % 60;
            var tempHourAndMinute = hour.ToString().PadLeft(2, '0') + ":" + tempMinute.ToString().PadLeft(2, '0');
            return tempHourAndMinute;
        }

        public static int ConvertHourToMin(string time)
        {
            var timeTemp = time.Split(':');
            return Convert.ToInt32(timeTemp[0]) * 60 + Convert.ToInt32(timeTemp[1]);
        }

        public static int ConvertDateOrTimeToInteger(bool isDate, DateTime myDateTime)
        {
            return int.Parse(isDate ? $"{myDateTime.Year}{myDateTime.Month}{myDateTime.Day}" : $"{myDateTime.Hour}{myDateTime.Minute}");
        }
    }

}
