using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;
using System.Collections.Generic;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{

    [Authorize()]
    public class NotificationController : BaseController
    {

        public ActionResult NotificationBox(int type)
        {
            ViewBag.TopOrBottom = type;
            return PartialView(CurrentUser);
        }

        public ActionResult NotificationButton(int type)
        {
            ViewBag.TopOrBottom = type;
            return PartialView(CurrentUser);
        }

        public ActionResult GetNotSeenCount()
        {
            ViewBag.Count = NotificationDAL.GetUnSeenCount(CurrentUser.Id);
            return PartialView();
        }

        public ActionResult GetNotifications()
        {
            var notifs = NotificationDAL.GetTop100UserUnRead(CurrentUser.Id);
            foreach (var item in notifs)
            {
                var bpr = new BatchProcessResultModel();
                var drm = (DALReturnModel<Notification>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Notification),
                                           new Func<int, BatchProcessResultModel, DALReturnModel<Notification>>(NotificationDAL.UpdateSeenDate), item.Id, bpr);                   
           
            }
            return PartialView(notifs.GroupBy(x => x.Course).ToList());
        }

        public ActionResult NotificationView(Notification model)
        {
            return PartialView(NotificationManager.GetInfo(model));
        }

        public ActionResult NotificationViewDetailed(Notification model)
        {
            return PartialView(NotificationManager.GetInfo(model));
        }

        public ActionResult SeeAll()
        {
            return View();
        }

        public ActionResult GetMoreNotifications(int page)
        {
            var model = NotificationDAL.GetAllUser(CurrentUser.Id).GroupBy(x => x.CreateDate.Date).OrderByDescending(x => x.Key).Skip(page * 7).Take(7).ToList();
            if (model.Any())
                return PartialView(model);
            else
                return Content("OK");
        }

        public ActionResult UpdateReaden(int id)
        {
            var bpr = new BatchProcessResultModel();
            var drm = (DALReturnModel<Notification>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Notification),
                                       new Func<int, BatchProcessResultModel, DALReturnModel<Notification>>(NotificationDAL.UpdateReaden), id, bpr);                   
            return Content("OK");
        }

    }
}
