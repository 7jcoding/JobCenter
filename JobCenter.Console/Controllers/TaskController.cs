using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using JobCenter.BLL;
using JobCenter.Models;
using JobCenter.Common;

namespace JobCenter.Console.Controllers
{
    public class TaskController : BaseController
    {

        JobBLL task = new JobBLL();

        public ActionResult Grid()
        {
            var result = task.GetTaskList(PageNo, PageSize);

            return View(result);
        }

        public ActionResult Edit(string taskId)
        {
            Job model = new Job();
            if (!string.IsNullOrEmpty(taskId))
            {
                model = task.GetTaskById(taskId);
            }
            return PartialView("_Edit", model);
        }

        public JsonResult Save(string data, string action)
        {
            try
            {
                var taskmodel = JsonHelper.ToObject<Job>(data);

                var result = false;
                if (QuartzHelper.ValidExpression(taskmodel.CronExpression))
                {
                    result = JobHelper.AddTask(taskmodel, action);
                    return Json(new { result = false, msg = "保存成功" });
                }
                else
                {
                    return Json(new { result = false, msg = "Cron表达式错误" });
                }

                
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = ex.Message });
            }
        }

        public ActionResult UpdateStatus(string taskId, int status)
        {
            try
            {
                JobHelper.UpdateTaskStatus(taskId, (JobStatus)status);

                return Json(new { result = true, msg = "操作成功" });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = ex.Message });
            }
        }
    }
}