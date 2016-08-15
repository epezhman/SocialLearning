using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class GroupMemberDAL
    {

        #region Get

        public static GroupMember Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static GroupMember Get(SocialLearningDataContext dc, int Id)
        {
            return dc.GroupMembers.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<GroupMember> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<GroupMember> GetAll(SocialLearningDataContext dc)
        {
            return dc.GroupMembers.ToList();
        }

        public static List<GroupMember> GetAll(int Id)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<GroupMember> GetAll(SocialLearningDataContext dc, int Id)
        {
            return dc.GroupMembers.Where(x => x.LearningGroupId == Id).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<GroupMember> Find(GroupMemberSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<GroupMember> Find(SocialLearningDataContext dc, GroupMemberSearchModel model)
        {
            var qry = from p in dc.GroupMembers select p;
            if (model != null)
            {
                if (model.SearchType.HasValue && model.SearchType > 0)
                {
                    qry = qry.Where(u => u.Type == model.SearchType);
                }
                if (model.SearchIsCircleAdmin.HasValue && model.SearchIsCircleAdmin > 0)
                {
                    qry = qry.Where(u => u.IsCircleAdmin == model.SearchIsCircleAdmin);
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

        public static DALReturnModel<GroupMember> Add(GroupMember model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<GroupMember> Add(SocialLearningDataContext dc, GroupMember model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new GroupMember();
                    if (model.Type > 0)
                        obj.Type = model.Type;
                    if (model.IsCircleAdmin > 0)
                        obj.IsCircleAdmin = model.IsCircleAdmin;
                    obj.UserId = model.UserId;
                    if (model.SocialGroupId > 0)
                        obj.SocialGroupId = model.SocialGroupId;
                    if (model.LearningGroupId > 0)
                        obj.LearningGroupId = model.LearningGroupId;
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

        public static DALReturnModel<GroupMember> AddMember(AddLearningGroupMemberModel model, BatchProcessResultModel bpr)
        {
            return AddMember(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<GroupMember> AddMember(SocialLearningDataContext dc, AddLearningGroupMemberModel model, BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            if (App_UserDAL.Get(model.Member) == null)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                noErrorFlag = false;
            }
            if (noErrorFlag && dc.GroupMembers.Any(x => x.LearningGroupId == model.GroupeId && x.UserId == App_UserDAL.Get(model.Member).Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new GroupMember();
                    obj.LearningGroupId = model.GroupeId;
                    obj.UserId = App_UserDAL.Get(model.Member).Id;
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

        #endregion

        #region Update

        public static DALReturnModel<GroupMember> Update(GroupMember model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<GroupMember> Update(SocialLearningDataContext dc, GroupMember model, BatchProcessResultModel bpr)
        {
            GroupMember obj = null;
            bool noErrorFlag = true;
            obj = dc.GroupMembers.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        if (model.Type > 0)
                            obj.Type = model.Type;
                        if (model.IsCircleAdmin > 0)
                            obj.IsCircleAdmin = model.IsCircleAdmin;
                        obj.UserId = model.UserId;
                        if (model.SocialGroupId > 0)
                            obj.SocialGroupId = model.SocialGroupId;
                        if (model.LearningGroupId > 0)
                            obj.LearningGroupId = model.LearningGroupId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 }, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<GroupMember> Delete(GroupMember model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<GroupMember> Delete(SocialLearningDataContext dc, GroupMember model)
        {
            try
            {
                var obj = dc.GroupMembers.Single(q => q.Id == model.Id);
                dc.GroupMembers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<GroupMember>(new GroupMember { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 });
            }
        }

        public static DALReturnModel<GroupMember> DeleteLearningGroupMember(int groupId, Guid userId)
        {
            return DeleteLearningGroupMember(DBUtility.GetSocialLearningDataContext, groupId, userId);
        }

        public static DALReturnModel<GroupMember> DeleteLearningGroupMember(SocialLearningDataContext dc, int groupId, Guid userId)
        {
            try
            {
                var obj = dc.GroupMembers.FirstOrDefault(q => q.App_User.GuidId == userId && q.LearningGroupId == groupId);
                if (obj != null)
                    dc.GroupMembers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<GroupMember>(new GroupMember { Id = groupId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 });
            }
        }

        public static DALReturnModel<GroupMember> DeleteSocialGroupMember(int groupId, Guid userId)
        {
            return DeleteSocialGroupMember(DBUtility.GetSocialLearningDataContext, groupId, userId);
        }

        public static DALReturnModel<GroupMember> DeleteSocialGroupMember(SocialLearningDataContext dc, int groupId, Guid userId)
        {
            try
            {
                var obj = dc.GroupMembers.FirstOrDefault(q => q.App_User.GuidId == userId && q.SocialGroupId == groupId);
                if (obj != null)
                    dc.GroupMembers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<GroupMember>(new GroupMember { Id = groupId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 });
            }
        }

        public static DALReturnModel<GroupMember> DeleteAll(int Id)
        {
            return DeleteAll(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static DALReturnModel<GroupMember> DeleteAll(SocialLearningDataContext dc, int Id)
        {
            try
            {
                foreach (var obj in dc.GroupMembers.Where(q => q.LearningGroupId == Id))
                {
                    dc.GroupMembers.DeleteOnSubmit(obj);
                    dc.SubmitChanges();
                }

                return new DALReturnModel<GroupMember>(new GroupMember { Id = Id }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<GroupMember>(new GroupMember { Id = 0 });
            }
        }

        #endregion
    }
}
