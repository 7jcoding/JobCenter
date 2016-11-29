using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;
using Quartz;
using Quartz.Spi;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Impl.Matchers;
using JobCenter.Models;

namespace JobCenter.Common
{
    /// <summary>
    /// 任务处理帮助类
    /// </summary>
    public class QuartzHelper
    {
        private QuartzHelper() { }

        private static object obj = new object();

        private static string scheme = "tcp";

        private static string server = Config.QuartzServer;

        private static string port = Config.QuartzPort;
        /// <summary>
        /// 缓存任务所在程序集信息
        /// </summary>
        private static Dictionary<string, Assembly> AssemblyDict = new Dictionary<string, Assembly>();

        private static IScheduler scheduler = null;


        /// <summary>
        /// 初始化任务调度对象
        /// </summary>
        public static void InitScheduler()
        {
            try
            {
                lock (obj)
                {
                    if (scheduler == null)
                    {
                        #region quartz 实例配置
                        //NameValueCollection properties = new NameValueCollection();

                        //properties["quartz.scheduler.instanceName"] = "ExampleDefaultQuartzScheduler";

                        //properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";

                        //properties["quartz.threadPool.threadCount"] = "10";

                        //properties["quartz.threadPool.threadPriority"] = "Normal";

                        //properties["quartz.jobStore.misfireThreshold"] = "60000";

                        //properties["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";

                        //properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";

                        //properties["quartz.scheduler.exporter.port"] = "555";

                        //properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";

                        //properties["quartz.scheduler.exporter.channelType"] = scheme;

                        //ISchedulerFactory factory = new StdSchedulerFactory(properties);

                        //scheduler = factory.GetScheduler();
                        #endregion
                        //// 配置文件的方式，配置quartz实例
                        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                        scheduler = schedulerFactory.GetScheduler();

                        LogHelper.WriteLog("任务调度初始化成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("任务调度初始化失败！", ex);
            }
        }

        /// <summary>
        /// 启用任务调度
        /// 启动调度时会把任务表中状态为“执行中”的任务加入到任务调度队列中
        /// </summary>
        public static void StartScheduler()
        {
            try
            {
                if (!scheduler.IsStarted)
                {
                    //添加全局监听
                    scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener(), GroupMatcher<TriggerKey>.AnyGroup());
                    scheduler.Start();

                    ///获取所有执行中的任务
                    List<Job> listJob = JobHelper.GetAllTaskList().ToList<Job>();

                    if (listJob != null && listJob.Count > 0)
                    {
                        foreach (Job job in listJob)
                        {
                            try
                            {
                                ScheduleJob(job);
                            }
                            catch (Exception e)
                            {
                                LogHelper.WriteLog(string.Format("任务“{0}”启动失败！", job.JobName), e);
                            }
                        }
                    }
                    LogHelper.WriteLog("任务调度启动成功！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("任务调度启动失败！", ex);
            }
        }

        /// <summary>
        /// 初始化 远程Quartz服务器中的，各个Scheduler实例。
        /// 提供给远程管理端的后台，用户获取Scheduler实例的信息。
        /// </summary>
        public static void InitRemoteScheduler()
        {
            try
            {
                NameValueCollection properties = new NameValueCollection();

                properties["quartz.scheduler.instanceName"] = "schedMaintenanceService";

                properties["quartz.scheduler.proxy"] = "true";

                properties["quartz.scheduler.proxy.address"] = string.Format("{0}://{1}:{2}/QuartzScheduler", scheme, server, port);

                ISchedulerFactory sf = new StdSchedulerFactory(properties);

                scheduler = sf.GetScheduler();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("初始化远程任务管理器失败" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除现有任务
        /// </summary>
        /// <param name="JobKey"></param>
        public static void DeleteJob(string JobKey)
        {
            try
            {
                JobKey jk = new JobKey(JobKey);
                if (scheduler.CheckExists(jk))
                {
                    //任务已经存在则删除
                    scheduler.DeleteJob(jk);
                    LogHelper.WriteLog(string.Format("任务“{0}”已经删除", JobKey));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 启用任务
        /// <param name="task">任务信息</param>
        /// <param name="isDeleteOldTask">是否删除原有任务</param>
        /// <returns>返回任务trigger</returns>
        /// </summary>
        public static void ScheduleJob(Job task, bool isDeleteOldTask = false)
        {
            if (isDeleteOldTask)
            {
                //先删除现有已存在任务
                DeleteJob(task.JobID.ToString());
            }
            //验证是否正确的Cron表达式
            if (ValidExpression(task.CronExpression))
            {
                IJobDetail job = new JobDetailImpl(task.JobID.ToString(), GetClassInfo(task.AssemblyName, task.ClassName));
                //添加任务执行参数
                job.JobDataMap.Add("JobParams", task.JobParams);

                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = task.CronExpression;
                trigger.Name = task.JobID.ToString();
                trigger.Description = task.JobName;
                scheduler.ScheduleJob(job, trigger);
                if (task.Status == JobStatus.STOP)
                {
                    JobKey jk = new JobKey(task.JobID.ToString());
                    scheduler.PauseJob(jk);
                }
                else
                {
                    LogHelper.WriteLog(string.Format("任务“{0}”启动成功,未来5次运行时间如下:", task.JobName));
                    List<DateTime> list = GetNextRunTime(task.CronExpression, 5);
                    foreach (var time in list)
                    {
                        LogHelper.WriteLog(time.ToString());
                    }
                }
            }
            else
            {
                throw new Exception(task.CronExpression + "不是正确的Cron表达式,无法启动该任务!");
            }
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="JobKey"></param>
        public static void PauseJob(string JobKey)
        {
            try
            {
                JobKey jk = new JobKey(JobKey);
                if (scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    scheduler.PauseJob(jk);
                    LogHelper.WriteLog(string.Format("任务“{0}”已经暂停", JobKey));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="JobKey">任务key</param>
        public static void ResumeJob(string JobKey)
        {
            try
            {
                JobKey jk = new JobKey(JobKey);
                if (scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    scheduler.ResumeJob(jk);
                    LogHelper.WriteLog(string.Format("任务“{0}”恢复运行", JobKey));
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("恢复任务失败！", ex);
            }
        }

        /// 获取类的属性、方法  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        /// <param name="className">类名</param>  
        private static Type GetClassInfo(string assemblyName, string className)
        {
            try
            {
                assemblyName = FileHelper.GetAbsolutePath(assemblyName + ".dll");
                Assembly assembly = null;
                if (!AssemblyDict.TryGetValue(assemblyName, out assembly))
                {
                    assembly = Assembly.LoadFrom(assemblyName);
                    AssemblyDict[assemblyName] = assembly;
                }
                Type type = assembly.GetType(className, true, true);
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 停止任务调度
        /// </summary>
        public static void StopSchedule()
        {
            try
            {
                //判断调度是否已经关闭
                if (!scheduler.IsShutdown)
                {
                    //等待任务运行完成
                    scheduler.Shutdown(true);
                    LogHelper.WriteLog("任务调度停止！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("任务调度停止失败！", ex);
            }
        }

        /// <summary>
        /// 校验字符串是否为正确的Cron表达式
        /// </summary>
        /// <param name="cronExpression">带校验表达式</param>
        /// <returns></returns>
        public static bool ValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }

        /// <summary>
        /// 获取任务在未来周期内哪些时间会运行
        /// </summary>
        /// <param name="CronExpression">Cron表达式</param>
        /// <param name="numTimes">运行次数</param>
        /// <returns>运行时间段</returns>
        public static List<DateTime> GetNextRunTime(string CronExpressionString, int numTimes)
        {
            if (numTimes < 0)
            {
                throw new Exception("参数numTimes值大于等于0");
            }
            //时间表达式
            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(CronExpressionString).Build();
            IList<DateTimeOffset> dates = TriggerUtils.ComputeFireTimes(trigger as IOperableTrigger, null, numTimes);
            List<DateTime> list = new List<DateTime>();
            foreach (DateTimeOffset dtf in dates)
            {
                list.Add(TimeZoneInfo.ConvertTimeFromUtc(dtf.DateTime, TimeZoneInfo.Local));
            }
            return list;
        }

        /// <summary>
        /// 获取当前执行的任务列表
        /// </summary>
        /// <returns></returns>
        public static object CurrentJobList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取当前执行的Job对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Job GetTaskDetail(IJobExecutionContext context)
        {
            Job job = new Job();
            if (context != null)
            {
                job.JobID = Guid.Parse(context.Trigger.Key.Name);
                job.JobName = context.Trigger.Description;
                job.RecentRunTime = DateTime.Now;
                job.JobParams = context.JobDetail.JobDataMap.Get("JobParams") != null ? context.JobDetail.JobDataMap.Get("JobParams").ToString() : "";
            }
            return job;
        }
    }
}
