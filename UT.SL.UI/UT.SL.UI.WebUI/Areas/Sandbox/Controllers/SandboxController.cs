using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.UI.WebUI.Controllers;

namespace UT.SL.UI.WebUI.Areas.Sandbox.Controllers
{
    public class SandboxController : BaseController
    {
        //
        // GET: /Sandbox/Sandbox/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Sandbox/Sandbox/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Sandbox/Sandbox/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Sandbox/Sandbox/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Sandbox/Sandbox/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Sandbox/Sandbox/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Sandbox/Sandbox/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Sandbox/Sandbox/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
      
    }
}
