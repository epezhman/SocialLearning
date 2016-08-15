using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.BLL;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Model.Enumeration;
using UT.SL.UI.WebUI.Controllers;

namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize()]
    public class AssessController : BaseController
    {

        public void Assess(int assessValue, int CurrentUserId, RequestDetailModel RDM, int ObjectId = 0, int Type = 0, int assessType = 0)
        {
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);

            }
            var drm = (DALReturnModel<AssessParent>)ManageAction.ProxyCall(RDM,
                                      new Func<AssessParent, DALReturnModel<AssessParent>>(AssessParentDAL.AddNotExist), new AssessParent { ObjectId = ObjectId, ObjectType = Type });
            var model = drm.ReturnObject;
            var exitsAssess = AssessDAL.GetIfExist(model.Id, CurrentUserId, assessType).LastOrDefault();
            if (exitsAssess == null)
            {
                var newAssess = new Assess
                {
                    UserId = CurrentUserId,
                    AssessValue = assessValue,
                    ParentId = model.Id,
                    CreateDate = DateTime.Now,
                    AssessType = assessType
                };
                RDM.ObjectType = (int)ObjectType.Assess;
                ManageAction.ProxyCall(RDM,
                                      new Func<Assess, DALReturnModel<Assess>>(AssessDAL.AddReaction), newAssess);
            }
            else if (exitsAssess.AssessValue != assessValue)
            {
                exitsAssess.AssessValue = assessValue;
                ManageAction.ProxyCall(RDM,
                                     new Func<Assess, DALReturnModel<Assess>>(AssessDAL.UpdateReaction), exitsAssess);
            }
            else if (exitsAssess.AssessValue == assessValue)
            {
                //AssessDAL.Delete(exitsAssess);

            }
            RDM.ObjectType = (int)ObjectType.AssessParent;
            ManageAction.ProxyCall(RDM,
                                     new Func<AssessParent, DALReturnModel<AssessParent>>(AssessParentDAL.UpdateReaction), model);
        }

        public ActionResult AssessPreview(int? AssessValue, int? AssessType)
        {
            ViewBag.AssessValue = AssessValue;
            ViewBag.AssessType = AssessType;
            return PartialView();
        }

    }
}
