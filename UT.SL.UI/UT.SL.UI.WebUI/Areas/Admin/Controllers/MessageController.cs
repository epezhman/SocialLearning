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
using System.Web;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class MessageController : BaseController
    {

        public ActionResult Send()
        {
            var model = new Message();
            return PartialView(model);
        }

        public ActionResult SendInThread(string id)
        {
            var model = new Message();
            ViewBag.Recv = id;
            return PartialView(model);
        }

        public ActionResult SaveFile(HttpPostedFileBase messageFile)
        {
            int fileId = 0;
            try
            {
                if (messageFile != null)
                {
                    var newFile = new Message
                    {
                        AutherId = CurrentUser.Id,
                        GuidId = Guid.NewGuid(),
                        FileMime = messageFile.ContentType,
                        FileSize = messageFile.ContentLength,
                        FileTitle = messageFile.FileName,
                    };

                    byte[] tempFile = null;
                    tempFile = new byte[messageFile.ContentLength];
                    messageFile.InputStream.Read(tempFile, 0, messageFile.ContentLength);
                    newFile.FileContent = tempFile;

                    var bpr = new BatchProcessResultModel();
                    var drm = (DALReturnModel<Message>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                   new Func<Message, BatchProcessResultModel, DALReturnModel<Message>>(MessageDAL.Add), newFile, bpr);
                    bpr = drm.BPR;
                    fileId = drm.ReturnObject.Id;
                    if (bpr.Failed > 0)
                    {
                        return Content("0");
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(fileId.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult RemoveFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                       new Func<Message, DALReturnModel<Message>>(MessageDAL.Delete), new Message { Id = id });
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Message message, string receivers, int? sendInThread)
        {
            int messageId = 0;
            var bpr = new BatchProcessResultModel();
            var recvs = new List<App_User>();
            if (string.IsNullOrEmpty(receivers))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ChooseReceivers, true, true);
                return PartialView("ProcessResult", bpr);
            }
            else
            {
                var rcvTokens = receivers.Split(new char[] { ',', ' ' });
                foreach (var item in rcvTokens)
                {
                    var user = App_UserDAL.Get(item);
                    if (user != null && !recvs.Any(x => x.Id == user.Id) && user.Id != CurrentUser.Id)
                        recvs.Add(user);
                }
                if (!recvs.Any())
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.ChooseReceivers, true, true);
                    return PartialView("ProcessResult", bpr);
                }
            }
            var threads = new List<MessageThread>();
            foreach (var item in recvs)
            {
                var thread = MessageThreadDAL.Get(CurrentUser.Id, item.Id);
                if (thread.Any())
                    threads.AddRange(thread);

                if (!threads.Any(x => x.OwnerUserId == CurrentUser.Id && x.AssociatedUserId == item.Id))
                {
                    var newThread = new MessageThread
                    {
                        MessageCount = 0,
                        LastUpdate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        HasNotReaden = false,
                        HasNotSeen = false
                    };
                    var newThread1 = newThread;
                    newThread1.OwnerUserId = CurrentUser.Id;
                    newThread1.AssociatedUserId = item.Id;
                    newThread1.GuidId = Guid.NewGuid();
                    var drm = (DALReturnModel<MessageThread>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                                  new Func<MessageThread, BatchProcessResultModel, DALReturnModel<MessageThread>>(MessageThreadDAL.Add), newThread1, bpr);
                    newThread1 = drm.ReturnObject;
                    threads.Add(newThread1);
                }
                if (!threads.Any(x => x.OwnerUserId == item.Id && x.AssociatedUserId == CurrentUser.Id))
                {
                    var newThread = new MessageThread
                    {
                        MessageCount = 0,
                        LastUpdate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        HasNotReaden = false,
                        HasNotSeen = false
                    };
                    var newThread2 = newThread;
                    newThread2.OwnerUserId = item.Id;
                    newThread2.AssociatedUserId = CurrentUser.Id;
                    newThread2.GuidId = Guid.NewGuid();
                    var drm = (DALReturnModel<MessageThread>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                                  new Func<MessageThread, BatchProcessResultModel, DALReturnModel<MessageThread>>(MessageThreadDAL.Add), newThread2, bpr);
                    newThread2 = drm.ReturnObject;
                    threads.Add(newThread2);
                }
            }
            foreach (var item in recvs)
            {
                var contact = MessageContactDAL.GetContacts(CurrentUser.Id, item.Id);
                if (!contact.Any(x => x.OwnerUserId == CurrentUser.Id && x.ContactUserId == item.Id))
                {
                    var newContact = new MessageContact
                    {
                        OwnerUserId = CurrentUser.Id,
                        ContactUserId = item.Id,
                        CreateDate = DateTime.Now
                    };
                    var drm = ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageContact),
                                  new Func<MessageContact, BatchProcessResultModel, DALReturnModel<MessageContact>>(MessageContactDAL.Add), newContact, bpr);
                }
                if (!contact.Any(x => x.OwnerUserId == item.Id && x.ContactUserId == CurrentUser.Id))
                {
                    var newContact = new MessageContact
                    {
                        OwnerUserId = item.Id,
                        ContactUserId = CurrentUser.Id,
                        CreateDate = DateTime.Now
                    };
                    var drm = ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageContact),
                                 new Func<MessageContact, BatchProcessResultModel, DALReturnModel<MessageContact>>(MessageContactDAL.Add), newContact, bpr);
                }
            }
            var msg = new Message();
            var isThereFile = false;
            if (message.Id > 0)
            {
                msg = MessageDAL.Get(message.Id);
                if (msg != null)
                {
                    messageId = msg.Id;
                    if (msg.FileContent != null)
                        isThereFile = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(message.Body.StringNormalizer()))
                    {
                        bpr.AddError(UT.SL.Model.Resource.App_Errors.YouForgotToWriteTheMessage, true, true);
                        return PartialView("ProcessResult", bpr);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(message.Body.StringNormalizer()))
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.YouForgotToWriteTheMessage, true, true);
                    return PartialView("ProcessResult", bpr);
                }
            }
            Guid? threadGuid = null;
            Guid? messageGuid = null;
            foreach (var item in threads)
            {
                var dupMesg = new Message();
                dupMesg.AutherId = CurrentUser.Id;
                dupMesg.GuidId = Guid.NewGuid();
                dupMesg.Body = message.Body.StringNormalizer();
                dupMesg.CreateDate = DateTime.Now;
                dupMesg.ThreadId = item.Id;
                dupMesg.BelongToUserId = item.OwnerUserId;
                if (dupMesg.AutherId == item.OwnerUserId)
                {
                    dupMesg.ReceiverId = item.AssociatedUserId;
                    dupMesg.Seen = true;
                    dupMesg.SnippestSeen = true;
                    dupMesg.SeenDate = DateTime.Now;
                }
                else
                {
                    dupMesg.ReceiverId = item.OwnerUserId;
                    dupMesg.SnippestSeen = false;
                    dupMesg.Seen = false;
                }
                if (isThereFile)
                {
                    dupMesg.FileContent = msg.FileContent;
                    dupMesg.FileMime = msg.FileMime;
                    dupMesg.FileSize = msg.FileSize;
                    dupMesg.FileTitle = msg.FileTitle;
                }
                var drm = (DALReturnModel<Message>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                              new Func<Message, BatchProcessResultModel, DALReturnModel<Message>>(MessageDAL.Add), dupMesg, bpr);
                if (drm.ReturnObject.Id > 0)
                {
                    if (item.OwnerUserId != CurrentUser.Id)
                    {
                        item.HasNotReaden = true;
                        item.HasNotSeen = true;
                    }
                    item.LastUpdate = DateTime.Now;
                    item.MessageCount = item.MessageCount + 1;
                    if (dupMesg.Body.Length >= 33)
                        item.Snippest = dupMesg.Body.Substring(0, 32) + "...";
                    else item.Snippest = dupMesg.Body;
                    var drm2 = (DALReturnModel<MessageThread>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.MessageThread),
                             new Func<MessageThread, BatchProcessResultModel, DALReturnModel<MessageThread>>(MessageThreadDAL.Update), item, bpr);
                    if (sendInThread.HasValue)
                    {
                        if (item.OwnerUserId == CurrentUser.Id)
                        {
                            threadGuid = item.GuidId;
                            messageGuid = drm.ReturnObject.GuidId;
                        }
                    }
                }
            }
            if (messageId > 0)
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                  new Func<Message, DALReturnModel<Message>>(MessageDAL.Delete), new Message { Id = messageId });
            }
            bpr = new BatchProcessResultModel();
            bpr.AddSuccess(UT.SL.Model.Resource.App_Common.MessageSent, true, true);
            bpr.SuccessClientScript = "updateThreads('" + Url.Action("GetMessages", "MessageThread", new { Area = "Admin" }) + "');";

            if (sendInThread.HasValue)
            {
                bpr.FailedClientScript = "updateAuthorMessage('" + Url.Action("GetLastAuthorMessage", "MessageThread", new { Area = "Admin", id = messageGuid }) + "'); updateThreadView('" + Url.Action("ThreadView", "MessageThread", new { Area = "Admin", id = threadGuid }) + "', '" + threadGuid + "')";
                bpr.AddError("justForJS", true, true);
            }
            return PartialView("ProcessResult", bpr);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Message),
                                  new Func<Message, DALReturnModel<Message>>(MessageDAL.Delete), new Message { GuidId = id });
            return Content("OK");
        }

    }
}
