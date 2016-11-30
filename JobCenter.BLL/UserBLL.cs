using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JobCenter.DAL;
using JobCenter.Models;

namespace JobCenter.BLL
{
    public class UserBLL
    {
        private UserDAL dal = new UserDAL();

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageOf<User> GetUserList(int pageNo, int pageSize)
        {
            return dal.GetUserList(pageNo, pageSize);
        }

        /// <summary>
        /// 根据用户名和密码获取管理员用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public User GetUserModel(string userName, string pwd)
        {
            return dal.GetUserModel(userName, pwd);
        }
    }
}
