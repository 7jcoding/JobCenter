using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace JobCenter.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {
                x.Service<JobCenterServer>(s =>
                {
                    s.ConstructUsing(name => new JobCenterServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomaticallyDelayed();
                x.SetDescription("任务调度服务处理中心");
                x.SetDisplayName("任务调度服务处理中心");
                x.SetServiceName("JobCenterServer");
            });
        }
    }
}
