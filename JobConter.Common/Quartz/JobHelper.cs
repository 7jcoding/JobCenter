using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JobCenter.BLL;
using JobCenter.Models;

namespace JobCenter.Common
{
    /// <summary>
    /// 任务帮助类
    /// </summary>
    public class JobHelper
    {
        /// <summary>
        /// 配置文件地址
        /// </summary>
        private static readonly string TaskPath = FileHelper.GetAbsolutePath("Config/JobConfig.xml");

        private static JobBLL task = new JobBLL();

        public static bool AddTask(Job model, string action)
        {
            var result = false;

            if (action == "edit")
            {
                result = task.Edit(model);
            }
            else
            {
                model.JobID = Guid.NewGuid();
                result = task.Add(model);
            }

            if (result)
            {
                QuartzHelper.ScheduleJob(model, true);
            }
            return result;
        }

        /// <summary>
        /// 删除指定id任务
        /// </summary>
        /// <param name="TaskID">任务id</param>
        public static void DeleteById(string taskId)
        {
            QuartzHelper.DeleteJob(taskId);

            task.DeleteById(taskId);
        }

        /// <summary>
        /// 更新任务运行状态
        /// </summary>
        /// <param name="JobID">任务id</param>
        /// <param name="Status">任务状态</param>
        public static void UpdateTaskStatus(string JobId, JobStatus Status)
        {
            if (Status == JobStatus.RUN)
            {
                QuartzHelper.ResumeJob(JobId);
            }
            else
            {
                QuartzHelper.PauseJob(JobId);
            }
            task.UpdateTaskStatus(JobId, (int)Status);
        }

        /// <summary>
        /// 更新任务下次运行时间
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="NextFireTime">下次运行时间</param>
        public static void UpdateNextFireTime(string taskId, DateTime nextFireTime)
        {
            task.UpdateNextFireTime(taskId, nextFireTime);
        }

        /// <summary>
        /// 任务完成后，更新上次执行时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="recentRunTime">上次执行时间</param>
        public static void UpdateRecentRunTime(string taskId, DateTime recentRunTime)
        {
            task.UpdateRecentRunTime(taskId, recentRunTime);
        }

        /// <summary>
        /// 从数据库中读取全部任务列表
        /// </summary>
        /// <returns></returns>
        private static IList<Job> TaskInDB()
        {
            return task.GetJobList();
        }

        /// <summary>
        /// 从配置文件中读取任务列表
        /// </summary>
        /// <returns></returns>
        private static IList<Job> ReadTaskConfig()
        {
            return XmlHelper.XmlToList<Job>(TaskPath, "Job");
        }

        /// <summary>
        /// 获取所有启用的任务
        /// </summary>
        /// <returns>所有启用的任务</returns>
        public static IList<Job> GetAllTaskList()
        {
            if (Config.StorageMode == 1)
            {
                return TaskInDB();
            }
            else
            {
                return ReadTaskConfig();
            }
        }

        /// <summary>
        /// 当前正在执行的任务列表
        /// </summary>
        /// <returns></returns>
        public static IList<Job> CurrentTaskList()
        {
            return task.GetJobList();
        }
    }
}
