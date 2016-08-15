using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Web;
using UT.SL.Model.Enumeration;
using UT.SL.BLL;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{

    [Authorize()]
    public class App_UserInfoController : BaseController
    {
        public ActionResult EditUserInfo(Guid Id)
        {
            var formObject = App_UserInfoDAL.Get(Id);
            if (formObject == null)
            {
                formObject = new App_UserInfo { Id = 0, App_User = new App_User { GuidId = Id } };
            }
            var listItems = new SelectListItems
             {
                 Items = TopicDAL.GetAll().OrderBy(x => x.Title).Select(x => new SelectListItem
                 {
                     Value = x.Id.ToString(),
                     Text = x.Title
                 })
             };
            if (formObject.App_User != null && formObject.App_User.UserTopicMappers != null)
            {
                listItems.SelectedIds = formObject.App_User.UserTopicMappers.Select(x => x.TopicId.ToString()).ToArray();
            }
            var model = new FormModel<App_UserInfo, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = listItems
            };
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult EditUserInfo(FormModel<App_UserInfo, SelectListItems> model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("FormObject.App_User.UserName");
            ModelState.Remove("FormObject.App_User.Password");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<App_UserInfo>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserInfo),
                               new Func<App_UserInfo, BatchProcessResultModel, DALReturnModel<App_UserInfo>>(App_UserInfoDAL.Update), model.FormObject, bpr));
                    bpr = drm.BPR;
                    var userId = drm.ReturnObject.App_User.Id;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                      new Func<App_User, DALReturnModel<App_User>>(App_UserDAL.DeleteUserTopics), new App_User { Id = userId });
                        if (model.ExtraKnownData != null && model.ExtraKnownData.SelectedIds != null)
                        {
                            foreach (var item in model.ExtraKnownData.SelectedIds)
                            {
                                var tempTopicMapper = new UserTopicMapper
                                {
                                    UserId = userId,
                                    TopicId = Int32.Parse(item)
                                };
                                var drm2 = (DALReturnModel<UserTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.UserTopicMapper),
                                      new Func<UserTopicMapper, BatchProcessResultModel, DALReturnModel<UserTopicMapper>>(App_UserDAL.AddUserTopic), tempTopicMapper, bpr);
                                bpr = drm2.BPR;
                            }
                        }
                        bpr.SuccessClientScript = "$('#searchForm').submit();";
                    }
                }
                catch (System.Exception)
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }
            return PartialView("ProcessResult", bpr);
        }

    }
}
