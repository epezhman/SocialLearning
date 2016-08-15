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


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{

    [Authorize()]
    public class App_UserEnrolementController : BaseController
    {
        public ActionResult RemoveMember(int Id)
        {
            var CId = 0;
            var membershipType = 0e;
            try
            {
                var thisOne = App_UserEnrolementDAL.Get(Id);
                CId = thisOne.CourseId;
                membershipType = thisOne.Type;
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserEnrolement),
                                      new Func<App_UserEnrolement, DALReturnModel<App_UserEnrolement>>(App_UserEnrolementDAL.Delete), new App_UserEnrolement { Id = Id });
                ObjectStreamManager.ObjectResourceToStreamForRemoveCourseMemeber(CId, thisOne.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));

            }
            catch 
            {
            }
            return PartialView("GetMembersLargePic", App_UserEnrolementDAL.GetAllByCourse(CId).Where(x => x.Type == membershipType).ToList());
        }

        [HttpPost]
        public ActionResult RemoveMemberByPanel(int gId, Guid userId)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserEnrolement),
                                      new Func<int, Guid, DALReturnModel<App_UserEnrolement>>(App_UserEnrolementDAL.DeleteMember), gId, userId);

                ObjectStreamManager.ObjectResourceToStreamForRemoveCourseMemeber(gId, App_UserDAL.Get(userId).Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));

            }
            catch (System.Exception)
            {
            }
            return Content("OK");
        }

        public ActionResult AddMember(int Id, int memberType)
        {
            var model = new AddCourseMembershipModel { CourseId = Id, MembershipType = memberType };
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult AddMember(AddCourseMembershipModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.Member))
                    {
                        if (model.Member.StringNormalizer().Split('-').Count() == 2 && !string.IsNullOrEmpty(model.Member.StringNormalizer().Split('-').Last()))
                        {
                            model.Member = model.Member.StringNormalizer().Split('-').Last();
                            var drm = ((DALReturnModel<App_UserEnrolement>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserEnrolement),
                               new Func<AddCourseMembershipModel, BatchProcessResultModel, DALReturnModel<App_UserEnrolement>>(App_UserEnrolementDAL.AddCourseMember), model, bpr));
                            bpr = drm.BPR;
                            var member = drm.ReturnObject;
                            if (member != null)
                                ObjectStreamManager.ObjectResourceToStreamForNewCourseMemeber(member.CourseId, member.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));

                        }
                    }
                }
                catch (System.Exception)
                {
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }
            var members = App_UserEnrolementDAL.GetAllByCourse(model.CourseId).Where(x => x.Type == model.MembershipType).ToList();
            return PartialView("GetMembersLargePic", members);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult AddMemberPanel(AddCourseMembershipModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.Member))
                    {
                        if (model.Member.StringNormalizer().Split('-').Count() == 2 && !string.IsNullOrEmpty(model.Member.StringNormalizer().Split('-').Last()))
                        {
                            model.Member = model.Member.StringNormalizer().Split('-').Last();
                            var drm = ((DALReturnModel<App_UserEnrolement>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserEnrolement),
                                new Func<AddCourseMembershipModel, BatchProcessResultModel, DALReturnModel<App_UserEnrolement>>(App_UserEnrolementDAL.AddCourseMember), model, bpr));
                            bpr = drm.BPR;
                            var member = drm.ReturnObject;
                            if (member != null)
                                ObjectStreamManager.ObjectResourceToStreamForNewCourseMemeber(member.CourseId, member.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                        }
                    }
                }
                catch (System.Exception)
                {
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }
            return Content("OK");
        }


        public ActionResult GetMembersLargePic(int Id, int memberType)
        {
            var model = App_UserEnrolementDAL.GetAllByCourse(Id).Where(x => x.Type == memberType).ToList();
            return PartialView(model);
        }

        public ActionResult GetTopMembershipMembers(int Id, int memberType)
        {
            var model = App_UserEnrolementDAL.GetAllByCourse(Id).Where(x => x.Type == memberType).ToList();
            return PartialView(model);
        }
    }
}
