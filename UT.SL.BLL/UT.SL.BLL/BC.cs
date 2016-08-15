//*
// * ****************************************************************
// * Filename:        ResourceFinder.cs 
// * version:         
// * Author's name:   Fatemeh orooji, Pezhman Nasirifard, Elearning lab 
// * Creation date:   
// * Purpose:         provide item's to show in user home page with algorithm's
// * ****************************************************************
// */
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UT.SL.DAL;
//using UT.SL.Model;

//namespace UT.SL.BLL
//{

//    /// <summary>
//    /// provide needed data founded by algorithms to show in user home page
//    /// </summary>
//    public class ResourceFinder
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="Id"></param>
//        /// <param name="page"></param>
//        /// <param name="date"></param>
//        /// <param name="userId"></param>
//        /// <param name="resourceSelections"></param>
//        /// <returns></returns>
//        public static List<ObjectViewModelList> GetCourseResources(int Id, int? page, DateTime? date, int userId, string resourceSelections)
//        {
//            var resourseTokens = new string[] { };
//            if (!string.IsNullOrEmpty(resourceSelections))
//            {
//                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
//                if (resourceSelections == "undefined")
//                    resourseTokens = new string[] { };
//            }
//            var filters = new FilterObjectModel();
//            if (resourseTokens.Any(x => x == "1"))
//            {
//                filters.IsResource = true;
//            }
//            if (resourseTokens.Any(x => x == "2"))
//            {
//                var shared = LearningGroupDAL.GetAllByUserId(userId, Id);
//                foreach (var item in shared)
//                {
//                    filters.LearningGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("2_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var lGroup = LearningGroupDAL.Get(thisGId);
//                            if (lGroup != null)
//                            {
//                                filters.LearningGroupIds.Add(lGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "3"))
//            {
//                var shared = SocialGroupDAL.GetAllByUserId(userId);
//                foreach (var item in shared)
//                {
//                    filters.SocialGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("3_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var sGroup = SocialGroupDAL.Get(thisGId);
//                            if (sGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(sGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "4"))
//            {
//                filters.IsAssigment = true;
//            }

//            if (resourseTokens.Any(x => x == "6"))
//            {
//                filters.IsRecommended = true;
//            }
//            if (resourseTokens.Any(x => x == "7"))
//            {
//                filters.IsHot = true;
//            }

//            var tempObjects = ObjectStreamDAL.GetAllBackbone(Id, userId, page, date, filters);

//            if (filters.IsRecommended || filters.IsHot)
//                tempObjects = ObjectStreamDAL.GetCreditBackbone(Id, userId, page, date, filters);

//            var result = new List<ObjectViewModelList>();
//            var tempResult = new List<ObjectViewModel>();
//            if (tempObjects.Any())
//                foreach (var item in tempObjects)
//                {
//                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
//                    if (tObject != null && tObject.Id > 0)
//                    {
//                        tempResult.Add(tObject);
//                        ObjectStreamDAL.UpdateReadFlag(item.ObjectId, item.ObjectType, userId);
//                    }
//                }
//            var tempObjs = new List<ObjectViewModel>();
//            foreach (var item in tempResult)
//            {
//                if (!item.IsWide)
//                {
//                    tempObjs.Add(item);
//                }
//                else
//                {
//                    if (tempObjs.Any())
//                    {
//                        result.Add(new ObjectViewModelList
//                        {
//                            ObjectViewModels = tempObjs,
//                            IsWide = false
//                        });
//                    }
//                    tempObjs = new List<ObjectViewModel>();
//                    tempObjs.Add(item);
//                    result.Add(new ObjectViewModelList
//                    {
//                        ObjectViewModels = tempObjs,
//                        IsWide = true
//                    });
//                    tempObjs = new List<ObjectViewModel>();
//                }
//            }
//            if (tempObjs.Any())
//            {
//                result.Add(new ObjectViewModelList
//                {
//                    ObjectViewModels = tempObjs,
//                    IsWide = false
//                });
//            }
//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="Id"></param>
//        /// <param name="date"></param>
//        /// <param name="userId"></param>
//        /// <param name="resourceSelections"></param>
//        /// <returns></returns>
//        public static List<ObjectViewModelList> GetNewCourseResources(int Id, DateTime date, int userId, string resourceSelections)
//        {
//            var resourseTokens = new string[] { };
//            if (!string.IsNullOrEmpty(resourceSelections))
//            {
//                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
//                if (resourceSelections == "undefined")
//                    resourseTokens = new string[] { };
//            }

//            var filters = new FilterObjectModel();
//            if (resourseTokens.Any(x => x == "1"))
//            {
//                filters.IsResource = true;
//            }
//            if (resourseTokens.Any(x => x == "2"))
//            {
//                var shared = LearningGroupDAL.GetAllByUserId(userId, Id);
//                foreach (var item in shared)
//                {
//                    filters.LearningGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("2_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var lGroup = LearningGroupDAL.Get(thisGId);
//                            if (lGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(lGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "3"))
//            {
//                var shared = SocialGroupDAL.GetAllByUserId(userId);
//                foreach (var item in shared)
//                {
//                    filters.SocialGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("3_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var sGroup = SocialGroupDAL.Get(thisGId);
//                            if (sGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(sGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "4"))
//            {
//                filters.IsAssigment = true;
//            }
//            var tempObjects = ObjectStreamDAL.GetNewBackbone(Id, userId, date, filters);
//            var result = new List<ObjectViewModelList>();
//            var tempResult = new List<ObjectViewModel>();
//            if (tempObjects.Any())
//                foreach (var item in tempObjects)
//                {
//                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
//                    if (tObject != null && tObject.Id > 0 && tObject.CreateUser.Id != userId)
//                    {
//                        tempResult.Add(tObject);
//                        ObjectStreamDAL.UpdateReadFlag(item.ObjectId, item.ObjectType, userId);
//                    }
//                }
//            var tempObjs = new List<ObjectViewModel>();
//            foreach (var item in tempResult)
//            {
//                if (!item.IsWide)
//                {
//                    tempObjs.Add(item);
//                }
//                else
//                {
//                    if (tempObjs.Any())
//                    {
//                        result.Add(new ObjectViewModelList
//                        {
//                            ObjectViewModels = tempObjs,
//                            IsWide = false
//                        });
//                    }
//                    tempObjs = new List<ObjectViewModel>();
//                    tempObjs.Add(item);
//                    result.Add(new ObjectViewModelList
//                    {
//                        ObjectViewModels = tempObjs,
//                        IsWide = true
//                    });
//                    tempObjs = new List<ObjectViewModel>();
//                }
//            }
//            if (tempObjs.Any())
//            {
//                result.Add(new ObjectViewModelList
//                {
//                    ObjectViewModels = tempObjs,
//                    IsWide = false
//                });
//            }
//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="page"></param>
//        /// <param name="date"></param>
//        /// <param name="userId"></param>
//        /// <param name="resourceSelections"></param>
//        /// <returns></returns>
//        public static List<ObjectViewModelList> GetUserHome(int? page, DateTime? date, int userId, string resourceSelections)
//        {
//            var resourseTokens = new string[] { };
//            if (!string.IsNullOrEmpty(resourceSelections))
//            {
//                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
//                if (resourceSelections == "undefined")
//                    resourseTokens = new string[] { };
//            }
//            var filters = new FilterObjectModel();
//            if (resourseTokens.Any(x => x == "1"))
//            {
//                filters.IsResource = true;
//            }
//            if (resourseTokens.Any(x => x == "2"))
//            {
//                var shared = LearningGroupDAL.GetAllByUserId(userId);
//                foreach (var item in shared)
//                {
//                    filters.LearningGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("2_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var lGroup = LearningGroupDAL.Get(thisGId);
//                            if (lGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(lGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "3"))
//            {
//                var shared = SocialGroupDAL.GetAllByUserId(userId);
//                foreach (var item in shared)
//                {
//                    filters.SocialGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("3_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var sGroup = SocialGroupDAL.Get(thisGId);
//                            if (sGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(sGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "4"))
//            {
//                filters.IsAssigment = true;
//            }
//            var tempObjects = ObjectStreamDAL.GetAllBackbone(null, userId, page, date, filters);
//            var result = new List<ObjectViewModelList>();
//            var tempResult = new List<ObjectViewModel>();
//            if (tempObjects.Any())
//                foreach (var item in tempObjects)
//                {
//                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
//                    if (tObject != null && tObject.Id > 0)
//                    {
//                        tempResult.Add(tObject);
//                        ObjectStreamDAL.UpdateReadFlag(item.ObjectId, item.ObjectType, userId);
//                    }
//                }
//            var tempObjs = new List<ObjectViewModel>();
//            foreach (var item in tempResult)
//            {
//                if (!item.IsWide)
//                {
//                    tempObjs.Add(item);
//                }
//                else
//                {
//                    if (tempObjs.Any())
//                    {
//                        result.Add(new ObjectViewModelList
//                        {
//                            ObjectViewModels = tempObjs,
//                            IsWide = false
//                        });
//                    }
//                    tempObjs = new List<ObjectViewModel>();
//                    tempObjs.Add(item);
//                    result.Add(new ObjectViewModelList
//                    {
//                        ObjectViewModels = tempObjs,
//                        IsWide = true
//                    });
//                    tempObjs = new List<ObjectViewModel>();
//                }
//            }
//            if (tempObjs.Any())
//            {
//                result.Add(new ObjectViewModelList
//                {
//                    ObjectViewModels = tempObjs,
//                    IsWide = false
//                });
//            }
//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="date"></param>
//        /// <param name="userId"></param>
//        /// <param name="resourceSelections"></param>
//        /// <returns></returns>
//        public static List<ObjectViewModelList> GetNewUserHome(DateTime date, int userId, string resourceSelections)
//        {
//            var resourseTokens = new string[] { };
//            if (!string.IsNullOrEmpty(resourceSelections))
//            {
//                resourseTokens = resourceSelections.Split(new char[] { ' ', ',' });
//                if (resourceSelections == "undefined")
//                    resourseTokens = new string[] { };
//            }

//            var filters = new FilterObjectModel();
//            if (resourseTokens.Any(x => x == "1"))
//            {
//                filters.IsResource = true;
//            }
//            if (resourseTokens.Any(x => x == "2"))
//            {
//                var shared = LearningGroupDAL.GetAllByUserId(userId);
//                foreach (var item in shared)
//                {
//                    filters.LearningGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("2_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("2_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var lGroup = LearningGroupDAL.Get(thisGId);
//                            if (lGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(lGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "3"))
//            {
//                var shared = SocialGroupDAL.GetAllByUserId(userId);
//                foreach (var item in shared)
//                {
//                    filters.SocialGroupIds.Add(item.Id);
//                }
//            }
//            if (resourseTokens.Any(x => x.StartsWith("3_")))
//            {
//                foreach (var item in resourseTokens.Where(x => x.StartsWith("3_")))
//                {
//                    var tokens = item.Split('_');
//                    if (tokens.Count() == 2)
//                    {
//                        var thisGId = 0;
//                        if (Int32.TryParse(tokens.Last(), out thisGId))
//                        {
//                            var sGroup = SocialGroupDAL.Get(thisGId);
//                            if (sGroup != null)
//                            {
//                                filters.SocialGroupIds.Add(sGroup.Id);
//                            }
//                        }
//                    }
//                }
//            }
//            if (resourseTokens.Any(x => x == "4"))
//            {
//                filters.IsAssigment = true;
//            }
//            var tempObjects = ObjectStreamDAL.GetNewBackbone(null, userId, date, filters);
//            var result = new List<ObjectViewModelList>();
//            var tempResult = new List<ObjectViewModel>();
//            if (tempObjects.Any())
//                foreach (var item in tempObjects)
//                {
//                    var tObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
//                    if (tObject != null && tObject.Id > 0 && tObject.CreateUser.Id != userId)
//                    {
//                        tempResult.Add(tObject);
//                        ObjectStreamDAL.UpdateReadFlag(item.ObjectId, item.ObjectType, userId);
//                    }
//                }

//            var tempObjs = new List<ObjectViewModel>();
//            foreach (var item in tempResult)
//            {
//                if (!item.IsWide)
//                {
//                    tempObjs.Add(item);
//                }
//                else
//                {
//                    if (tempObjs.Any())
//                    {
//                        result.Add(new ObjectViewModelList
//                        {
//                            ObjectViewModels = tempObjs,
//                            IsWide = false
//                        });
//                    }
//                    tempObjs = new List<ObjectViewModel>();
//                    tempObjs.Add(item);
//                    result.Add(new ObjectViewModelList
//                    {
//                        ObjectViewModels = tempObjs,
//                        IsWide = true
//                    });
//                    tempObjs = new List<ObjectViewModel>();
//                }
//            }
//            if (tempObjs.Any())
//            {
//                result.Add(new ObjectViewModelList
//                {
//                    ObjectViewModels = tempObjs,
//                    IsWide = false
//                });
//            }
//            return result;
//        }

//    }
//}



//if (filterContext.HttpContext.ActionParameters.Any(x => x.Key.ToLower() == "objectid") && filterContext.ActionParameters.Any(x => x.Key.ToLower() == "objecttype" || x.Key.ToLower() == "type"))
//{
//    objectIdVal = filterContext.ActionParameters.First(x => x.Key.ToLower() == "objectid").Value.ToString();
//    var objectType = filterContext.ActionParameters.FirstOrDefault(x => x.Key.ToLower() == "objecttype");
//    objectTypeVal = string.Empty;
//    if (!string.IsNullOrEmpty(objectType.Key))
//    {
//        objectTypeVal = objectType.Value.ToString();
//    }
//    if (string.IsNullOrEmpty(objectTypeVal))
//    {
//        objectType = filterContext.ActionParameters.FirstOrDefault(x => x.Key.ToLower() == "type");
//        if (!string.IsNullOrEmpty(objectType.Key))
//        {
//            objectTypeVal = objectType.Value.ToString();
//        }
//    }
//}
//if (string.IsNullOrEmpty(objectIdVal) || string.IsNullOrEmpty(objectTypeVal))
//{
//    foreach (var item in filterContext.ActionParameters)
//    {
//        var paramType = item.Value.GetType().Name;
//        if (paramType.ToLower().StartsWith("formmodel"))
//        {
//            var paramProps = item.Value.GetType().GetProperties();
//            if (paramProps.Any(x => x.PropertyType.Name.ToLower().StartsWith("objectmodel")))
//            {
//                var objectmodel = paramProps.FirstOrDefault(x => x.PropertyType.Name.ToLower().StartsWith("objectmodel"));
//                var objectmodelValue = objectmodel.GetValue(item.Value, null);
//                if (objectmodel != null)
//                {
//                    var objectModelProperties = objectmodel.PropertyType.GetProperties();
//                    if (objectModelProperties.Any(x => x.Name.ToLower().StartsWith("type")))
//                    {
//                        var objectModelType = objectModelProperties.FirstOrDefault(x => x.Name.ToLower().StartsWith("type"));
//                        if (objectModelType != null)
//                        {
//                            var objectModelTypeValue = objectModelType.GetValue(objectmodelValue, null);
//                            if (objectModelTypeValue != null)
//                            {
//                                objectTypeVal = objectModelTypeValue.ToString();
//                            }
//                        }
//                    }
//                    if (objectModelProperties.Any(x => x.Name.ToLower().StartsWith("objectid")))
//                    {
//                        var objectModelType = objectModelProperties.FirstOrDefault(x => x.Name.ToLower().StartsWith("objectid"));
//                        if (objectModelType != null)
//                        {
//                            var objectModelTypeValue = objectModelType.GetValue(objectmodelValue, null);
//                            if (objectModelTypeValue != null)
//                            {
//                                objectIdVal = objectModelTypeValue.ToString();
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}