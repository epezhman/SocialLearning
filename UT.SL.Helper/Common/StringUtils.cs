using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace UT.SL.Helper
{
    public static class StringUtils
    {

        #region Anti XSS

        public static string RemoteXSSBlackList(this string htmltext)
        {
            XDocument html = XDocument.Parse(htmltext);
            foreach (var script in html.Descendants("script"))
            {
                script.ToString();
            }

            string temp = htmltext.Replace("<script>", " ");
            return temp;
        }

        #endregion

        #region Strip HTML

        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(this string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(this string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        public static string HtmlStrip(this string input)
        {
            input = Regex.Replace(input, "<style>(.|\n)*?</style>", string.Empty);
            input = Regex.Replace(input, @"<xml>(.|\n)*?</xml>", string.Empty); // remove all <xml></xml> tags and anything inbetween.  
            return Regex.Replace(input, @"<(.|\n)*?>", string.Empty); // remove any tags but not there content "<p>bob<span> johnson</span></p>" becomes "bob johnson"
        }

        #endregion

        #region Convert

        /// <summary>
        /// تبدیل یک رشته عددی به یک عدد صحیح
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static string RemoveDangerousChars(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            string tmp = inputString.Replace("\\", "").Replace("--", "").Replace("=", "").Replace(">", "").Replace("<", "");
            tmp = tmp.Replace("#", "").Replace("%", "").Replace("'", "");
            tmp = tmp.Trim().TrimEnd().TrimStart();
            return tmp;
        }

        public static string StringNormalizer(this string s)
        {
            var st = new StringBuilder(s.RemoveDangerousChars().CorrectFarsiChars());
            return st.ToString();
        }

        public static string StringNormalizerOnlyFarsi(this string s)
        {
            var st = new StringBuilder(s.CorrectFarsiCharsOnlyChars());
            return st.ToString();
        }

        public static string ShortenStringForTitle(string one, string two, string three)
        {
            var result = string.Empty;
            var fullResult = string.Empty;
            if (!String.IsNullOrEmpty(one))
            {
                if (one.Length >= 30)
                {
                    result = one.Substring(0, 30) + "...";
                }
                else
                {
                    result = one;
                }
                fullResult = one;
            }
            else if (!String.IsNullOrEmpty(two))
            {
                if (two.Length >= 30)
                {
                    result = two.Substring(0, 30) + "...";
                }
                else
                {
                    result = two;
                }
                fullResult = two;
            }
            else if (!String.IsNullOrEmpty(three))
            {
                if (three.Length >= 30)
                {
                    result = three.Substring(0, 30) + "...";
                }
                else
                {
                    result = three;
                }
                fullResult = three;
            }
            return fullResult;
        }

        public static string ShortenStringForPreview(string one, string two, string three)
        {
            var result = string.Empty;
            var fullResult = string.Empty;
            if (!String.IsNullOrEmpty(one))
            {
                if (one.Length >= 30)
                {
                    result = one.Substring(0, 30) + "...";
                }
                else
                {
                    result = one;
                }
                fullResult = one;
            }
            else if (!String.IsNullOrEmpty(two))
            {
                if (two.Length >= 30)
                {
                    result = two.Substring(0, 30) + "...";
                }
                else
                {
                    result = two;
                }
                fullResult = two;
            }
            else if (!String.IsNullOrEmpty(three))
            {
                if (three.Length >= 30)
                {
                    result = three.Substring(0, 30) + "...";
                }
                else
                {
                    result = three;
                }
                fullResult = three;
            }
            return result.Replace("\n", "").StringNormalizer();
        }

        /// <summary>
        /// تبدیل یک سری حروف فارسی به یک کد استاندارد
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string CorrectFarsiChars(this string inputString)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                string tmp = inputString.RemoveDangerousChars();

                if (!string.IsNullOrEmpty(tmp))
                {
                    #region Correct 'ک'
                    tmp = tmp.Replace('\u0643', '\u06A9'); //ك 
                    tmp = tmp.Replace('\u06AA', '\u06A9'); //ڪ
                    tmp = tmp.Replace('\u06AB', '\u06A9'); //ګ
                    tmp = tmp.Replace('\u06AC', '\u06A9'); //ڬ
                    tmp = tmp.Replace('\u06AD', '\u06A9'); //ڭ
                    tmp = tmp.Replace('\u06AE', '\u06A9'); //ڮ
                    #endregion

                    #region Correct 'ي'
                    //tmp = tmp.Replace('\u0626', '\u064A'); //ئ
                    tmp = tmp.Replace('\u06CC', '\u064A'); //ی
                    tmp = tmp.Replace('\u06CD', '\u064A'); //ۍ
                    tmp = tmp.Replace('\u06CE', '\u064A'); //ێ
                    tmp = tmp.Replace('\u06D0', '\u064A'); //ې
                    tmp = tmp.Replace('\u06D1', '\u064A'); //ۑ
                    #endregion

                    #region Correct 'گ'
                    tmp = tmp.Replace('\u06B0', '\u06AF'); //ڰ 
                    tmp = tmp.Replace('\u06B1', '\u06AF'); //ڱ
                    tmp = tmp.Replace('\u06B2', '\u06AF'); //ڲ
                    tmp = tmp.Replace('\u06B3', '\u06AF'); //ڳ
                    tmp = tmp.Replace('\u06B4', '\u06A9'); //ڴ
                    #endregion
                }
                return tmp;
            }
            return inputString;
        }

        public static string CorrectFarsiCharsOnlyChars(this string inputString)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                string tmp = inputString;

                if (!string.IsNullOrEmpty(tmp))
                {
                    #region Correct 'ک'
                    tmp = tmp.Replace('\u0643', '\u06A9'); //ك 
                    tmp = tmp.Replace('\u06AA', '\u06A9'); //ڪ
                    tmp = tmp.Replace('\u06AB', '\u06A9'); //ګ
                    tmp = tmp.Replace('\u06AC', '\u06A9'); //ڬ
                    tmp = tmp.Replace('\u06AD', '\u06A9'); //ڭ
                    tmp = tmp.Replace('\u06AE', '\u06A9'); //ڮ
                    #endregion

                    #region Correct 'ي'
                    //tmp = tmp.Replace('\u0626', '\u064A'); //ئ
                    tmp = tmp.Replace('\u06CC', '\u064A'); //ی
                    tmp = tmp.Replace('\u06CD', '\u064A'); //ۍ
                    tmp = tmp.Replace('\u06CE', '\u064A'); //ێ
                    tmp = tmp.Replace('\u06D0', '\u064A'); //ې
                    tmp = tmp.Replace('\u06D1', '\u064A'); //ۑ
                    #endregion

                    #region Correct 'گ'
                    tmp = tmp.Replace('\u06B0', '\u06AF'); //ڰ 
                    tmp = tmp.Replace('\u06B1', '\u06AF'); //ڱ
                    tmp = tmp.Replace('\u06B2', '\u06AF'); //ڲ
                    tmp = tmp.Replace('\u06B3', '\u06AF'); //ڳ
                    tmp = tmp.Replace('\u06B4', '\u06A9'); //ڴ
                    #endregion
                }
                return tmp;
            }
            return inputString;
        }

        public static string UppercaseFirst(this string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Checks if Search String is valid
        /// </summary>
        /// <param name="nationalCode"></param>
        /// <returns></returns>
        public static bool IsValidSearchString(this string inputString, int minLength = 2)
        {
            bool rs = false;
            if (!string.IsNullOrEmpty(inputString) && inputString.Length > minLength)
            {
                string tmp = inputString.StringNormalizer();
                if (inputString.Length > minLength)
                    rs = true;
            }
            return rs;
        }

        /// <summary>
        /// Checks if National Code is valid
        /// </summary>
        /// <param name="nationalCode"></param>
        /// <returns></returns>
        public static bool IsValidNationalCode(this string nationalCode)
        {
            nationalCode = nationalCode.StringNormalizer();

            if (!string.IsNullOrEmpty(nationalCode))
            {
                if (nationalCode[0] == 'F')
                    return true;

                if (nationalCode.Length < 10)
                {
                    while (nationalCode.Length == 10)
                        nationalCode = "0" + nationalCode;
                }

                if (!Regex.IsMatch(nationalCode, RegXPattern.NationalCode))
                    return false;

                if (nationalCode == "1111111111" || nationalCode == "0000000000" || nationalCode == "2222222222" || nationalCode == "3333333333" || nationalCode == "4444444444" ||
                    nationalCode == "5555555555" || nationalCode == "6666666666" || nationalCode == "7777777777" || nationalCode == "8888888888" || nationalCode == "9999999999")
                {
                    return false;
                }
                int c = int.Parse((nationalCode[9]).ToString());
                int n = int.Parse(nationalCode[0].ToString()) * 10 +
                    int.Parse(nationalCode[1].ToString()) * 9 +
                     int.Parse(nationalCode[2].ToString()) * 8 +
                     int.Parse(nationalCode[3].ToString()) * 7 +
                    int.Parse(nationalCode[4].ToString()) * 6 +
                     int.Parse(nationalCode[5].ToString()) * 5 +
                     int.Parse(nationalCode[6].ToString()) * 4 +
                     int.Parse(nationalCode[7].ToString()) * 3 +
                     int.Parse(nationalCode[8].ToString()) * 2;
                int r = n - (n / 11) * 11;
                if ((r == 0 && r == c) || (r == 1 && c == 1) || (r > 1 && c == 11 - r))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if Email is valid
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string email)
        {
            bool rs = false;
            email = email.StringNormalizer();
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    Regex reg = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|edu|ir|ac|biz|info|mobi|name|aero|asia|jobs|museum)\b");
                    rs = reg.IsMatch(email);
                }
                catch (Exception)
                {
                    rs = false;
                }
            }
            return rs;
        }

        public static bool IsNumeric(this string txt)
        {
            bool rs = true;
            txt = txt.StringNormalizer();
            foreach (var c in txt.ToCharArray())
            {
                if (c < '0' || c > '9')
                    rs = false;
            }
            return rs;
        }

        public static string GetFirstLetters(this string str, int len, string addEndStr = "...")
        {
            str = str.StringNormalizer();
            if (str.Length > len)
                return str.Substring(0, len) + addEndStr;
            return str;
        }

        public static string FixLength(this string str, int len)
        {
            str = str.StringNormalizer();
            if (str.Length > len)
                str = str.Substring(0, len) + "...";
            return str;
        }

        #endregion

    }
}
