using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Configuration;

namespace UT.SL.Helper
{
    public static class DateUtils
    {
        public static bool ValidateJilaliDate(string input, out DateTime jilaliDate)
        {
            jilaliDate = DateTime.MinValue;
            try
            {
                string[] dateSplits = input.Split(new char[] { '/' }, 3);
                int day, month, year;
                year = int.Parse(dateSplits[0]);
                month = int.Parse(dateSplits[1]);
                day = int.Parse(dateSplits[2]);
                if (day > 31 && year <= 31)
                {
                    int temp = year;
                    year = day;
                    day = temp;
                    string tempSplit = dateSplits[0];
                    dateSplits[0] = dateSplits[2];
                    dateSplits[2] = tempSplit;
                }
                if (dateSplits[0].Length <= 2)
                    year += 1300;

                PersianCalendar pCal = new PersianCalendar();
                DateTime date = pCal.ToDateTime(year, month, day, 0, 0, 0, 0);
                jilaliDate = date;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string Gregorian2Jilali(DateTime date)
        {
            if (date != DateTime.MinValue)
            {
                PersianCalendar pCal = new PersianCalendar();
                return String.Format("{0}/{1}/{2}", pCal.GetYear(date),
                    pCal.GetMonth(date), pCal.GetDayOfMonth(date));
            }
            else
                return string.Empty;
        }

        public static string Gregorian2Jilali(DateTime dt, string seperator = "/")
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(dt) + seperator + pc.GetMonth(dt) + seperator + pc.GetDayOfMonth(dt);
        }

        public static string JilaliYear(DateTime date)
        {
            if (date != DateTime.MinValue)
            {
                PersianCalendar pCal = new PersianCalendar();
                return String.Format("{0}", pCal.GetYear(date));
            }
            else
                return string.Empty;
        }

        public static DateTime GetFirstDateOfWeek(this DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }
        public static DateTime GetLastDateOfWeek(this DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime lastDayInWeek = dayInWeek.Date;
            while (lastDayInWeek.DayOfWeek != firstDay)
                lastDayInWeek = lastDayInWeek.AddDays(1);

            return lastDayInWeek;
        }

        public static int GetCurrentYear()
        {
            int year = Convert.ToInt32(DateUtils.JilaliYear(DateTime.Now));
            if (year > 100) year = year % 100;
            return year;
        }
        /// <summary>
        /// تبدیل تاریخ شمسی به میلادی
        /// </summary>
        /// <param name="date">yyyy/mm/dd</param>
        /// <param name="seperator">seperator default "/"</param>
        /// <returns></returns>
        public static DateTime Jalali2Gregorian(string date, string seperator = "/")
        {
            try
            {
                int indexFirst = date.IndexOf(seperator);
                int indexLast = date.LastIndexOf(seperator);
                string yy = date.Substring(0, indexFirst);
                string mm = date.Substring(indexFirst + 1, indexLast - (indexFirst + 1));
                string dd = date.Substring(indexLast + 1);
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(int.Parse(yy), int.Parse(mm), int.Parse(dd), 0, 0, 0, 0);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime Jalali2Gregorian(int yy, int mm, int dd)
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(yy, mm, dd, 0, 0, 0, 0);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// یک رشته تاریخ فارسی بدون جداکننده را گرفته و سعی در فرمت دهی آن می کند
        /// مثلا رشته زیر را گرفته و به شکل صحیح آن تبدیل می کند
        /// inputs  : 440213   , 13880701   , 139111     , 9121     , 91111    , 91611
        /// returns : 44/02/13 , 1388/07/01 , 1391/01/01 , 91/02/01 , 91/11/01 , 91/06/11
        /// </summary>
        /// <param name="persianDate"></param>
        /// <returns></returns>
        public static string ToDateFormat(this string persianDate)
        {
            int yy = 0, mm = 0, dd = 0;
            if (!string.IsNullOrEmpty(persianDate) && persianDate.Length > 3)
            {
                string md = string.Empty;
                if (persianDate.StartsWith("1"))
                {
                    yy = int.Parse(persianDate.Substring(0, 4));
                    md = persianDate.Substring(4);
                }
                else
                {
                    yy = int.Parse(persianDate.Substring(0, 2));
                    md = persianDate.Substring(2);
                }
                switch (md.Length)
                {
                    case 4:
                        mm = int.Parse(md.Substring(0, 2));
                        dd = int.Parse(md.Substring(2));
                        break;
                    case 2:
                        mm = int.Parse(md.Substring(0, 1));
                        dd = int.Parse(md.Substring(1));
                        break;
                    case 3:
                        if (md[0] < (char)50)
                        {
                            mm = int.Parse(md.Substring(0, 2));
                            dd = int.Parse(md.Substring(2));
                        }
                        else
                        {
                            mm = int.Parse(md.Substring(0, 1));
                            dd = int.Parse(md.Substring(1));
                        }
                        break;
                }
                return string.Format("{0}/{1:00}/{2:00}", yy, mm, dd);
            }
            return persianDate;
        }

        /// <summary>
        /// تبدیل یک رشته تاریخ فارسی به تاریخ میلادی
        /// yy/mm/dd
        /// </summary>
        /// <param name="persianDate">رشته تاریخ فارسی</param>
        /// <param name="time">در صورتی که بخواهیم زمان هم به تاریخ اضافه شود زمان هم وارد می کنیم وگرنه رشته خالی وارد می کنیم</param>
        /// <param name="seperator">مشخص می کنیم که بخش های تاریخ فارسی با چه جدا کننده ای جدا شده اند</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string persianDate, string time = "", char seperator = '/')
        {
            DateTime now = DateTime.Now;
            string[] part = persianDate.Split(seperator);
            PersianCalendar pcal = new PersianCalendar();
            if (part[0].Length <= 2) part[0] = "13" + part[0];
            if (!string.IsNullOrEmpty(time))
                now = pcal.ToDateTime(part[0].ToInt(), part[1].ToInt(), part[2].ToInt(), time.GetHour(), time.GetMinute(), 0, 0);
            else
                now = pcal.ToDateTime(part[0].ToInt(), part[1].ToInt(), part[2].ToInt(), 0, 0, 0, 0);
            return now;
        }

        public static DateTime ConvertToDateTime(this String persianDate)
        {
            string temp = persianDate.ToString();
            char[] delimiterChars = { ' ', '\r', '\t', '\n', '"', '/' };
            string[] details = temp.Split(delimiterChars);
            var pCal = new System.Globalization.PersianCalendar();
            if (details.Length != 3)
                return DateTime.Now.AddYears(-10).AddMinutes(-10).AddDays(-10);
            else
            {
                try
                {
                    return pCal.ToDateTime(Convert.ToInt32(details[2]), Convert.ToInt32(details[1]), Convert.ToInt32(details[0]), 0, 0, 0, 0);
                }
                catch
                {
                    try
                    {
                        return pCal.ToDateTime(Convert.ToInt32(details[0]), Convert.ToInt32(details[1]), Convert.ToInt32(details[2]), 0, 0, 0, 0);
                    }
                    catch
                    {
                        return DateTime.MinValue;
                    }
                }
            }
        }
        /// <summary>
        /// تاریخ را به رشته تاریخ شمسی تبدیل می کند.
        /// </summary>
        /// <param name="dt">تاریخی که می خواهد فارسی شود</param>
        /// <param name="seperator">جدا کننده ای که برای نمایش تاریخ فارسی استفاده شود را مشخص می کنیم </param>
        /// <returns></returns>
        public static string ToPersianDate(this DateTime dt, string seperator = "/")
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                string sy = pc.GetYear(dt).ToString();
                string sm = pc.GetMonth(dt).ToString("00");
                string sd = pc.GetDayOfMonth(dt).ToString("00");
                return sy + seperator + sm + seperator + sd;
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string ToPersianDateNoYear(this DateTime dt, string seperator = "/")
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                string sm = pc.GetMonth(dt).ToString("00");
                string sd = pc.GetDayOfMonth(dt).ToString("00");
                return seperator + sm + seperator + sd;
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string ToLittlePersianDate(this DateTime dt, string seperator = "/")
        {
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{1}{0}{2}{0}{3}", seperator, pc.GetYear(dt) % 1300, pc.GetMonth(dt), pc.GetDayOfMonth(dt));
        }

        public static string[] PersianDays = { "شنبه", "یک شنبه", "دو شنبه", "سه شنبه", "چهار شنبه", "پنج شنبه", "جمعه" };

        public static string GetPersianDayOfWeek(DayOfWeek dw, bool showBreif = false)
        {
            try
            {
                if (!showBreif)
                    switch (dw)
                    {
                        case DayOfWeek.Saturday: return PersianDays[0];
                        case DayOfWeek.Sunday: return PersianDays[1];
                        case DayOfWeek.Monday: return PersianDays[2];
                        case DayOfWeek.Tuesday: return PersianDays[3];
                        case DayOfWeek.Wednesday: return PersianDays[4];
                        case DayOfWeek.Thursday: return PersianDays[5];
                        case DayOfWeek.Friday: return PersianDays[6];
                    }
                else
                    switch (dw)
                    {
                        case DayOfWeek.Saturday: return "شنبه";
                        case DayOfWeek.Sunday: return "یک ";
                        case DayOfWeek.Monday: return "دو ";
                        case DayOfWeek.Tuesday: return "سه ";
                        case DayOfWeek.Wednesday: return "چهار";
                        case DayOfWeek.Thursday: return "پنج ";
                        case DayOfWeek.Friday: return "جمعه";
                    }
            }
            catch
            {

                throw;
            }

            return "";
        }

        public static string[] PersianMonth =   { 
             "فروردین" , "اردیبهشت" , "خرداد" 
            ,"تیر" , "مرداد" , "شهریور" 
        ,"مهر" , "آبان" , "آذر" 
        ,"دی" , "بهمن" , "اسفند" 
        };

        public static string ToPersianDateString(this DateTime dt, string seperator = " ")
        {
            PersianCalendar pc = new PersianCalendar();
            int m = pc.GetMonth(dt);
            return GetPersianDayOfWeek(dt.DayOfWeek) + " " + NumberUtils.NumToString(pc.GetDayOfMonth(dt).ToString()) + seperator + PersianMonth[m - 1] + seperator + pc.GetYear(dt);
        }

        /// <summary>
        /// Persian date with time
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string ToPersianDateTime(this DateTime dt, string seperator = "/")
        {
            return dt.ToPersianDate(seperator) + " " + string.Format("{0:00}:{1:00}", dt.Hour, dt.Minute);
        }

        /// <summary>
        /// difference of 2 datetimes in minutes
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static int GetDifferenceFrom(this DateTime dt, DateTime delta)
        {
            int rs = 0;

            TimeSpan ts = delta.Subtract(dt);
            rs = ts.Days * 24 * 60 + ts.Hours * 60 + ts.Minutes;
            return rs;
        }

        public static DateTime WesternizeDateTime(this DateTime d)
        {
            var pCal = new System.Globalization.PersianCalendar();
            try
            {
                if (d != null)
                {
                    var dateString = d.Date.ToString("yyyy/MM/dd");
                    var dateTokens = dateString.Split('/');
                    if (dateTokens.Count() == 3)
                    {
                        var year = 0;
                        if (Int32.TryParse(dateTokens[0], out year))
                        {
                            if (year > 1000 && year < 1500)
                            {
                                return pCal.ToDateTime(Convert.ToInt32(dateTokens[0]), Convert.ToInt32(dateTokens[1]), Convert.ToInt32(dateTokens[2]), 0, 0, 0, 0);
                            }
                            else if (year > 1700 && year < 2300)
                            {
                                return d;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return d;
        }

        public static string ToPersianDate(this DateTime? dt)
        {
            try
            {
                if (dt.HasValue)
                {
                    var dtst = dt.Value.ToShortDateString();
                    var tokens = dtst.Split('/');
                    var dtemp = string.Empty;
                    if (tokens[0].Length == 1)
                        dtemp += "0" + tokens[0] + "/";
                    if (tokens[1].Length == 1)
                        dtemp += "0" + tokens[1] + "/";
                    dtemp += tokens[2];

                }
                else return string.Empty;

            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public static DateTime? WesternizeDateTime(this DateTime? d)
        {
            var pCal = new System.Globalization.PersianCalendar();
            try
            {
                if (d.HasValue && d != null)
                {
                    var dateString = d.Value.Date.ToString("yyyy/MM/dd");
                    var dateTokens = dateString.Split('/');
                    if (dateTokens.Count() == 3)
                    {
                        var year = 0;
                        if (Int32.TryParse(dateTokens[0], out year))
                        {
                            if (year > 1000 && year < 1500)
                            {
                                return pCal.ToDateTime(Convert.ToInt32(dateTokens[0]), Convert.ToInt32(dateTokens[1]), Convert.ToInt32(dateTokens[2]), 0, 0, 0, 0);
                            }
                            else if (year > 1700 && year < 2300)
                            {
                                return d;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return d;
        }

    }
}
