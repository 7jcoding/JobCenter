using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public List<Job> GetTaskList(int pageIndex, int pageSize)
        {
            return null;
        }

        /// <summary>
        /// 读取数据库中全部的任务
        /// </summary>
        /// <returns></returns>
        public List<Job> GetAllTaskList()
        {
            var sql = @"SELECT TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                    FROM p_Task(nolock)
                        WHERE IsDelete=0 and Status =1";

            return null;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(string taskId, int status)
        {
            var sql = @" UPDATE p_Task
                           SET Status = @Status 
                         WHERE TaskID=@TaskID
                        ";

            return false;
        }

        /// <summary>
        /// 修改任务的下次启动时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="nextFireTime"></param>
        /// <returns></returns>
        public bool UpdateNextFireTime(string taskId, DateTime nextFireTime)
        {
            var sql = @" UPDATE p_Task
                           SET NextFireTime = @NextFireTime 
                               ,ModifyTime = GETDATE()
                         WHERE TaskID=@TaskID
                        ";
            return false;
        }

        /// <summary>
        /// 根据任务Id 修改 上次运行时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="recentRunTime"></param>
        /// <returns></returns>
        public bool UpdateRecentRunTime(string taskId, DateTime recentRunTime)
        {
            var sql = @" UPDATE p_Task
                           SET RecentRunTime = @RecentRunTime 
                               ,ModifyTime = GETDATE()
                         WHERE TaskID=@TaskID
                        ";

            return false;
        }

        /// <summary>
        /// 根据任务Id 获取任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Job GetTaskById(string taskId)
        {
            var sql = @"SELECT TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                    FROM p_Task(nolock)
                        WHERE TaskID=@TaskID";

            return null;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add(Job task)
        {
            var sql = @" INSERT INTO p_Task
                               (TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,CronRemark,Remark)
                         VALUES
                               (@TaskID ,@TaskName,@TaskParam,@CronExpressionString,@AssemblyName,@ClassName,@Status,0,getdate(),getdate(),@CronRemark,@Remark)";


            return false;
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Edit(Job task)
        {

            var sql = @" UPDATE p_Task
                           SET TaskName = @TaskName,TaskParam = @TaskParam,CronExpressionString = @CronExpressionString,AssemblyName = @AssemblyName,ClassName = @ClassName,
                               Status = @Status,IsDelete = 0,ModifyTime =getdate() ,CronRemark = @CronRemark,Remark = @Remark
                         WHERE TaskID = @TaskID";


            return false;
        }
    }
}
