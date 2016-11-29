using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JobCenter.Common;

namespace JobCenter.Server
{
    /// <summary>
    /// Job处理服务
    /// </summary>
    public class JobCenterServer
    {
        public void Start()
        {
            //配置信息读取
            //配置信息读取
            ConfigInit.InitConfig();
            QuartzHelper.InitScheduler();
            QuartzHelper.StartScheduler();
        }

        public void Stop()
        {
            QuartzHelper.StopSchedule();
            System.Environment.Exit(0);
        }
    }
}
