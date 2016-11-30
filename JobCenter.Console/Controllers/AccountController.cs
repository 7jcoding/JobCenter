using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;
using JobCenter.BLL;
using JobCenter.Models;
using JobCenter.Common;

namespace JobCenter.Console.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string errorInfo, string returnUrl)
        {
            ViewBag.ErrorInfo = errorInfo;
            ViewBag.ReturnUrl = string.IsNullOrEmpty(Request["ReturnUrl"]) ? returnUrl : Request["ReturnUrl"];
            return View();
        }

        public ActionResult UserLogin(string userName, string pwd, string returnUrl)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pwd))
            {
                var error = "";
                var checkResult = true; //CheckUserInfo(userName, pwd, out error);
                if (checkResult)
                {
                    User user = new User();
                    user.UserName = userName;
                    Session["loginuser"] = user;
                    if (string.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Home");
                    else
                        return Redirect(returnUrl);

                }
                else
                {
                    return new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                                                    (new { Controller = "Account", action = "Login", errorInfo = error }));
                }

            }
            else
            {
                return new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
               (new { Controller = "Account", action = "Login", errorInfo = "用户名或密码不能为空" }));
            }

        }

        private bool CheckUserInfo(string userName, string pwd, out string error)
        {
            error = string.Empty;

            UserBLL userBll = new UserBLL();
            var users = userBll.GetUserModel(userName, Md5Hash.GetMd5String(pwd));
            if (users == null)
            {
                error = "用户名或密码错误";
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
