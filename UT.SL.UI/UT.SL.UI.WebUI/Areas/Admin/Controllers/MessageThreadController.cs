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
    public class MessageThreadController : BaseController
    {

        public ActionResult Inbox(int type)
        {
            ViewBag.TopOrBottom = type;
            return PartialView(CurrentUser);
        }

        public ActionResult InboxButton(int type)
        {
            ViewBag.TopOrBottom = type;
            return PartialView(CurrentUser);
        }

        public ActionResult GetNotSeenCount()
        {
            ViewBag.Count = MessageThreadDAL.GetUnSeenThreadCount(CurrentUser.Id);
            return PartialView();
        }

        public ActionResult GetMessages()
        {
            var threads = MessageThreadDAL.GetAll(CurrentUser.Id);
            return PartialView(threads.OrderByDescending(x => x.LastUpdate).ToList());
        }

        public ActionResult Read(Guid id)
        {
            var thread = MessageThreadDAL.Get(id);
            foreach (var item in thread.Messages.Where(x => !x.Seen.Value || !x.Seen.HasValue))
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                  new Func<Message, DALReturnModel<Message>>(MessageDAL.UpdateSeen), item);
            }
            var drm = (DALReturnModel<MessageThread>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                                 new Func<MessageThread, DALReturnModel<MessageThread>>(MessageThreadDAL.UpdateSeen), thread);
            thread = drm.ReturnObject;
            return PartialView(thread);
        }

        public ActionResult GetLastAuthorMessage(Guid id)
        {
            var message = MessageDAL.Get(id);
            return PartialView("AuthorSection", message);
        }

        public ActionResult GetLastReceiverMessage(Guid id)
        {
            var thread = MessageThreadDAL.Get(id);
            var messages = thread.Messages.Where(x => !x.Seen.Value && x.AutherId != CurrentUser.Id).ToList();
            foreach (var item in messages)
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                  new Func<Message, DALReturnModel<Message>>(MessageDAL.UpdateSeen), item);
            }
            if (messages.Any())
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                                     new Func<MessageThread, DALReturnModel<MessageThread>>(MessageThreadDAL.UpdateSeen), thread);
            return PartialView("ReceiverSections", messages);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                                  new Func<MessageThread, DALReturnModel<MessageThread>>(MessageThreadDAL.Delete), new MessageThread { GuidId = id });
            return Content("OK");
        }

        public ActionResult GetMoreMessages(Guid id, int page, int cnt, Guid lastId)
        {
            var thread = MessageThreadDAL.Get(id);
            var lastMessage = thread.Messages.SingleOrDefault(x => x.GuidId == lastId);
            if (lastMessage != null)
                lastMessage = thread.Messages.Last();
            var skipper = cnt - 3 - page * 7;
            var messages = thread.Messages.Where(x => x.CreateDate < lastMessage.CreateDate).Skip(cnt - 3 - page * 7).Take(7).ToList();
            if (messages.Any() && skipper > -7)
                return PartialView(messages);
            else
                return Content("OK");
        }

        public ActionResult ThreadView(Guid id)
        {
            var thread = MessageThreadDAL.Get(id);
            foreach (var item in thread.Messages.Where(x => !x.Seen.Value || !x.Seen.HasValue))
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                  new Func<Message, DALReturnModel<Message>>(MessageDAL.UpdateSeen), item);
            }
            var drm = (DALReturnModel<MessageThread>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                                 new Func<MessageThread, DALReturnModel<MessageThread>>(MessageThreadDAL.UpdateSeen), thread);
            thread = drm.ReturnObject;
            return PartialView(thread);
        }

    }
}
