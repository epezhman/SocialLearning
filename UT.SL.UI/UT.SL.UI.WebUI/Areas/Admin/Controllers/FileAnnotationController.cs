using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using UT.SL.BLL;
using System.Drawing;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class FileAnnotationController : BaseController
    {
        public ActionResult FileContentPreview(int objectId = 0, int type = 0)
        {
            var model = ManageObject.GetSharedObject(objectId, type);
            return PartialView(model);
        }

        public ActionResult FileContentGuidPreview(Guid objectId, int type = 0)
        {
            var model = ManageObject.GetSharedObject(objectId, type);
            return PartialView("FileContentPreview", model);
        }

        public ActionResult FileContentPreviewForDelete(int objectId = 0, int type = 0)
        {
            var model = ManageObject.GetSharedObject(objectId, type);
            return PartialView(model);
        }

        public ActionResult ViewImageResource(int objectId = 0, int type = 0)
        {
            var model = ManageObject.GetSharedObject(objectId, type);
            
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream),
                                       new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), objectId, type, CurrentUser.Id);
            return File(model.FileContent.ToArray(), model.FileMime);
        }

        public ActionResult ViewImageResourceCompressed(int objectId = 0, int type = 0)
        {
            var model = ManageObject.GetSharedObject(objectId, type);
            byte[] tempFile = null;
            System.IO.MemoryStream myMemStream = new System.IO.MemoryStream(model.FileContent);
            var resizeImage = new Bitmap(Image.FromStream(myMemStream, true, true), new Size(90, 90));
            ImageConverter converter = new ImageConverter();
            tempFile = (byte[])converter.ConvertTo(resizeImage, typeof(byte[]));
            model.FileContent = tempFile;
            return File(model.FileContent.ToArray(), model.FileMime);
        }

        public ActionResult DownloadResource(int objectId = 0, int type = 0)
        {
            var model = ManageObject.GetSharedObject(objectId, type);
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream),
                                      new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), objectId, type, CurrentUser.Id);

            return File(model.FileContent.ToArray(), model.FileMime, model.FileTitle);
        }

    }
}
