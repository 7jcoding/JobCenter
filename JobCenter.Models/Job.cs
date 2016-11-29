using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobCenter.Models
{
    public class Job
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid JobID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任务执行参数
        /// </summary>
        public string JobParams { get; set; }

        /// <summary>
        /// 运行频率规则设置
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// 任务运频率备注说明
        /// </summary>
        public string CronRemark { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 任务修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 任务最近运行时间
        /// </summary>
        public DateTime RecentRunTime { get; set; }

        /// <summary>
        /// 任务下次运行时间
        /// </summary>
        public DateTime NextRunTime { get; set; }

        /// <summary>
        /// 任务备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }
    }

    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// 停止状态
        /// </summary>
        STOP = 0,
        /// <summary>
        /// 运行状态
        /// </summary>
        RUN = 1
    }

    /// <summary>
    /// 任务配置的方式
    /// </summary>
    public enum JobStore
    {
        /// <summary>
        /// 数据库存储
        /// </summary>
        DB = 1,
        /// <summary>
        /// XML存储
        /// </summary>
        XML = 2
    }
}
