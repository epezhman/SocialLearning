using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{
    public partial class ObjectStreamGroupMapperDAL
    {

        #region Get

        public static ObjectStreamGroupMapper Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ObjectStreamGroupMapper Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ObjectStreamGroupMappers.SingleOrDefault(u => u.Id == Id);
        }

        public static bool IfAnyObjectInSocialGroupExist(int objectStreamId)
        {
            return IfAnyObjectInSocialGroupExist(DBUtility.GetSocialLearningDataContext, objectStreamId);
        }

        public static bool IfAnyObjectInSocialGroupExist(SocialLearningDataContext dc, int objectStreamId)
        {
            return dc.ObjectStreamGroupMappers.Any(u => u.ObjectStreamId == objectStreamId);
        }

        public static ObjectStreamGroupMapper GetByObjectStream(int objectStreamId)
        {
            return GetByObjectStream(DBUtility.GetSocialLearningDataContext, objectStreamId);
        }

        public static ObjectStreamGroupMapper GetByObjectStream(SocialLearningDataContext dc, int objectStreamId)
        {
            return dc.ObjectStreamGroupMappers.SingleOrDefault(u => u.ObjectStreamId == objectStreamId);
        }        
       
        #endregion
     
    }
}
 