using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.DAL;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.UI.WebUI.Areas.Sandbox.Models;
using UT.SL.BLL;

namespace UT.SL.UI.WebUI.Areas.Sandbox.Controllers
{
    public class TestController : BaseController
    {
        //
        // GET: /Sandbox/Test/

        public ActionResult Index(int id)
        {
            return View("Index2");
        }

        public ActionResult BadgetViewer(string id)
        {
            if (id != null)
                ViewBag.Title = id;
            else
                ViewBag.Title = String.Empty;
            return PartialView();
        }

        public ActionResult NewBody(UT.SL.Model.ObjectViewModel obj)
        {

            ResourceDAL.UpdateBody(obj.Body, obj.Id);
            return null;
        }
    }
}
