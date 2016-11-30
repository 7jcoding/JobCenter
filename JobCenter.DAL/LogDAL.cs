using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using JobCenter.Models;

namespace JobCenter.DAL
{
    public class LogDAL
    {
        /// <summary>
        /// 记录运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="taskId"></param>
        /// <param name="result"></param>
        public void WriteRunInfo(string remark, string taskId, string result)
        {
            var sql = @"INSERT INTO J_RunningLog
                            (JobID
                            ,Remark
                            ,Description)
                        VALUES
                            (@JobID
                            ,@Remark
                            ,@Description)";

            object param = new { JobID = taskId, Remark = remark, Description = result };

            SQLHelper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Message"></param>
        /// <param name="Exception"></param>
        /// <param name="Name"></param>
        public void WriteErrorInfo(string Level, string Message, string Exception, string Name)
        {
            var sql = @"INSERT INTO J_ErrorLog
                                ([Level]
                                ,[Logger]
                                ,[Message]
                                ,[Exception]
                                ,[Name])
                         VALUES
                               (@Level
                               ,@Logger
                               ,@Message
                               ,@Exception
                               ,@Name)";

            object param = new { Level = Level, Logger = "system", Message = Message, Exception = Exception, Name = Name };
            SQLHelper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 读取错误日志列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<Log.ErrorLog> GetErrorLogList(int pageNo, int pageSize)
        {
            var QUERY_SQL = @"(  select Id,CreateTime,Thread,Level,Logger,Message,Exception,Name
                                 from J_ErrorLog 
                                 WHERE DATEDIFF(dd,CreateTime,GETDATE())<=30";

            QUERY_SQL += ") pp ";
            string SQL = string.Format(@" SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY pp.dtDate DESC) AS RowNum,* FROM {0}
										) as A WHERE A.RowNum BETWEEN (@PageIndex-1)* @PageSize+1 AND @PageIndex*@PageSize ORDER BY RowNum;",
                                  QUERY_SQL);

            SQL += string.Format(@" SELECT COUNT(1) FROM {0};", QUERY_SQL);

            object param = new { pageIndex = pageNo, pageSize = pageSize };

            DataSet ds = SQLHelper.FillDataSet(SQL, param);
            return new PageOf<Log.ErrorLog>()
            {
                PageIndex = pageNo,
                PageSize = pageSize,
                Total = Convert.ToInt32(ds.Tables[1].Rows[0][0]),
                Items = DataMapHelper.DataSetToList<Log.ErrorLog>(ds)
            };
        }

        /// <summary>
        /// 读取运行日志列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<Log.RunLog> GetRunLogList(int pageNo, int pageSize)
        {
            var QUERY_SQL = @"( SELECT Id,Remark,Description,CreateTime,j.JobName,j.ClassName
                                FROM J_RunningLog r INNER JOIN J_Jobs j on r.JobID = j.JobID 
                                WHERE DATEDIFF(dd,r.CreateTime,GETDATE())<=30";

            QUERY_SQL += ") pp ";
            string SQL = string.Format(@" SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY pp.CreateTime DESC) AS RowNum,* FROM {0}
										) AS A WHERE A.RowNum BETWEEN (@PageIndex-1)* @PageSize+1 AND @PageIndex*@PageSize ORDER BY RowNum;",
                                  QUERY_SQL);

            SQL += string.Format(@" SELECT COUNT(1) FROM {0};", QUERY_SQL);

            object param = new { pageIndex = pageNo, pageSize = pageSize };

            DataSet ds = SQLHelper.FillDataSet(SQL, param);
            return new PageOf<Log.RunLog>()
            {
                PageIndex = pageNo,
                PageSize = pageSize,
                Total = Convert.ToInt32(ds.Tables[1].Rows[0][0]),
                Items = DataMapHelper.DataSetToList<Log.RunLog>(ds)
            };
        }
    }
}
