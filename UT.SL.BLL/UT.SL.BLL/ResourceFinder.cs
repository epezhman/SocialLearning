/*
 * ****************************************************************
 * Filename:        ResourceFinder.cs 
 * version:         
 * Author's name:   Fatemeh orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         provide item's to show in user home page with algorithm's
 * ****************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Model.Enumeration;

namespace UT.SL.BLL
{

    /// <summary>
    /// provide needed data founded by algorithms to show in user home page
    /// </summary>
    public class ResourceFinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="page"></param>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <param name="resourceSelections"></param>
        /// <param name="RDM"></param>
        /// <returns></returns>
        public static List<ObjectViewModelList> GetCourseResources(int Id, int? page, DateTime? date, int userId, string resourceSelections, RequestDetailModel RDM)
        {
            var resourseTokens = new string[] { };
            if (!string.IsNullOrEmpty(resourceSelections))
            {
                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
                if (resourceSelections == "undefined")
                    resourseTokens = new string[] { };
            }
            var filters = new FilterObjectModel();
            if (resourseTokens.Any(x => x == "1"))
            {
                filters.IsResource = true;
            }
            if (resourseTokens.Any(x => x == "2"))
            {
                var shared = LearningGroupDAL.GetAllByUserId(userId, Id);
                foreach (var item in shared)
                {
                    filters.LearningGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("2_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var lGroup = LearningGroupDAL.Get(thisGId);
                            if (lGroup != null)
                            {
                                filters.LearningGroupIds.Add(lGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "3"))
            {
                var shared = SocialGroupDAL.GetAllByUserId(userId);
                foreach (var item in shared)
                {
                    filters.SocialGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("3_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var sGroup = SocialGroupDAL.Get(thisGId);
                            if (sGroup != null)
                            {
                                filters.SocialGroupIds.Add(sGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "4"))
            {
                filters.IsActivity = true;
            }
            if (resourseTokens.Any(x => x == "8"))
            {
                filters.IsForums = true;
            }
            if (resourseTokens.Any(x => x == "9"))
            {
                filters.IsAssignments = true;
            }
            if (resourseTokens.Any(x => x == "6"))
            {
                filters.IsRecommended = true;
            }
            if (resourseTokens.Any(x => x == "7"))
            {
                filters.IsHot = true;
            }

            var tempObjects = ObjectStreamDAL.GetAllBackbone(Id, userId, page, date, filters);
            //IEnumerable<ObjectStream> tempObjects;
            //if (filters.IsRecommended || filters.IsHot)
            //    tempObjects = ObjectStreamDAL.GetCreditBackbone(Id, userId, page, date, filters);
            //else
            //    tempObjects = ObjectStreamDAL.GetAllBackbone(Id, userId, page, date, filters);
            var result = new List<ObjectViewModelList>();

            //در این تکه کد نوشته ام که برای تبهای پیشنهادی و پرطرفدار دو درس تستی همان لیست اولیه را بصورت خالی برگرداند بدون آنکه کوئری بزند و ریسورسها را بخواند
            if ((Id == 39 || Id == 41) &&  (filters.IsHot == true || filters.IsRecommended == true))
            {
                return result;
            }
            //تا اینجا




            var tempResult = new List<ObjectViewModel>();
            if (tempObjects.Any())
                foreach (var item in tempObjects)
                {
                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (tObject != null && tObject.Id > 0)
                    {
                        var res = ObjectStreamDAL.StreamIsReadenOrEdited(item.ObjectId, item.ObjectType, userId);
                        if (res == 1)
                        {
                            tObject.IsReaden = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateLastShownDate), item.ObjectId, item.ObjectType, userId);
                            if (drm.ReturnObject != null && drm.ReturnObject.Id > 0 && drm.ReturnObject.LastClcikedDate.HasValue)
                            {
                                tObject.ClickedDate = drm.ReturnObject.LastClcikedDate;
                            }
                        }
                        else if (res == 2)
                        {
                            tObject.IsEdited = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        else
                        {
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        tempResult.Add(tObject);
                        RDM.ObjectType = (int)ObjectType.ObjectStream;
                    }
                }
            var tempObjs = new List<ObjectViewModel>();
            foreach (var item in tempResult)
            {
                if (!item.IsWide)
                {
                    tempObjs.Add(item);
                }
                else
                {
                    if (tempObjs.Any())
                    {
                        result.Add(new ObjectViewModelList
                        {
                            ObjectViewModels = tempObjs,
                            IsWide = false
                        });
                    }
                    tempObjs = new List<ObjectViewModel>();
                    tempObjs.Add(item);
                    result.Add(new ObjectViewModelList
                    {
                        ObjectViewModels = tempObjs,
                        IsWide = true
                    });
                    tempObjs = new List<ObjectViewModel>();
                }
            }
            if (tempObjs.Any())
            {
                result.Add(new ObjectViewModelList
                {
                    ObjectViewModels = tempObjs,
                    IsWide = false
                });
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <param name="resourceSelections"></param>
        /// <param name="RDM"></param>
        /// <returns></returns>
        public static List<ObjectViewModelList> GetNewCourseResources(int Id, DateTime date, int userId, string resourceSelections, RequestDetailModel RDM)
        {
            var resourseTokens = new string[] { };
            if (!string.IsNullOrEmpty(resourceSelections))
            {
                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
                if (resourceSelections == "undefined")
                    resourseTokens = new string[] { };
            }

            var filters = new FilterObjectModel();
            if (resourseTokens.Any(x => x == "1"))
            {
                filters.IsResource = true;
            }
            if (resourseTokens.Any(x => x == "2"))
            {
                var shared = LearningGroupDAL.GetAllByUserId(userId, Id);
                foreach (var item in shared)
                {
                    filters.LearningGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("2_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var lGroup = LearningGroupDAL.Get(thisGId);
                            if (lGroup != null)
                            {
                                filters.SocialGroupIds.Add(lGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "3"))
            {
                var shared = SocialGroupDAL.GetAllByUserId(userId);
                foreach (var item in shared)
                {
                    filters.SocialGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("3_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var sGroup = SocialGroupDAL.Get(thisGId);
                            if (sGroup != null)
                            {
                                filters.SocialGroupIds.Add(sGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "4"))
            {
                filters.IsActivity = true;
            }
            if (resourseTokens.Any(x => x == "8"))
            {
                filters.IsForums = true;
            }
            if (resourseTokens.Any(x => x == "9"))
            {
                filters.IsAssignments = true;
            }
            var tempObjects = ObjectStreamDAL.GetNewBackbone(Id, userId, date, filters);
            var result = new List<ObjectViewModelList>();
            var tempResult = new List<ObjectViewModel>();
            if (tempObjects.Any())
                foreach (var item in tempObjects)
                {
                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (tObject != null && tObject.Id > 0 && tObject.CreateUser.Id != userId)
                    {
                        var res = ObjectStreamDAL.StreamIsReadenOrEdited(item.ObjectId, item.ObjectType, userId);
                        if (res == 1)
                        {
                            tObject.IsReaden = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateLastShownDate), item.ObjectId, item.ObjectType, userId);
                            if (drm.ReturnObject != null && drm.ReturnObject.Id > 0 && drm.ReturnObject.LastClcikedDate.HasValue)
                            {
                                tObject.ClickedDate = drm.ReturnObject.LastClcikedDate;
                            }
                        }
                        else if (res == 2)
                        {
                            tObject.IsEdited = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        else
                        {
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        tempResult.Add(tObject);
                        RDM.ObjectType = (int)ObjectType.ObjectStream;
                    }
                }
            var tempObjs = new List<ObjectViewModel>();
            foreach (var item in tempResult)
            {
                if (!item.IsWide)
                {
                    tempObjs.Add(item);
                }
                else
                {
                    if (tempObjs.Any())
                    {
                        result.Add(new ObjectViewModelList
                        {
                            ObjectViewModels = tempObjs,
                            IsWide = false
                        });
                    }
                    tempObjs = new List<ObjectViewModel>();
                    tempObjs.Add(item);
                    result.Add(new ObjectViewModelList
                    {
                        ObjectViewModels = tempObjs,
                        IsWide = true
                    });
                    tempObjs = new List<ObjectViewModel>();
                }
            }
            if (tempObjs.Any())
            {
                result.Add(new ObjectViewModelList
                {
                    ObjectViewModels = tempObjs,
                    IsWide = false
                });
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <param name="resourceSelections"></param>
        /// <param name="RDM"></param>
        /// <returns></returns>
        public static List<ObjectViewModelList> GetUserHome(int? page, DateTime? date, int userId, string resourceSelections, RequestDetailModel RDM)
        {
            var resourseTokens = new string[] { };
            if (!string.IsNullOrEmpty(resourceSelections))
            {
                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
                if (resourceSelections == "undefined")
                    resourseTokens = new string[] { };
            }
            var filters = new FilterObjectModel();
            if (resourseTokens.Any(x => x == "1"))
            {
                filters.IsResource = true;
            }
            if (resourseTokens.Any(x => x == "2"))
            {
                var shared = LearningGroupDAL.GetAllByUserId(userId);
                foreach (var item in shared)
                {
                    filters.LearningGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("2_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var lGroup = LearningGroupDAL.Get(thisGId);
                            if (lGroup != null)
                            {
                                filters.SocialGroupIds.Add(lGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "3"))
            {
                var shared = SocialGroupDAL.GetAllByUserId(userId);
                foreach (var item in shared)
                {
                    filters.SocialGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("3_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var sGroup = SocialGroupDAL.Get(thisGId);
                            if (sGroup != null)
                            {
                                filters.SocialGroupIds.Add(sGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "4"))
            {
                filters.IsActivity = true;
            }
            if (resourseTokens.Any(x => x == "8"))
            {
                filters.IsForums = true;
            }
            if (resourseTokens.Any(x => x == "9"))
            {
                filters.IsAssignments = true;
            }
            var tempObjects = ObjectStreamDAL.GetAllBackbone(null, userId, page, date, filters);
            var result = new List<ObjectViewModelList>();
            var tempResult = new List<ObjectViewModel>();
            if (tempObjects.Any())
                foreach (var item in tempObjects)
                {
                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (tObject != null && tObject.Id > 0)
                    {
                        var res = ObjectStreamDAL.StreamIsReadenOrEdited(item.ObjectId, item.ObjectType, userId);
                        if (res == 1)
                        {
                            tObject.IsReaden = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateLastShownDate), item.ObjectId, item.ObjectType, userId);
                            if (drm.ReturnObject != null && drm.ReturnObject.Id > 0 && drm.ReturnObject.LastClcikedDate.HasValue)
                            {
                                tObject.ClickedDate = drm.ReturnObject.LastClcikedDate;
                            }
                        }
                        else if (res == 2)
                        {
                            tObject.IsEdited = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        else
                        {
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        tempResult.Add(tObject);
                        RDM.ObjectType = (int)ObjectType.ObjectStream;
                    }
                }
            var tempObjs = new List<ObjectViewModel>();
            foreach (var item in tempResult)
            {
                if (!item.IsWide)
                {
                    tempObjs.Add(item);
                }
                else
                {
                    if (tempObjs.Any())
                    {
                        result.Add(new ObjectViewModelList
                        {
                            ObjectViewModels = tempObjs,
                            IsWide = false
                        });
                    }
                    tempObjs = new List<ObjectViewModel>();
                    tempObjs.Add(item);
                    result.Add(new ObjectViewModelList
                    {
                        ObjectViewModels = tempObjs,
                        IsWide = true
                    });
                    tempObjs = new List<ObjectViewModel>();
                }
            }
            if (tempObjs.Any())
            {
                result.Add(new ObjectViewModelList
                {
                    ObjectViewModels = tempObjs,
                    IsWide = false
                });
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <param name="resourceSelections"></param>
        /// <param name="RDM"></param>
        /// <returns></returns>
        public static List<ObjectViewModelList> GetNewUserHome(DateTime date, int userId, string resourceSelections, RequestDetailModel RDM)
        {
            var resourseTokens = new string[] { };
            if (!string.IsNullOrEmpty(resourceSelections))
            {
                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
                if (resourceSelections == "undefined")
                    resourseTokens = new string[] { };
            }

            var filters = new FilterObjectModel();
            if (resourseTokens.Any(x => x == "1"))
            {
                filters.IsResource = true;
            }
            if (resourseTokens.Any(x => x == "2"))
            {
                var shared = LearningGroupDAL.GetAllByUserId(userId);
                foreach (var item in shared)
                {
                    filters.LearningGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("2_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var lGroup = LearningGroupDAL.Get(thisGId);
                            if (lGroup != null)
                            {
                                filters.SocialGroupIds.Add(lGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "3"))
            {
                var shared = SocialGroupDAL.GetAllByUserId(userId);
                foreach (var item in shared)
                {
                    filters.SocialGroupIds.Add(item.Id);
                }
            }
            if (resourseTokens.Any(x => x.StartsWith("3_")))
            {
                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
                {
                    var tokens = item.Split('_');
                    if (tokens.Count() == 2)
                    {
                        var thisGId = 0;
                        if (Int32.TryParse(tokens.Last(), out thisGId))
                        {
                            var sGroup = SocialGroupDAL.Get(thisGId);
                            if (sGroup != null)
                            {
                                filters.SocialGroupIds.Add(sGroup.Id);
                            }
                        }
                    }
                }
            }
            if (resourseTokens.Any(x => x == "4"))
            {
                filters.IsActivity = true;
            }
            if (resourseTokens.Any(x => x == "8"))
            {
                filters.IsForums = true;
            }
            if (resourseTokens.Any(x => x == "9"))
            {
                filters.IsAssignments = true;
            }
            var tempObjects = ObjectStreamDAL.GetNewBackbone(null, userId, date, filters);
            var result = new List<ObjectViewModelList>();
            var tempResult = new List<ObjectViewModel>();
            if (tempObjects.Any())
                foreach (var item in tempObjects)
                {
                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (tObject != null && tObject.Id > 0 && tObject.CreateUser.Id != userId)
                    {
                        var res = ObjectStreamDAL.StreamIsReadenOrEdited(item.ObjectId, item.ObjectType, userId);
                        if (res == 1)
                        {
                            tObject.IsReaden = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateLastShownDate), item.ObjectId, item.ObjectType, userId);
                            if (drm.ReturnObject != null && drm.ReturnObject.Id > 0 && drm.ReturnObject.LastClcikedDate.HasValue)
                            {
                                tObject.ClickedDate = drm.ReturnObject.LastClcikedDate;
                            }
                        }
                        else if (res == 2)
                        {
                            tObject.IsEdited = true;
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        else
                        {
                            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(RDM, new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), item.ObjectId, item.ObjectType, userId);
                        }
                        tempResult.Add(tObject);
                        RDM.ObjectType = (int)ObjectType.ObjectStream;
                    }
                }

            var tempObjs = new List<ObjectViewModel>();
            foreach (var item in tempResult)
            {
                if (!item.IsWide)
                {
                    tempObjs.Add(item);
                }
                else
                {
                    if (tempObjs.Any())
                    {
                        result.Add(new ObjectViewModelList
                        {
                            ObjectViewModels = tempObjs,
                            IsWide = false
                        });
                    }
                    tempObjs = new List<ObjectViewModel>();
                    tempObjs.Add(item);
                    result.Add(new ObjectViewModelList
                    {
                        ObjectViewModels = tempObjs,
                        IsWide = true
                    });
                    tempObjs = new List<ObjectViewModel>();
                }
            }
            if (tempObjs.Any())
            {
                result.Add(new ObjectViewModelList
                {
                    ObjectViewModels = tempObjs,
                    IsWide = false
                });
            }
            return result;
        }

    }
}
