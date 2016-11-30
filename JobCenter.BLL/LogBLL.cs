using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JobCenter.DAL;
using JobCenter.Models;

namespace JobCenter.BLL
{
    public class LogBLL
    {
        public readonly LogDAL dal = new LogDAL();

        /// <summary>
        /// 记录运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="taskId"></param>
        /// <param name="result"></param>
        public void WriteRunInfo(string taskName, string taskId, string result)
        {
            dal.WriteRunInfo(taskName, taskId, result);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="sLevel"></param>
        /// <param name="sMessage"></param>
        /// <param name="sException"></param>
        /// <param name="sName"></param>
        public void WriteErrorInfo(string sLevel, string sMessage, string sException, string sName)
        {
            dal.WriteErrorInfo(sLevel, sMessage, sException, sName);
        }

        /// <summary>
        /// 读取错误日志列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<Log.ErrorLog> GetErrorLogList(int pageNo, int pageSize)
        {
            return dal.GetErrorLogList(pageNo, pageSize);
        }

        /// <summary>
        /// 读取运行日志列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<Log.RunLog> GetRunLogList(int pageNo, int pageSize)
        {
            return dal.GetRunLogList(pageNo, pageSize);
        }
    }
}
