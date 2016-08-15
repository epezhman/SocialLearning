using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Helper
{
    public static class TimeUtils
    {
        /// <summary>
        /// Converts a time string to 4 digits int
        /// </summary>
        /// <param name="s">hh:mm</param>
        /// <returns></returns>
        public static int ToIntTime(this string s)
        {
            return (s.GetHour() * 100) + s.GetMinute();
        }

        /// <summary>
        /// Extract hour from a string of tme
        /// </summary>
        /// <param name="s">hh:mm</param>
        /// <returns></returns>
        public static int GetHour(this string s)
        {
            string[] sd = s.Split(':');
            if (sd.Length > 0)
                return sd[0].ToInt();
            else return 0;
        }

        /// <summary>
        /// Extract minutes from a string of tme
        /// </summary>
        /// <param name="s">hh:mm</param>
        /// <returns></returns>
        public static int GetMinute(this string s)
        {
            string[] sd = s.Split(':');
            if (sd.Length > 1)
                return sd[1].ToInt();
            else return 0;
        }
    }
}
