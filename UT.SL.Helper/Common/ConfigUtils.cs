using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Configuration;

namespace UT.SL.Helper
{
    public class ConfigUtils
    {
        public static string AppSettingsKey_SiteTitle = "SiteTitle";
        public static string AppSettingsKey_SiteName = "SiteName";
        public static string AppSettingsKey_SiteUrl = "SiteUrl";

        public static string GetAppSettingsValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConnectionString(string connectionName)
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }
    }
}
