﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    #region Razor Demos
    public static class NameUtil
    {
        public static string GetName()
        {
            return "Name: Çağıl Alsaç";
        }
    }
    #endregion
    public class DemosController : Controller
    {

        #region
        // GET: Demos
        public ActionResult Razor1()
        {
            return View();
        }
        public ActionResult Razor2()
        {
            return View();
        }
        #endregion
    }
}