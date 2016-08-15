using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class SocialGroupDAL
    {

        #region Get

        public static SocialGroup Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static SocialGroup Get(SocialLearningDataContext dc, int Id)
        {
            return dc.SocialGroups.SingleOrDefault(u => u.Id == Id);
        }

        public static GroupMember GetMembership(int Id)
        {
            return GetMembership(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static GroupMember GetMembership(SocialLearningDataContext dc, int Id)
        {
            return dc.GroupMembers.Single(q => q.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<SocialGroup> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<SocialGroup> GetAll(SocialLearningDataContext dc)
        {
            return dc.SocialGroups.ToList();
        }

        public static List<GroupMember> GetAllMemebrs(int Id)
        {
            return GetAllMemebrs(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<GroupMember> GetAllMemebrs(SocialLearningDataContext dc, int Id)
        {
            return dc.SocialGroups.SingleOrDefault(x => x.Id == Id).GroupMembers.ToList();
        }

        public static List<SocialGroup> GetAllByUserId(int Id)
        {
            return GetAllByUserId(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<SocialGroup> GetAllByUserId(SocialLearningDataContext dc, int Id)
        {
            return dc.SocialGroups.Where(x => x.CreateUserId == Id || x.GroupMembers.Any(p => p.UserId == Id)).ToList();
        }

        public static List<SocialGroup> GetAllCreatedByUserId(int Id)
        {
            return GetAllCreatedByUserId(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<SocialGroup> GetAllCreatedByUserId(SocialLearningDataContext dc, int Id)
        {
            return dc.SocialGroups.Where(x => x.CreateUserId == Id).ToList();
        }

        public static IEnumerable<ObjectStreamGroupMapper> GetAllStreamMapper(int groupId)
        {
            return GetAllStreamMapper(DBUtility.GetSocialLearningDataContext, groupId);
        }

        public static IEnumerable<ObjectStreamGroupMapper> GetAllStreamMapper(SocialLearningDataContext dc, int groupId)
        {
            return dc.ObjectStreamGroupMappers.Where(x => x.SocialGroupId == groupId).ToList().Distinct(new ObjectStreamGroupMapperComparer()).AsQueryable();
        }

        public static List<App_User> GetAllUserUveThisUIserInGoups(int userId)
        {
            return GetAllUserUveThisUIserInGoups(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<App_User> GetAllUserUveThisUIserInGoups(SocialLearningDataContext dc, int userId)
        {
            return dc.SocialGroups.Where(x => x.GroupMembers.Any(q => q.IsCircleAdmin != 1 && q.UserId == userId)).Select(x => x.App_User).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<SocialGroup> Find(SocialGroupSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<SocialGroup> Find(SocialLearningDataContext dc, SocialGroupSearchModel model)
        {
            var qry = from p in dc.SocialGroups select p;
            if (model != null)
            {
                qry = qry.Where(u => u.CreateUserId == model.userId);
                if (model.SearchType.HasValue && model.SearchType > 0)
                {
                    qry = qry.Where(u => u.Type == model.SearchType);
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderBy(u => u.Id);
            return qry;
        }

        #endregion

        #region Add
        public static DALReturnModel<SocialGroup> Add(SocialGroup model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<SocialGroup> Add(SocialLearningDataContext dc, SocialGroup model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new SocialGroup();
                    obj.Title = model.Title.StringNormalizer();
                    obj.Type = model.Type;
                    obj.CreateUserId = model.CreateUserId;
                    obj.CreateDate = DateTime.Now;
                    obj.About = model.About.StringNormalizer();
                    dc.SocialGroups.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<SocialGroup>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<SocialGroup>(new SocialGroup { Id = 0 }, bpr);
        }

        public static DALReturnModel<GroupMember> AddGroupMember(AddGroupMemberModel model, BatchProcessResultModel bpr)
        {
            return AddGroupMember(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<GroupMember> AddGroupMember(SocialLearningDataContext dc, AddGroupMemberModel model, BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            if (dc.GroupMembers.Any(x => x.SocialGroupId == model.GroupeId && x.UserId == App_UserDAL.Get(model.Member).Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new GroupMember();
                    obj.SocialGroupId = model.GroupeId;
                    obj.UserId = App_UserDAL.Get(model.Member).Id;
                    obj.IsCircleAdmin = model.IsAdmin;
                    dc.GroupMembers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<GroupMember>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 }, bpr);
        }

        public static DALReturnModel<GroupMember> AddSocialGroupMember(AddSocialGroupMemberModel model, BatchProcessResultModel bpr)
        {
            return AddSocialGroupMember(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<GroupMember> AddSocialGroupMember(SocialLearningDataContext dc, AddSocialGroupMemberModel model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (App_UserDAL.Get(model.Member) == null)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                noErrorFlag = false;
            }
            if (noErrorFlag && dc.GroupMembers.Any(x => x.SocialGroupId == model.GroupeId && x.UserId == App_UserDAL.Get(model.Member).Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new GroupMember();
                    obj.SocialGroupId = model.GroupeId;
                    obj.UserId = App_UserDAL.Get(model.Member).Id;
                    obj.IsCircleAdmin = model.IsAdmin;
                    dc.GroupMembers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<GroupMember>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 }, bpr);
        }

        public static DALReturnModel<ObjectStreamGroupMapper> AddObjectStreamGroup(int streamId, int gId)
        {
            return AddObjectStreamGroup(DBUtility.GetSocialLearningDataContext, streamId, gId);
        }

        public static DALReturnModel<ObjectStreamGroupMapper> AddObjectStreamGroup(SocialLearningDataContext dc, int streamId, int gId)
        {
            bool noErrorFlag = true;
            var objectStreamMapper = dc.ObjectStreamGroupMappers.SingleOrDefault(x => x.ObjectStreamId == streamId && x.SocialGroupId == gId);
            try
            {
                if (noErrorFlag && objectStreamMapper == null)
                {
                    objectStreamMapper = new ObjectStreamGroupMapper
                    {
                        ObjectStreamId = streamId,
                        SocialGroupId = gId
                    };
                    dc.ObjectStreamGroupMappers.InsertOnSubmit(objectStreamMapper);
                    dc.SubmitChanges();
                    return new DALReturnModel<ObjectStreamGroupMapper>(objectStreamMapper);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectStreamGroupMapper>(new ObjectStreamGroupMapper { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<SocialGroup> Update(SocialGroup model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<SocialGroup> Update(SocialLearningDataContext dc, SocialGroup model, BatchProcessResultModel bpr)
        {
            SocialGroup obj = null;
            bool noErrorFlag = true;
            obj = dc.SocialGroups.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.About = model.About.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<SocialGroup>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<SocialGroup> Delete(SocialGroup model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<SocialGroup> Delete(SocialLearningDataContext dc, SocialGroup model)
        {
            try
            {
                var obj = dc.SocialGroups.Single(q => q.Id == model.Id);
                foreach (var item in obj.GroupMembers.ToList())
                {
                    dc.GroupMembers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var item in dc.Shares.Where(x => x.SocialGroupId == model.Id).ToList())
                {
                    dc.Shares.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                dc.SocialGroups.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<SocialGroup>(new SocialGroup { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<SocialGroup>(new SocialGroup { Id = 0 });
            }
        }

        public static DALReturnModel<GroupMember> DeleteMember(int id)
        {
            return DeleteMember(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<GroupMember> DeleteMember(SocialLearningDataContext dc, int id)
        {
            try
            {
                var obj = dc.GroupMembers.Single(q => q.Id == id);
                dc.GroupMembers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<GroupMember>(new GroupMember { Id = id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 });
            }
        }

        public static DALReturnModel<SocialGroup> DeleteSocialMember(int id)
        {
            return DeleteSocialMember(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<SocialGroup> DeleteSocialMember(SocialLearningDataContext dc, int id)
        {
            int groupId = 0;
            try
            {
                var obj = dc.GroupMembers.Single(q => q.Id == id);
                groupId = obj.SocialGroupId.Value;
                dc.GroupMembers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<SocialGroup>(new SocialGroup { Id = id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<SocialGroup>(new SocialGroup { Id = id });
            }
        }

        public static DALReturnModel<SocialGroup> DeleteGroupMember(int gId, int userId)
        {
            return DeleteGroupMember(DBUtility.GetSocialLearningDataContext, gId, userId);
        }

        public static DALReturnModel<SocialGroup> DeleteGroupMember(SocialLearningDataContext dc, int gId, int userId)
        {
            try
            {
                var obj = dc.ObjectStreamGroupMappers.Where(q => q.SocialGroupId == gId && q.ObjectStream.UserId == userId);
                var toBeDeleted = obj.ToList();
                var streamIds = new List<int>();
                foreach (var item in toBeDeleted)
                {
                    streamIds.Add(item.ObjectStreamId);
                    dc.ObjectStreamGroupMappers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var item in streamIds)
                {
                    ObjectStreamDAL.Delete(new ObjectStream { Id = item });
                }
                return new DALReturnModel<SocialGroup>(new SocialGroup { Id = gId });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<SocialGroup>(new SocialGroup { Id = 0 });
            }
        }

        #endregion
    }
}

