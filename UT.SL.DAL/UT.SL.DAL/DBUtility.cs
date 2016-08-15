using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using UT.SL.Helper;
using UT.SL.Data.LINQ;

namespace UT.SL.DAL
{
    public class DBUtility
    {
        public static string SocialLearningConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            }
        }

        public static SocialLearningDataContext GetSocialLearningDataContext
        {
            get
            {
                var dc = DataContextFactory.GetWebRequestScopedDataContext<SocialLearningDataContext>("Default", SocialLearningConnection);
                return dc;
            }
        }
    }
}
