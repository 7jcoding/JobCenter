using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Quartz;
using JobCenter.BLL;
using JobCenter.Models;
using JobCenter.Common;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace JobCenter.Jobs
{
    /// <summary>
    /// 用户加密串日志信息处理
    /// </summary>
    /// <remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class UnidProcessJob : JobBase, IJob
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
                    // 3. 开始执行相关任务
                    LogHelper.WriteLog("当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    var process_data = DateTime.Now;
                    var yesterday = process_data.AddDays(-1);
                    LogHelper.WriteLog(string.Format("正在处理[{0}]的用户加密串日记录", yesterday.ToString(Config.SHORT_DATE_FORMAT)));
                    ProcessUnidFromRedis(yesterday);
                    LogHelper.WriteLog(string.Format("正在处理[{0}]的用户加密串日记录", process_data.ToString(Config.SHORT_DATE_FORMAT)));
                    ProcessUnidFromRedis(process_data);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(context.Trigger.Description, ex);
                }
            });
        }
        /// <summary>
        /// 把Redis数据搬迁到sqlserver
        /// </summary>
        public void ProcessUnidFromRedis(DateTime process_data)
        {
            var logBLL = new UnidLogBLL();
            var list = new List<UnidLog>();
            var dic = new Dictionary<string,string>();
            var hashId = Config.PC_UNID_REIDS_KEY_PREFIX + process_data.ToString(Config.SHORT_DATE_FORMAT);
            using (var client = RedisClientHelper.LoginPCInstance.GetClient())
            {
                dic = client.GetAllEntriesFromHash(hashId);                
                int i = 0;
                foreach (var item in dic)
                {
                    if (logBLL.AddUnidLog(JsonSerializer.DeserializeFromString<UnidLog>(item.Value)))
                    {
                        client.RemoveEntryFromHash(hashId, item.Key);
                        i++;
                    }
                }
                LogHelper.WriteLog(string.Format("共有[{0}]条记录被处理，[{1}]条记录未处理", i, dic.Count - i));
            }            
        }
    }
}
