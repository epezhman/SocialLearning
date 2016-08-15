using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UT.SL.Helper
{
    public static class RegXPattern
    {

        public const string Username = "/^[a-z0-9_-]{3,16}$/";
        public const string Password = "/^[a-z0-9_-]{6,18}$/";
        public const string Hex = "/^#?([a-f0-9]{6}|[a-f0-9]{3})$/";
        public const string Email = "^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$";
        public const string Url = @"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
        public const string IpAddress = @"/^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/";
        public const string DisallowCertainChar = @"^[^'<>?%!\s]{1,20}$";
        public const string Time = @"^(|(0\d)|(1[0-2])):(([0-5]\d)):(([0-5]\d))\s([AP]M)$";
        public const string Date = @"((19|20)[0-9]{2})-(([1-9])|(0[1-9])|(1[0-2]))-((3[0-1])|([0-2][0-9])|([0-9]))";
        public const string OnlyOneSpace = @"([a-zA-Z]{1}[a-zA-Z]*[\s]{0,1}[a-zA-Z])+([\s]{0,1}[a-zA-Z]+)";
        public const string Anyinteger = @"^[-+]?\d*$";
        public const string UnsighnedFloatNumber = @"^\d*\.?\d*$";
        public const string SignedFloatNumber = @"^[-+]?\d*\.?\d*$";
        public const string SignedInt = @"^(\+|-)?\d+$";
        public const string DateWithSlash = @"^\d{1,2}\/\d{1,2}\/\d{4}$";
        public const string Between1to999 = @"^[1-9][0-9]{0,2}$";
        public const string MobilePhone = @"^09[1|3][0-9]{8}$";
        public const string NationalCode = @"[0-9]{10}";
        public const string IranPostalCode = @"[1-9][0-9]{9}";
        public const string CertificateSN = @"[1-9][0-9]{5}";

        public static bool Match(string Text, string sPattern)
        {
            return Regex.IsMatch(Text, sPattern);
        }
    }
}
