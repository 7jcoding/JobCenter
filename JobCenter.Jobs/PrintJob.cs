using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using JobCenter.Common;

namespace JobCenter.Jobs
{
    [DisallowConcurrentExecution]
    public class PrintJob : JobBase, IJob
    {
        /// <summary>
        /// IJob 接口
        /// </summary>
        /// <param name="context"></param>        
        public void Execute(IJobExecutionContext context)
        {
            base.ExecuteJob(context, () =>
            {
                try
                {
                    LogHelper.WriteLog("当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    LogHelper.WriteLog(">>Job模板【打印服务】");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(context.Trigger.Description, ex);
                }
            });
        }
    }
}
