﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using JobCenter.BLL;

namespace JobCenter.Console.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult List()
        {
            UserBLL userBll = new UserBLL();
            var users = userBll.GetUserList(PageNo, PageSize);
            return View(users);
        }
    }
}