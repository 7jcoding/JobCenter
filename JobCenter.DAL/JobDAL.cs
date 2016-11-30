using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using JobCenter.Models;

namespace JobCenter.DAL
{
    public class JobDAL
    {
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<Job> GetTaskList(int pageIndex, int pageSize)
        {
            var QUERY_SQL = @"( SELECT JobID,JobName,JobParams,CronExpression,AssemblyName,ClassName,Status,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                            FROM J_Jobs WHERE Status = 1";

            QUERY_SQL += ") pp ";
            string SQL = string.Format(@" SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY pp.ModifyTime desc) AS RowNum,* FROM {0}
										) as A WHERE A.RowNum BETWEEN (@PageIndex-1)* @PageSize+1 AND @PageIndex*@PageSize ORDER BY RowNum;",
                                  QUERY_SQL);

            SQL += string.Format(@" SELECT COUNT(1) FROM {0};", QUERY_SQL);

            object param = new { pageIndex = pageIndex, pageSize = pageSize };

            DataSet ds = SQLHelper.FillDataSet(SQL, param);
            return new PageOf<Job>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Total = Convert.ToInt32(ds.Tables[1].Rows[0][0]),
                Items = DataMapHelper.DataSetToList<Job>(ds)
            };
        }

        /// <summary>
        /// 读取数据库中全部的任务
        /// </summary>
        /// <returns></returns>
        public List<Job> GetJobList()
        {
            var sql = @"SELECT JobID,JobName,JobParams,CronExpression,AssemblyName,ClassName,Status,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                    FROM J_Jobs WHERE Status = 1";
            var result = SQLHelper.ToList<Job>(sql);
            return result;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(string taskId, int status)
        {
            var sql = @" UPDATE J_Jobs SET Status = @Status WHERE JobID = @JobID";
            object param = new { JobID = taskId, Status = status };
            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 修改任务的下次启动时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="nextFireTime"></param>
        /// <returns></returns>
        public bool UpdateNextFireTime(string taskId, DateTime nextFireTime)
        {
            var sql = @" UPDATE J_Jobs
                         SET NextFireTime = @NextFireTime,ModifyTime = GETDATE()
                         WHERE JobID = @JobID";

            object param = new { JobID = taskId, NextFireTime = nextFireTime };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 根据任务Id 修改 上次运行时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="recentRunTime"></param>
        /// <returns></returns>
        public bool UpdateRecentRunTime(string taskId, DateTime recentRunTime)
        {
            var sql = @" UPDATE J_Jobs
                         SET RecentRunTime = @RecentRunTime,ModifyTime = GETDATE()
                         WHERE JobID = @JobID";

            object param = new { JobID = taskId, RecentRunTime = recentRunTime };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 根据任务Id 获取任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Job GetTaskById(string taskId)
        {
            var sql = @"SELECT JobID,JobName,JobParams,CronExpression,AssemblyName,ClassName,Status,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                    FROM J_Jobs
                        WHERE JobID = @JobID";

            object param = new { JobID = taskId };
            var result = SQLHelper.Single<Job>(sql, param);

            return result;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add(Job task)
        {
            var sql = @" INSERT INTO J_Jobs
                               (JobID,JobName,JobParams,CronExpression,AssemblyName,ClassName,Status,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark)
                         VALUES
                               (@JobID,@JobName,@JobParams,@CronExpression,@AssemblyName,@ClassName,@Status,@CreatedTime,@ModifyTime,@RecentRunTime,@NextFireTime,@CronRemark,@Remark)";

            object param = new
            {
                JobID = task.JobID,
                JobName = task.JobName,
                JobParams = task.JobParams,
                CronExpressionString = task.CronExpression,
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                Status = task.Status,
                CronRemark = task.CronRemark,
                Remark = task.Remark
            };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Edit(Job task)
        {
            var sql = @" UPDATE J_Jobs
                           SET JobName = @JobName,JobParams = @JobParams,CronExpression = @CronExpression,AssemblyName = @AssemblyName,ClassName = @ClassName,
                               Status = @Status,IsDelete = 0,ModifyTime =GETDATE() ,CronRemark = @CronRemark,Remark = @Remark
                         WHERE JobID = @JobID";

            object param = new
            {
                JobID = task.JobID,
                JobName = task.JobName,
                JobParams = task.JobParams,
                CronExpressionString = task.CronExpression,
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                Status = task.Status,
                CronRemark = task.CronRemark,
                Remark = task.Remark
            };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }
    }
}
