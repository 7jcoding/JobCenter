using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using JobCenter.Models;

namespace JobCenter.DAL
{
    public class UserDAL
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<User> GetUserList(int pageNo, int pageSize)
        {
            var QUERY_SQL = @"( SELECT UserId,UserName,PassWord,RealName,Email,Mobile,IsAdmin,Status,CreateTime,LastLoginTime
                                FROM U_User";

            QUERY_SQL += ") pp ";
            string SQL = string.Format(@" SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY pp.CreateTime desc) AS RowNum,* FROM {0}
										) AS A WHERE A.RowNum BETWEEN (@PageIndex-1)* @PageSize+1 AND @PageIndex*@PageSize ORDER BY RowNum;",
                                  QUERY_SQL);

            SQL += string.Format(@" SELECT COUNT(1) FROM {0};", QUERY_SQL);

            object param = new { pageIndex = pageNo, pageSize = pageSize };

            DataSet ds = SQLHelper.FillDataSet(SQL, param);
            return new PageOf<User>()
            {
                PageIndex = pageNo,
                PageSize = pageSize,
                Total = Convert.ToInt32(ds.Tables[1].Rows[0][0]),
                Items = DataMapHelper.DataSetToList<User>(ds)
            };
        }

        /// <summary>
        /// 根据用户名和密码获取管理员用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public User GetUserModel(string userName, string pwd)
        {
            var sql = @"  SELECT UserId,UserName,PassWord,RealName,Email,Mobile,IsAdmin,Status,CreateTime,LastLoginTime
                          FROM  U_User
                          WHERE UserName = @UserName AND PassWord = @PassWord";

            object param = new { UserName = userName, PassWord = pwd };

            return SQLHelper.Single<User>(sql, param);

        }
    }
}
