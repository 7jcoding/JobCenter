using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobCenter.Models
{
    /// <summary>
    /// 用户加密串日志记录
    /// </summary>
    public class UnidLog
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户加密串
        /// </summary>
        public string Unid { get; set; }
        /// <summary>
        /// 用户请求地址
        /// </summary>
        public string RequestUrl { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
