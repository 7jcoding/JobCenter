using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JobCenter.DAL;
using JobCenter.Models;

namespace JobCenter.BLL
{
    public class UnidLogBLL
    {
        private readonly UnidLogDAL _dal = new UnidLogDAL();

        /// 添加用户加密串日志记录
        /// </summary>
        /// <returns></returns>
        public bool AddUnidLog(UnidLog model)
        {
            return model == null ? false : _dal.AddUnidLog(model) > 0;
        }
    }
}
