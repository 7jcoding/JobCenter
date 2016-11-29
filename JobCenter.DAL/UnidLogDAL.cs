using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using JobCenter.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace JobCenter.DAL
{
    public class UnidLogDAL
    {
        Database db = DatabaseFactory.CreateDatabase("fh_log");
        /// 添加用户加密串日志记录
        /// </summary>
        /// <returns></returns>
        public int AddUnidLog(UnidLog model)
        {
            string sql = "INSERT INTO FH_UnidLog(UserId,Unid,RequestUrl,CreateTime)VALUES(@UserId,@Unid,@RequestUrl,@CreateTime)";
            using (DbCommand cmd = db.GetSqlStringCommand(sql))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, model.UserId);
                db.AddInParameter(cmd, "@Unid", DbType.String, model.Unid);
                db.AddInParameter(cmd, "@RequestUrl", DbType.String, model.RequestUrl);
                db.AddInParameter(cmd, "@CreateTime", DbType.DateTime, model.CreateTime);
                return db.ExecuteNonQuery(cmd);
            }
        }
    }
}
