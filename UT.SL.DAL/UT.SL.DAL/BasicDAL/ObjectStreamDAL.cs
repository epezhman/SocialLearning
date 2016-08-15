using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using System.Web.Mvc;


namespace UT.SL.DAL
{
    public partial class ObjectStreamDAL
    {

        #region Get

        public static ObjectStream Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ObjectStream Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ObjectStreams.SingleOrDefault(u => u.Id == Id);
        }

        public static ObjectStream Get(int objectId, int objectType)
        {
            return Get(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static ObjectStream Get(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.ObjectStreams.SingleOrDefault(u => u.ObjectId == objectId && u.ObjectType == objectType);
        }

        #endregion

        #region GetAll

        public static List<ObjectStream> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ObjectStream> GetAll(SocialLearningDataContext dc)
        {
            return dc.ObjectStreams.ToList();
        }

        public static List<ObjectStream> GetAll(int objectId, int objectType)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<ObjectStream> GetAll(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.ObjectStreams.Where(x => x.ObjectId == objectId && x.ObjectType == objectType).ToList();
        }

        public static IEnumerable<ObjectBackbone> GetNullBackbone()
        {
            return GetNullBackbone(DBUtility.GetSocialLearningDataContext);
        }

        public static IEnumerable<ObjectBackbone> GetNullBackbone(SocialLearningDataContext dc)
        {
            return new List<ObjectBackbone>();
        }

        public static IEnumerable<ObjectStream> GetAllForObjectAndUser(int objectId, int objectType, int userId)
        {
            return GetAllForObjectAndUser(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId);
        }

        public static IEnumerable<ObjectStream> GetAllForObjectAndUser(SocialLearningDataContext dc, int objectId, int objectType, int userId)
        {
            return dc.ObjectStreams.Where(x => x.ObjectId == objectId && x.ObjectType == objectType && x.UserId == userId).ToList();
        }

        public static IEnumerable<ObjectStream> GetAllBackbone(int? courseId, int userId, int? page, DateTime? date, FilterObjectModel filters)
        {
            return GetAllBackbone(DBUtility.GetSocialLearningDataContext, courseId, userId, page, date, filters);
        }

        public static IEnumerable<ObjectStream> GetAllBackbone(SocialLearningDataContext dc, int? courseId, int userId, int? page, DateTime? date, FilterObjectModel filters)
        {
            var result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.UserId == userId && x.CourseId == null) select m);
            if (filters.IsRecommended)
            {
                foreach (var item in result)
                {
                    item.KnowledgeCreditValue = UserKnowledgeProfileDAL.GetUserKnowledgeValue(courseId.Value, userId, item.ObjectId, item.ObjectType);
                    UpdateKnowledgeCreditValue(item.ObjectId, item.ObjectType, item.UserId, item.CourseId.Value, item.KnowledgeCreditValue.Value);
                }
                //result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.UserId == userId) select m);
            }
            if (filters.IsHot)
            {
                foreach (var item in result)
                {
                    item.InterestCreditValue = UserSoActProfileDAL.GetUserInterestValue(courseId.Value, userId, item.ObjectId, item.ObjectType);
                    UpdateInterestCreditValue(item.ObjectId, item.ObjectType, item.UserId, item.CourseId.Value, item.InterestCreditValue.Value);
                }
                //result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.UserId == userId) select m);
            }
            if (filters.IsResource)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource);
            }
            if (filters.IsActivity)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum);
            }
            if (filters.IsAssignments)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
            }
            if (filters.IsForums)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum);
            }
            if (filters.SocialGroupIds != null && filters.SocialGroupIds.Any())
            {
                var tempResult = new List<ObjectStream>();
                foreach (var item in filters.SocialGroupIds)
                {
                    tempResult.AddRange(result.Where(x => x.ObjectStreamGroupMappers.Any(p => p.SocialGroupId == item)));
                }
                result = tempResult.AsQueryable();
            }
            if (filters.LearningGroupIds != null && filters.LearningGroupIds.Any())
            {
                var tempResult = new List<ObjectStream>();
                foreach (var item in filters.LearningGroupIds)
                {
                    tempResult.AddRange(result.Where(x => x.ObjectStreamGroupMappers.Any(p => p.LearningGroupId == item)));
                }
                result = tempResult.AsQueryable();
            }

            // var distinctFilter = result.GroupBy(x => new { x.ObjectId, x.ObjectType }).Select(x => new { ObjectId = x.Key.ObjectId, ObjectType = x.Key.ObjectType, CreateDate = x.First().CreateDate }).AsQueryable();

            var distinctFilter = result.GroupBy(x => new { x.ObjectId, x.ObjectType, x.InterestCreditValue, x.KnowledgeCreditValue }).Select(x => new { ObjectId = x.Key.ObjectId, ObjectType = x.Key.ObjectType, InterestCreditValue = x.Key.InterestCreditValue, KnowledgeCreditValue = x.Key.KnowledgeCreditValue, CreateDate = x.First().CreateDate }).AsQueryable();
            //if (filters.IsRecommended)
            //{
            //    distinctFilter = distinctFilter.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate);
            //}
            //if (filters.IsHot)
            //{
            //    distinctFilter = distinctFilter.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate); ;
            //}

            if (page.HasValue)
            {
                if (!date.HasValue)
                {
                    var tempList = distinctFilter.OrderByDescending(x => x.CreateDate).Skip(page.Value * 10).Take(10).ToList();
                    if (filters.IsRecommended)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Skip(page.Value * 10).Take(10).ToList();
                    }
                    if (filters.IsHot)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Skip(page.Value * 10).Take(10).ToList();
                    }
                    //tempList.Reverse();
                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;

                }
                else
                {
                    var tempList = distinctFilter.OrderByDescending(x => x.CreateDate).Where(x => x.CreateDate < date.Value).Skip(page.Value * 10).Take(10).ToList();
                    //tempList.Reverse();
                    if (filters.IsRecommended)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Skip(page.Value * 10).Take(10).ToList();
                    }
                    if (filters.IsHot)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Skip(page.Value * 10).Take(10).ToList();
                    }
                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;
                }
            }
            else
            {
                if (!date.HasValue)
                {
                    var tempList = distinctFilter.OrderByDescending(x => x.CreateDate).Take(10).ToList();
                    // tempList.Reverse();
                    if (filters.IsRecommended)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Take(10).ToList();
                    }
                    if (filters.IsHot)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Take(10).ToList();
                    }
                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;
                }
                else
                {
                    var tempList = distinctFilter.OrderByDescending(x => x.CreateDate).Where(x => x.CreateDate < date.Value).Take(10).ToList();
                    // tempList.Reverse();
                    if (filters.IsRecommended)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Take(10).ToList();
                    }
                    if (filters.IsHot)
                    {
                        tempList = distinctFilter.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Take(10).ToList();
                    }
                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;
                }
            }
        }

        public static IEnumerable<ObjectStream> GetCreditBackbone(int? courseId, int userId, int? page, DateTime? date, FilterObjectModel filters)
        {
            return GetCreditBackbone(DBUtility.GetSocialLearningDataContext, courseId, userId, page, date, filters);
        }

        public static IEnumerable<ObjectStream> GetCreditBackbone(SocialLearningDataContext dc, int? courseId, int userId, int? page, DateTime? date, FilterObjectModel filters)
        {
            var result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.UserId == userId) select m);
            if (filters.IsRecommended)
            {
                foreach (var item in result)
                {
                    item.KnowledgeCreditValue = UserKnowledgeProfileDAL.GetUserKnowledgeValue(courseId.Value, userId, item.ObjectId, item.ObjectType);
                    UpdateKnowledgeCreditValue(item.ObjectId, item.ObjectType, item.UserId, item.CourseId.Value, item.KnowledgeCreditValue.Value);
                }
                result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.UserId == userId) select m);
            }
            if (filters.IsHot)
            {
                foreach (var item in result)
                {
                    item.InterestCreditValue = UserSoActProfileDAL.GetUserInterestValue(courseId.Value, userId, item.ObjectId, item.ObjectType);
                    UpdateInterestCreditValue(item.ObjectId, item.ObjectType, item.UserId, item.CourseId.Value, item.InterestCreditValue.Value);
                }
                result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate).Where(x => x.UserId == userId) select m);
            }
            if (filters.IsResource)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource);
            }
            if (filters.IsActivity)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum);
            }
            if (filters.SocialGroupIds != null && filters.SocialGroupIds.Any())
            {
                var tempResult = new List<ObjectStream>();
                foreach (var item in filters.SocialGroupIds)
                {
                    tempResult.AddRange(result.Where(x => x.ObjectStreamGroupMappers.Any(p => p.SocialGroupId == item)));
                }
                result = tempResult.AsQueryable();
            }
            if (filters.LearningGroupIds != null && filters.LearningGroupIds.Any())
            {
                var tempResult = new List<ObjectStream>();
                foreach (var item in filters.LearningGroupIds)
                {
                    tempResult.AddRange(result.Where(x => x.ObjectStreamGroupMappers.Any(p => p.LearningGroupId == item)));
                }
                result = tempResult.AsQueryable();
            }

            var distinctFilter = result.GroupBy(x => new { x.ObjectId, x.ObjectType, x.InterestCreditValue, x.KnowledgeCreditValue }).Select(x => new { ObjectId = x.Key.ObjectId, ObjectType = x.Key.ObjectType, InterestCreditValue = x.Key.InterestCreditValue, KnowledgeCreditValue = x.Key.KnowledgeCreditValue, CreateDate = x.First().CreateDate }).AsQueryable();
            if (filters.IsRecommended)
            {
                distinctFilter = distinctFilter.OrderByDescending(x => x.KnowledgeCreditValue).ThenByDescending(x => x.CreateDate);
            }
            if (filters.IsHot)
            {
                distinctFilter = distinctFilter.OrderByDescending(x => x.InterestCreditValue).ThenByDescending(x => x.CreateDate); ;
            }

            if (page.HasValue)
            {
                if (!date.HasValue)
                {
                    var tempList = distinctFilter.Skip(page.Value * 10).Take(10).ToList();
                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;

                }
                else
                {
                    var tempList = distinctFilter.Where(x => x.CreateDate < date.Value).Skip(page.Value * 10).Take(10).ToList();

                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;
                }
            }
            else
            {
                if (!date.HasValue)
                {
                    var tempList = distinctFilter.Take(10).ToList();

                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;
                }
                else
                {
                    var tempList = distinctFilter.Where(x => x.CreateDate < date.Value).Take(10).ToList();

                    var returnList = new List<ObjectStream>();
                    foreach (var item in tempList)
                    {
                        returnList.Add(new ObjectStream
                        {
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType
                        });
                    }
                    return returnList;
                }
            }
        }

        public static IEnumerable<ObjectStream> GetNewBackbone(int? courseId, int userId, DateTime date, FilterObjectModel filters)
        {
            return GetNewBackbone(DBUtility.GetSocialLearningDataContext, courseId, userId, date, filters);
        }

        public static IEnumerable<ObjectStream> GetNewBackbone(SocialLearningDataContext dc, int? courseId, int userId, DateTime date, FilterObjectModel filters)
        {
            var result = (courseId.HasValue ? from m in dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId) select m : from m in dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.UserId == userId) select m);

            if (filters.IsResource)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource);
            }
            if (filters.IsActivity)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum);
            }
            if (filters.IsForums)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum);
            }
            if (filters.IsAssignments)
            {
                result = result.Where(x => x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
            }
            if (filters.SocialGroupIds != null && filters.SocialGroupIds.Any())
            {
                var tempResult = new List<ObjectStream>();
                foreach (var item in filters.SocialGroupIds)
                {
                    tempResult.AddRange(result.Where(x => x.ObjectStreamGroupMappers.Any(p => p.SocialGroupId == item)));
                }
                result = tempResult.AsQueryable();
            }
            if (filters.LearningGroupIds != null && filters.LearningGroupIds.Any())
            {
                var tempResult = new List<ObjectStream>();
                foreach (var item in filters.LearningGroupIds)
                {
                    tempResult.AddRange(result.Where(x => x.ObjectStreamGroupMappers.Any(p => p.LearningGroupId == item)));
                }
                result = tempResult.AsQueryable();
            }
            var distinctFilter = result.GroupBy(x => new { x.ObjectId, x.ObjectType }).Select(x => new { ObjectId = x.Key.ObjectId, ObjectType = x.Key.ObjectType, CreateDate = x.First().CreateDate }).AsQueryable();

            var tempList = distinctFilter.Where(x => x.CreateDate >= date).OrderByDescending(x => x.CreateDate).ToList();
            //tempList.Reverse();
            var returnList = new List<ObjectStream>();
            foreach (var item in tempList)
            {
                returnList.Add(new ObjectStream
                {
                    ObjectId = item.ObjectId,
                    ObjectType = item.ObjectType
                });
            }
            return returnList;
        }

        public static IEnumerable<ObjectStream> GetAllForCourse(int courseId)
        {
            return GetAllForCourse(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static IEnumerable<ObjectStream> GetAllForCourse(SocialLearningDataContext dc, int courseId)
        {
            var result = dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId).ToList().Distinct(new ObjectStreamComparer()).AsQueryable();
            return result;
        }

        public static IEnumerable<ObjectStream> GetAllForCourseAndUser(int courseId, int userId)
        {
            return GetAllForCourseAndUser(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static IEnumerable<ObjectStream> GetAllForCourseAndUser(SocialLearningDataContext dc, int courseId, int userId)
        {
            var result = dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId).ToList().Distinct(new ObjectStreamComparer()).AsQueryable();
            return result;
        }

        public static IEnumerable<ObjectStream> GetAllForCourseAndUser2(int courseId, int userId)
        {
            return GetAllForCourseAndUser2(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static IEnumerable<ObjectStream> GetAllForCourseAndUser2(SocialLearningDataContext dc, int courseId, int userId)
        {
            //var result = dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId).ToList().Distinct(new ObjectStreamComparer()).AsQueryable();
            return dc.ObjectStreams.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId && x.UserId == userId).ToList();
        }

        public static List<App_User> GetAllAssociatedUser(int objectId, int type)
        {
            return GetAllAssociatedUser(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<App_User> GetAllAssociatedUser(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.ObjectStreams.Where(x => x.ObjectType == type && x.ObjectId == objectId).Select(x => x.App_User).Distinct().ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<ObjectStream> Add(ObjectStream model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectStream> Add(SocialLearningDataContext dc, ObjectStream model)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.SingleOrDefault(x => x.UserId == model.UserId && x.ObjectId == model.ObjectId && x.ObjectType == model.ObjectType);
            try
            {
                if (noErrorFlag)
                {
                    obj = new ObjectStream();
                    if (model.CourseId.HasValue)
                        obj.CourseId = model.CourseId;
                    obj.UserId = model.UserId;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CreateDate = DateTime.Now;
                    dc.ObjectStreams.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<ObjectStream>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
        }

        public static DALReturnModel<ObjectStream> AddForNewUser(ObjectStream model)
        {
            return AddForNewUser(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectStream> AddForNewUser(SocialLearningDataContext dc, ObjectStream model)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.SingleOrDefault(x => x.UserId == model.UserId && x.ObjectId == model.ObjectId && x.ObjectType == model.ObjectType);
            try
            {
                if (noErrorFlag && obj == null)
                {
                    obj = new ObjectStream();
                    if (model.CourseId.HasValue)
                        obj.CourseId = model.CourseId;
                    obj.UserId = model.UserId;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CreateDate = model.CreateDate;
                    dc.ObjectStreams.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<ObjectStream>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<ObjectStream> Update(ObjectStream model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ObjectStream> Update(SocialLearningDataContext dc, ObjectStream model, BatchProcessResultModel bpr)
        {
            ObjectStream obj = null;
            bool noErrorFlag = true;
            obj = dc.ObjectStreams.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ObjectStream>(obj, bpr);
        }

        public static DALReturnModel<ObjectStream> UpdateReadFlag(int objectId, int objectType, int userId)
        {
            return UpdateReadFlag(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId);
        }

        public static DALReturnModel<ObjectStream> UpdateReadFlag(SocialLearningDataContext dc, int objectId, int objectType, int userId)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId == userId);
            try
            {
                if (noErrorFlag && obj.Any())
                {
                    foreach (var item in obj)
                    {
                        if (!item.IsRead.HasValue || (item.IsRead.HasValue && !item.IsRead.Value))
                        {
                            item.IsRead = true;
                            item.ReadDate = DateTime.Now;
                            item.LastShownDate = DateTime.Now;
                            item.LastClcikedDate = DateTime.Now;
                            dc.SubmitChanges();
                        }

                    }
                }
            }
            catch
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 1 }); ///!!!wrong
        }

        public static byte StreamIsReadenOrEdited(int objectId, int objectType, int userId)
        {
            return StreamIsReadenOrEdited(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId);
        }

        public static byte StreamIsReadenOrEdited(SocialLearningDataContext dc, int objectId, int objectType, int userId)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId == userId);
            try
            {
                if (noErrorFlag && obj.Any())
                {
                    foreach (var item in obj)
                    {
                        if (item.IsRead.HasValue && item.IsRead.Value)
                        {
                            return 1;
                        }
                        if (item.IsRead.HasValue && !item.IsRead.Value && item.ReadDate.HasValue)
                        {
                            return 2;
                        }
                    }
                }
            }
            catch
            {
                return 0;
            }
            return 0;
        }

        public static DALReturnModel<ObjectStream> UpdateClickedDate(int objectId, int objectType, int userId)
        {
            return UpdateClickedDate(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId);
        }

        public static DALReturnModel<ObjectStream> UpdateClickedDate(SocialLearningDataContext dc, int objectId, int objectType, int userId)
        {
            bool noErrorFlag = true;
            var dates = new List<DateTime?>();
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId == userId).ToList();
            try
            {
                if (noErrorFlag && obj.Any())
                {
                    foreach (var item in obj)
                    {

                        dates.Add(item.LastClcikedDate);
                        item.IsRead = true;
                        item.LastClcikedDate = DateTime.Now;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0, LastClcikedDate = null });
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 1, LastClcikedDate = dates.Any(x => x.HasValue) ? dates.Max() : null });
        }

        public static DALReturnModel<ObjectStream> UpdateLastShownDate(int objectId, int objectType, int userId)
        {
            return UpdateLastShownDate(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId);
        }

        public static DALReturnModel<ObjectStream> UpdateLastShownDate(SocialLearningDataContext dc, int objectId, int objectType, int userId)
        {
            bool noErrorFlag = true;
            var dates = new List<DateTime?>();
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId == userId);
            try
            {
                if (noErrorFlag && obj.Any())
                {
                    foreach (var item in obj)
                    {
                        dates.Add(item.LastClcikedDate);
                        item.IsRead = true;
                        item.LastShownDate = DateTime.Now;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0, LastClcikedDate = null });
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 1, LastClcikedDate = dates.Any(x => x.HasValue) ? dates.Max() : null });
        }

        public static bool UpdateEditedFlag(int objectId, int objectType, int userId)
        {
            return UpdateEditedFlag(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId);
        }

        public static bool UpdateEditedFlag(SocialLearningDataContext dc, int objectId, int objectType, int userId)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId != userId);
            try
            {
                if (noErrorFlag && obj.Any())
                {
                    foreach (var item in obj)
                    {
                        item.IsRead = false;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }



        public static DALReturnModel<ObjectStream> UpdateKnowledgeCreditValue(int objectId, int objectType, int userId, int courseId, double creditValue)
        {
            return UpdateKnowledgeCreditValue(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId, courseId, creditValue);
        }

        public static DALReturnModel<ObjectStream> UpdateKnowledgeCreditValue(SocialLearningDataContext dc, int objectId, int objectType, int userId, int courseId, double creditValue)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId == userId && u.CourseId == courseId).ToList();
            try
            {
                if (noErrorFlag && obj.Count != 0)
                {
                    foreach (var item in obj)
                    {
                        item.KnowledgeCreditValue = creditValue;
                        dc.SubmitChanges();
                    }

                }
            }
            catch
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 1 }); ///!!!wrong
        }

        public static DALReturnModel<ObjectStream> UpdateInterestCreditValue(int objectId, int objectType, int userId, int courseId, double creditValue)
        {
            return UpdateInterestCreditValue(DBUtility.GetSocialLearningDataContext, objectId, objectType, userId, courseId, creditValue);
        }

        public static DALReturnModel<ObjectStream> UpdateInterestCreditValue(SocialLearningDataContext dc, int objectId, int objectType, int userId, int courseId, double creditValue)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreams.Where(u => u.ObjectId == objectId && u.ObjectType == objectType && u.UserId == userId && u.CourseId == courseId).ToList();
            try
            {
                if (noErrorFlag && obj.Count != 0)
                {
                    foreach (var item in obj)
                    {
                        item.InterestCreditValue = creditValue;
                        dc.SubmitChanges();
                    }

                }
            }
            catch
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
            }
            return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 1 }); ///!!!wrong
        }

        #endregion

        #region Delete

        public static DALReturnModel<ObjectStream> Delete(ObjectStream model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectStream> Delete(SocialLearningDataContext dc, ObjectStream model)
        {
            try
            {
                var obj = dc.ObjectStreams.Where(q => q.Id == model.Id);
                var toBeDeleted = obj.ToList();
                foreach (var item in toBeDeleted)
                {
                    var mappersToBeDelete = item.ObjectStreamGroupMappers.ToList();
                    foreach (var mapper in mappersToBeDelete)
                    {
                        dc.ObjectStreamGroupMappers.DeleteOnSubmit(mapper);
                        dc.SubmitChanges();
                    }
                    dc.ObjectStreams.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
            }
        }

        public static DALReturnModel<ObjectStream> DeleteFromStream(int objectId, int objectType)
        {
            return DeleteFromStream(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static DALReturnModel<ObjectStream> DeleteFromStream(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                var obj = dc.ObjectStreams.Where(q => q.ObjectId == objectId && q.ObjectType == objectType);
                var toBeDeleted = obj.ToList();
                foreach (var item in toBeDeleted)
                {
                    var mappersToBeDelete = item.ObjectStreamGroupMappers.ToList();
                    foreach (var mapper in mappersToBeDelete)
                    {
                        dc.ObjectStreamGroupMappers.DeleteOnSubmit(mapper);
                        dc.SubmitChanges();
                    }
                    dc.ObjectStreams.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = objectId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
            }

        }

        public static DALReturnModel<ObjectStream> DeleteFromStreamUser(int courseId, int userId)
        {
            return DeleteFromStreamUser(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static DALReturnModel<ObjectStream> DeleteFromStreamUser(SocialLearningDataContext dc, int courseId, int userId)
        {
            try
            {
                var obj = dc.ObjectStreams.Where(q => q.CourseId == courseId && q.UserId == userId);
                var toBeDeleted = obj.ToList();
                foreach (var item in toBeDeleted)
                {
                    var mappersToBeDelete = item.ObjectStreamGroupMappers.ToList();
                    foreach (var mapper in mappersToBeDelete)
                    {
                        dc.ObjectStreamGroupMappers.DeleteOnSubmit(mapper);
                        dc.SubmitChanges();
                    }
                    dc.ObjectStreams.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = courseId }); ///!!!wrong

            }
            catch (System.Exception)
            {
                return new DALReturnModel<ObjectStream>(new ObjectStream { Id = 0 });
            }
        }

        #endregion

        #region Find

        public static IQueryable<ObjectStream> Find(ObjectStreamSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<ObjectStream> Find(SocialLearningDataContext dc, ObjectStreamSearchModel model)
        {
            var qry = from p in dc.ObjectStreams select p;
            if (model != null)
            {
                if (model.IsCourse)
                {
                    qry = qry.Where(u => u.CourseId != null);
                }
                else
                {
                    qry = qry.Where(u => u.CourseId == null);
                }
                if (model.UserId != 0)
                {
                    qry = qry.Where(u => u.UserId == model.UserId);
                }
                if (model.ObjectType != 0)
                {
                    qry = qry.Where(u => u.ObjectType == model.ObjectType);
                }
                else
                {
                    var objType = new int[(int)UT.SL.Model.Enumeration.ObjectType.Resource, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, (int)UT.SL.Model.Enumeration.ObjectType.Forum];

                }
                qry = qry.Where(u => u.IsRead == null);

            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }

            //var result = qry.GroupBy(x => new { x.ObjectId, x.ObjectType, x.Id, x.UserId, x.CourseId, x.IsRead, x.KnowledgeCreditValue, x.InterestCreditValue }).Select(x => new { Id = x.Key.Id, UserId = x.Key.UserId, CreateDate = x.First().CreateDate, ObjectType = x.Key.ObjectType, ObjectId = x.Key.ObjectId, CourseId = x.Key.CourseId, IsRead= x.Key.IsRead , KnowledgeCreditValue = x.Key.KnowledgeCreditValue, InterestCreditValue = x.Key.InterestCreditValue }).AsQueryable();
            qry = qry.OrderByDescending(u => u.CreateDate).Take(20);

            return qry;
        }

        #endregion

        #region Reapir

        public static void Repair()
        {
            Repair(DBUtility.GetSocialLearningDataContext);
        }

        public static void Repair(SocialLearningDataContext dc)
        {
            var list = dc.ObjectStreams.Where(x => x.IsRead.HasValue && !x.ReadDate.HasValue).ToList();
            foreach (var item in list)
            {
                item.ReadDate = item.CreateDate;
                dc.SubmitChanges();
            }
        }

        public static void RepairResources()
        {
            RepairResources(DBUtility.GetSocialLearningDataContext);
        }

        public static void RepairResources(SocialLearningDataContext dc)
        {
            var list = dc.ObjectStreams.Where(x => x.ObjectType == 2).ToList();
            foreach (var item in list)
            {
                if (ResourceDAL.Get(item.ObjectId) == null)
                {
                    dc.ObjectStreams.DeleteOnSubmit(item);
                }

            }
            dc.SubmitChanges();
        }

        #endregion

    }
}


class ObjectStreamComparer : IEqualityComparer<ObjectStream>
{
    #region  IEqualityComparer<ObjectStream> Members

    public bool Equals(ObjectStream x, ObjectStream y)
    {
        return x.ObjectId.Equals(y.ObjectId);
    }

    public int GetHashCode(ObjectStream obj)
    {
        return obj.ObjectId.GetHashCode();
    }

    #endregion
}
