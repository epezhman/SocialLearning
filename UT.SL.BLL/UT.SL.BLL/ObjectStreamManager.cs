/*
 * ****************************************************************
 * Filename:        ObjectStreamManager.cs 
 * version:         
 * Author's name:   Fatemeh orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         manage shared objects
 * ****************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Model;
using System.Data.Linq;
using UT.SL.Helper;
using UT.SL.Model.Enumeration;

namespace UT.SL.BLL
{
    /// <summary>
    /// it provide social share capability for objects
    /// </summary>
    public class ObjectStreamManager
    {

        /// <summary>
        /// used to shate an object with others
        /// </summary>
        /// <param name="objectId">id of the object</param>
        /// <param name="objectType">type of the object</param>
        /// <param name="Ids">id of people or groups to share with them</param>
        ///<param name="currentUserId">currentUserId</param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStream(int objectId, int objectType, ToEmergeIds Ids, RequestDetailModel RDM, int currentUserId = 0)
        {
            var objectModel = ManageObject.GetSharedObject(objectId, objectType);
            if (Ids != null)
            {
                if (Ids.LearningGroups != null && Ids.LearningGroups.Any())
                {
                    foreach (var item in Ids.LearningGroups)
                    {
                        var gId = 0;
                        if (Int32.TryParse(item, out gId))
                        {
                            var group = LearningGroupDAL.Get(gId);
                            if (group != null)
                            {
                                foreach (var member in group.GroupMembers)
                                {
                                    var objectStreamModel = new ObjectStream();
                                    if (objectModel.CourseId > 0)
                                        objectStreamModel.CourseId = objectModel.CourseId;
                                    objectStreamModel.ObjectId = objectModel.Id;
                                    objectStreamModel.ObjectType = objectModel.Type;
                                    objectStreamModel.UserId = member.UserId;
                                    RDM.ObjectType = (int)ObjectType.ObjectStream;
                                    var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                                    var streamId = drm.ReturnObject.Id;
                                    RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                                    var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(LearningGroupDAL.AddObjectStreamGroup), streamId, gId);
                                }
                                var objectGroupMapper = new ObjectGroupMapper
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = objectModel.Id,
                                    ObjectType = objectModel.Type,
                                    LearningGroupId = group.Id
                                };
                                RDM.ObjectType = (int)ObjectType.ObjectGroupMapper;
                                var drm3 = (DALReturnModel<ObjectGroupMapper>)ManageAction.ProxyCall(RDM, new Func<ObjectGroupMapper, DALReturnModel<ObjectGroupMapper>>(ObjectGroupMapperDAL.Add), objectGroupMapper);
                            }
                        }
                    }
                }
                if (Ids.SocialGroups != null && Ids.SocialGroups.Any())
                {
                    foreach (var item in Ids.SocialGroups)
                    {
                        var gId = 0;
                        if (Int32.TryParse(item, out gId))
                        {
                            var group = SocialGroupDAL.Get(gId);
                            if (group != null)
                            {
                                foreach (var member in group.GroupMembers)
                                {
                                    var objectStreamModel = new ObjectStream();
                                    if (objectModel.CourseId > 0)
                                        objectStreamModel.CourseId = objectModel.CourseId;
                                    objectStreamModel.ObjectId = objectModel.Id;
                                    objectStreamModel.ObjectType = objectModel.Type;
                                    objectStreamModel.UserId = member.UserId;
                                    RDM.ObjectType = (int)ObjectType.ObjectStream;
                                    var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                                    var streamId = drm.ReturnObject.Id;
                                    RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                                    var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(SocialGroupDAL.AddObjectStreamGroup), streamId, gId);
                                }
                                var objectGroupMapper = new ObjectGroupMapper
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = objectModel.Id,
                                    ObjectType = objectModel.Type,
                                    SocialGroupId = group.Id
                                };
                                RDM.ObjectType = (int)ObjectType.ObjectGroupMapper;
                                var drm3 = (DALReturnModel<ObjectGroupMapper>)ManageAction.ProxyCall(RDM, new Func<ObjectGroupMapper, DALReturnModel<ObjectGroupMapper>>(ObjectGroupMapperDAL.Add), objectGroupMapper);
                            }
                        }
                    }
                }
                if (Ids.ShareUserIds != null && Ids.ShareUserIds.Any())
                {
                    foreach (var item in Ids.ShareUserIds)
                    {
                        var uId = 0;
                        if (Int32.TryParse(item, out uId))
                        {
                            var user = App_UserDAL.Get(uId);
                            if (user != null)
                            {
                                var objectStreamModel = new ObjectStream();
                                if (objectModel.CourseId > 0)
                                    objectStreamModel.CourseId = objectModel.CourseId;
                                objectStreamModel.ObjectId = objectModel.Id;
                                objectStreamModel.ObjectType = objectModel.Type;
                                objectStreamModel.UserId = user.Id;
                                RDM.ObjectType = (int)ObjectType.ObjectStream;
                                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                            }
                        }
                    }
                }
                if (objectModel.CourseId > 0 && !Ids.LearningGroups.Any() && !Ids.SocialGroups.Any() && !Ids.ShareUserIds.Any())
                {
                    var course = CourseDAL.Get(objectModel.CourseId.Value);
                    if (course != null)
                    {
                        foreach (var item in course.App_UserEnrolements)
                        {
                            var objectStreamModel = new ObjectStream();
                            if (objectModel.CourseId > 0)
                                objectStreamModel.CourseId = objectModel.CourseId;
                            objectStreamModel.ObjectId = objectModel.Id;
                            objectStreamModel.ObjectType = objectModel.Type;
                            objectStreamModel.UserId = item.UserId;
                            RDM.ObjectType = (int)ObjectType.ObjectStream;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                        }
                        if (objectModel.CourseId > 0)
                        {
                            var objectStreamCourseModel = new ObjectStreamCourse();
                            objectStreamCourseModel.ObjectId = objectModel.Id;
                            objectStreamCourseModel.ObjectType = objectModel.Type;
                            objectStreamCourseModel.CourseId = objectModel.CourseId.Value;
                            ObjectStreamCourseDAL.Add(objectStreamCourseModel);
                            RDM.ObjectType = (int)ObjectType.ObjectStreamCourse;
                            var drm = (DALReturnModel<ObjectStreamCourse>)ManageAction.ProxyCall(RDM, new Func<ObjectStreamCourse, DALReturnModel<ObjectStreamCourse>>(ObjectStreamCourseDAL.Add), objectStreamCourseModel);
                        }
                    }
                }
                if (objectModel.CourseId == 0 && !Ids.LearningGroups.Any() && !Ids.SocialGroups.Any() && !Ids.ShareUserIds.Any() && currentUserId > 0)
                {
                    var objectStreamModel = new ObjectStream();
                    objectStreamModel.ObjectId = objectModel.Id;
                    objectStreamModel.ObjectType = objectModel.Type;
                    objectStreamModel.UserId = currentUserId;
                    RDM.ObjectType = (int)ObjectType.ObjectStream;
                    var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="Ids"></param>
        /// <param name="oldIds"></param>
        /// <param name="RDM"></param>
        public static void EditObjectResourceToStream(int objectId, int objectType, ToEmergeIds Ids, SharedGroupIds oldIds, RequestDetailModel RDM)
        {
            var objectModel = ManageObject.GetSharedObject(objectId, objectType);
            var toDeleteIds = new ToEmergeIds();
            var toAddNewIds = new ToEmergeIds();
            foreach (var item in Ids.LearningGroups)
            {
                if (!oldIds.LearningGroups.Any(x => x.ToString() == item))
                {
                    toAddNewIds.LearningGroups.Add(item.ToString());
                }
            }
            foreach (var item in Ids.SocialGroups)
            {
                if (!oldIds.SocialGroups.Any(x => x.ToString() == item))
                {
                    toAddNewIds.SocialGroups.Add(item.ToString());
                }
            }
            foreach (var item in Ids.ShareUserIds)
            {
                if (!oldIds.ShareUserIds.Any(x => x.ToString() == item))
                {
                    toAddNewIds.ShareUserIds.Add(item.ToString());
                }
            }
            foreach (var item in oldIds.LearningGroups)
            {
                if (!Ids.LearningGroups.Any(x => x == item.ToString()))
                {
                    toDeleteIds.LearningGroups.Add(item.ToString());
                }
            }
            foreach (var item in oldIds.SocialGroups)
            {
                if (!Ids.SocialGroups.Any(x => x == item.ToString()))
                {
                    toDeleteIds.SocialGroups.Add(item.ToString());
                }
            }
            foreach (var item in oldIds.ShareUserIds)
            {
                if (!Ids.ShareUserIds.Any(x => x == item.ToString()))
                {
                    toDeleteIds.ShareUserIds.Add(item.ToString());
                }
            }
            if (toAddNewIds.LearningGroups != null && toAddNewIds.LearningGroups.Any())
            {
                foreach (var item in toAddNewIds.LearningGroups)
                {
                    var gId = 0;
                    if (Int32.TryParse(item, out gId))
                    {
                        var group = LearningGroupDAL.Get(gId);
                        if (group != null)
                        {
                            foreach (var member in group.GroupMembers)
                            {
                                var objectStreamModel = new ObjectStream();
                                if (objectModel.CourseId > 0)
                                    objectStreamModel.CourseId = objectModel.CourseId;
                                objectStreamModel.ObjectId = objectModel.Id;
                                objectStreamModel.ObjectType = objectModel.Type;
                                objectStreamModel.UserId = member.UserId;
                                RDM.ObjectType = (int)ObjectType.ObjectStream;
                                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                                var streamId = drm.ReturnObject.Id;
                                RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                                var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(LearningGroupDAL.AddObjectStreamGroup), streamId, gId);
                            }
                            var objectGroupMapper = new ObjectGroupMapper
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = objectModel.Id,
                                ObjectType = objectModel.Type,
                                LearningGroupId = group.Id
                            };
                            RDM.ObjectType = (int)ObjectType.ObjectGroupMapper;
                            var drm3 = (DALReturnModel<ObjectGroupMapper>)ManageAction.ProxyCall(RDM, new Func<ObjectGroupMapper, DALReturnModel<ObjectGroupMapper>>(ObjectGroupMapperDAL.Add), objectGroupMapper);
                        }
                    }
                }
            }
            if (toAddNewIds.SocialGroups != null && toAddNewIds.SocialGroups.Any())
            {
                foreach (var item in toAddNewIds.SocialGroups)
                {
                    var gId = 0;
                    if (Int32.TryParse(item, out gId))
                    {
                        var group = SocialGroupDAL.Get(gId);
                        if (group != null)
                        {
                            foreach (var member in group.GroupMembers)
                            {
                                var objectStreamModel = new ObjectStream();
                                if (objectModel.CourseId > 0)
                                    objectStreamModel.CourseId = objectModel.CourseId;
                                objectStreamModel.ObjectId = objectModel.Id;
                                objectStreamModel.ObjectType = objectModel.Type;
                                objectStreamModel.UserId = member.UserId;
                                RDM.ObjectType = (int)ObjectType.ObjectStream;
                                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                                var streamId = drm.ReturnObject.Id;
                                RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                                var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(SocialGroupDAL.AddObjectStreamGroup), streamId, gId);
                            }
                            var objectGroupMapper = new ObjectGroupMapper
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = objectModel.Id,
                                ObjectType = objectModel.Type,
                                SocialGroupId = group.Id
                            };
                            RDM.ObjectType = (int)ObjectType.ObjectGroupMapper;
                            var drm3 = (DALReturnModel<ObjectGroupMapper>)ManageAction.ProxyCall(RDM, new Func<ObjectGroupMapper, DALReturnModel<ObjectGroupMapper>>(ObjectGroupMapperDAL.Add), objectGroupMapper);
                        }
                    }
                }
            }
            if (toAddNewIds.ShareUserIds != null && toAddNewIds.ShareUserIds.Any())
            {
                foreach (var item in Ids.ShareUserIds)
                {
                    var uId = 0;
                    if (Int32.TryParse(item, out uId))
                    {
                        var user = App_UserDAL.Get(uId);
                        if (user != null)
                        {
                            var objectStreamModel = new ObjectStream();
                            if (objectModel.CourseId > 0)
                                objectStreamModel.CourseId = objectModel.CourseId;
                            objectStreamModel.ObjectId = objectModel.Id;
                            objectStreamModel.ObjectType = objectModel.Type;
                            objectStreamModel.UserId = user.Id;
                            RDM.ObjectType = (int)ObjectType.ObjectStream;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                        }
                    }
                }
            }
            if (toDeleteIds.LearningGroups != null && toDeleteIds.LearningGroups.Any())
            {
                foreach (var item in toDeleteIds.LearningGroups)
                {
                    var gId = 0;
                    if (Int32.TryParse(item, out gId))
                    {
                        var group = LearningGroupDAL.Get(gId);
                        if (group != null)
                        {
                            foreach (var member in group.GroupMembers)
                            {
                                var objectStreamModel = new ObjectStream();
                                if (objectModel.CourseId > 0)
                                    objectStreamModel.CourseId = objectModel.CourseId;
                                objectStreamModel.ObjectId = objectModel.Id;
                                objectStreamModel.ObjectType = objectModel.Type;
                                objectStreamModel.UserId = member.UserId;
                                RDM.ObjectType = (int)ObjectType.ObjectStream;
                                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                                var streamId = drm.ReturnObject.Id;
                                RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                                var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(LearningGroupDAL.AddObjectStreamGroup), streamId, gId);
                            }
                            var objectGroupMapper = new ObjectGroupMapper
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = objectModel.Id,
                                ObjectType = objectModel.Type,
                                LearningGroupId = group.Id
                            };
                            RDM.ObjectType = (int)ObjectType.ObjectGroupMapper;
                            var drm3 = (DALReturnModel<ObjectGroupMapper>)ManageAction.ProxyCall(RDM, new Func<ObjectGroupMapper, DALReturnModel<ObjectGroupMapper>>(ObjectGroupMapperDAL.Delete), objectGroupMapper);
                        }
                    }
                }
            }
            if (toDeleteIds.SocialGroups != null && toDeleteIds.SocialGroups.Any())
            {
                foreach (var item in toDeleteIds.SocialGroups)
                {
                    var gId = 0;
                    if (Int32.TryParse(item, out gId))
                    {
                        var group = SocialGroupDAL.Get(gId);
                        if (group != null)
                        {
                            foreach (var member in group.GroupMembers)
                            {
                                var objectStreamModel = new ObjectStream();
                                if (objectModel.CourseId > 0)
                                    objectStreamModel.CourseId = objectModel.CourseId;
                                objectStreamModel.ObjectId = objectModel.Id;
                                objectStreamModel.ObjectType = objectModel.Type;
                                objectStreamModel.UserId = member.UserId;
                                RDM.ObjectType = (int)ObjectType.ObjectStream;
                                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
                                var streamId = drm.ReturnObject.Id;
                                //SocialGroupDAL.Delete(streamId, gId);
                            }
                            var objectGroupMapper = new ObjectGroupMapper
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = objectModel.Id,
                                ObjectType = objectModel.Type,
                                SocialGroupId = group.Id
                            };
                            RDM.ObjectType = (int)ObjectType.ObjectGroupMapper;
                            var drm3 = (DALReturnModel<ObjectGroupMapper>)ManageAction.ProxyCall(RDM, new Func<ObjectGroupMapper, DALReturnModel<ObjectGroupMapper>>(ObjectGroupMapperDAL.Delete), objectGroupMapper);
                        }
                    }
                }
            }
            if (toDeleteIds.ShareUserIds != null && toDeleteIds.ShareUserIds.Any())
            {
                foreach (var item in toDeleteIds.ShareUserIds)
                {
                    var uId = 0;
                    if (Int32.TryParse(item, out uId))
                    {
                        var user = App_UserDAL.Get(uId);
                        if (user != null)
                        {
                            var objectStreamModel = new ObjectStream();
                            if (objectModel.CourseId > 0)
                                objectStreamModel.CourseId = objectModel.CourseId;
                            objectStreamModel.ObjectId = objectModel.Id;
                            objectStreamModel.ObjectType = objectModel.Type;
                            objectStreamModel.UserId = user.Id;
                            RDM.ObjectType = (int)ObjectType.ObjectStream;
                            var drm3 = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Delete), objectStreamModel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        public static SharedGroupIds FindObjectWhichStream(int objectId, int objectType)
        {
            var model = new SharedGroupIds();
            model.CourseIds = ObjectStreamCourseDAL.GetAllWithIdAndType(objectId, objectType);
            model.LearningGroups = ObjectGroupMapperDAL.GetAllLearningWithObjectIdAndType(objectId, objectType);
            model.SocialGroups = ObjectGroupMapperDAL.GetAllSocialWithObjectIdAndType(objectId, objectType);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="userId"></param>
        public static void UpdateEditedObjectStream(int objectId, int objectType, int userId)
        {
            ObjectStreamDAL.UpdateEditedFlag(objectId, objectType, userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStreamForNewCourseMemeber(int courseId, int userId, RequestDetailModel RDM)
        {
            var objectStreams = ObjectStreamCourseDAL.GetAllForCourse(courseId);
            foreach (var item in objectStreams.OrderBy(x => x.CreateDate))
            {
                var objectStreamModel = new ObjectStream();
                objectStreamModel.CourseId = item.CourseId;
                objectStreamModel.ObjectId = item.ObjectId;
                objectStreamModel.ObjectType = item.ObjectType;
                objectStreamModel.CreateDate = item.CreateDate;
                objectStreamModel.UserId = userId;
                RDM.ObjectType = (int)ObjectType.ObjectStream;
                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Add), objectStreamModel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="learningGroupId"></param>
        /// <param name="userId"></param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStreamForNewLearningGroupMemeber(int learningGroupId, int userId, RequestDetailModel RDM)
        {
            var objectStreamGroup = LearningGroupDAL.GetAllStreamMapper(learningGroupId);
            foreach (var item in objectStreamGroup)
            {
                var objectStreamModel = new ObjectStream();
                if (item.ObjectStream.CourseId.HasValue)
                    objectStreamModel.CourseId = item.ObjectStream.CourseId;
                objectStreamModel.ObjectId = item.ObjectStream.ObjectId;
                objectStreamModel.ObjectType = item.ObjectStream.ObjectId;
                objectStreamModel.CreateDate = item.ObjectStream.CreateDate;
                objectStreamModel.UserId = userId;
                RDM.ObjectType = (int)ObjectType.ObjectStream;
                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.AddForNewUser), objectStreamModel);
                var streamId = drm.ReturnObject.Id;
                RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(LearningGroupDAL.AddObjectStreamGroup), streamId, learningGroupId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socialGroupId"></param>
        /// <param name="userId"></param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStreamForNewSocialGroupMemeber(int socialGroupId, int userId, RequestDetailModel RDM)
        {
            var objectStreamGroup = SocialGroupDAL.GetAllStreamMapper(socialGroupId);
            foreach (var item in objectStreamGroup)
            {
                var objectStreamModel = new ObjectStream();
                if (item.ObjectStream.CourseId.HasValue)
                    objectStreamModel.CourseId = item.ObjectStream.CourseId;
                objectStreamModel.ObjectId = item.ObjectStream.ObjectId;
                objectStreamModel.ObjectType = item.ObjectStream.ObjectId;
                objectStreamModel.CreateDate = item.ObjectStream.CreateDate;
                objectStreamModel.UserId = userId;
                RDM.ObjectType = (int)ObjectType.ObjectStream;
                var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.AddForNewUser), objectStreamModel);
                var streamId = drm.ReturnObject.Id;
                RDM.ObjectType = (int)ObjectType.ObjectStreamGroupMapper;
                var drm2 = (DALReturnModel<ObjectStreamGroupMapper>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStreamGroupMapper>>(SocialGroupDAL.AddObjectStreamGroup), streamId, socialGroupId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStreamForRemoveCourseMemeber(int courseId, int userId, RequestDetailModel RDM)
        {
            RDM.ObjectType = (int)ObjectType.ObjectStream;
            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.DeleteFromStreamUser), courseId, userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="learningGroupId"></param>
        /// <param name="userId"></param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStreamForRemoveLearningGroupMemeber(int learningGroupId, int userId, RequestDetailModel RDM)
        {
            RDM.ObjectType = (int)ObjectType.LearningGroup;
            var drm = (DALReturnModel<LearningGroup>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<LearningGroup>>(LearningGroupDAL.DeleteGroupMember), learningGroupId, userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socialGroupId"></param>
        /// <param name="userId"></param>
        /// <param name="RDM"></param>
        public static void ObjectResourceToStreamForRemoveSocialGroupMemeber(int socialGroupId, int userId, RequestDetailModel RDM)
        {
            RDM.ObjectType = (int)ObjectType.SocialGroup;
            var drm = (DALReturnModel<SocialGroup>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<SocialGroup>>(SocialGroupDAL.DeleteGroupMember), socialGroupId, userId);
        }

        /// <summary>
        /// Delete an object from stream
        /// </summary>
        /// <param name="objectId">id of the object</param>
        /// <param name="objectType">type of the object</param>
        /// <param name="RDM"></param>
        public static void ObjectResourceDeleteFromStream(int objectId, int objectType, RequestDetailModel RDM)
        {
            RDM.ObjectType = (int)ObjectType.ObjectStream;
            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.DeleteFromStream), objectId, objectType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<SharedVia> FindViaWhatItWasShared(int objectId, int objectType, int userId)
        {
            var result = new List<SharedVia>();
            var streams = ObjectStreamDAL.GetAllForObjectAndUser(objectId, objectType, userId);
            try
            {
                foreach (var item in streams)
                {
                    var group = ObjectStreamGroupMapperDAL.GetByObjectStream(item.Id);
                    if (group != null)
                    {
                        if (group.LearningGroup != null)
                            result.Add(new SharedVia
                            {
                                Id = group.LearningGroupId.Value,
                                Title = group.LearningGroup.Title,
                                SharedType = UT.SL.Model.Enumeration.SharedType.FromLearningGroup
                            });
                        if (group.SocialGroup != null)
                            result.Add(new SharedVia
                            {
                                Id = group.SocialGroupId.Value,
                                Title = group.SocialGroup.Title,
                                SharedType = UT.SL.Model.Enumeration.SharedType.FromSocialGroup
                            });
                    }
                    else
                    {
                        //if (item.Course != null)
                        //    result.Add(new SharedVia
                        //       {
                        //           Id = item.CourseId.Value,
                        //           Title = item.Course.Title,
                        //           SharedType = UT.SL.Model.Enumeration.SharedType.FromCourse
                        //       });
                    }
                }
            }
            catch
            {
                return result;
            }
            return result;
        }

    }
}
