using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobCenter.Common
{
    /// <summary>
    /// 公用配置文件
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 数据库连接字符串信息
        /// </summary>
        [PathMap(Key = "SqlConnect")]
        public static string SqlConnect { get; set; }

        #region  quartz 服务器的地址和端口

        [PathMap(Key = "QuartzServer")]
        public static string QuartzServer { get; set; }

        [PathMap(Key = "QuartzPort")]
        public static string QuartzPort { get; set; }

        #endregion

        /// <summary>
        /// 任务配置的存储方式
        /// </summary>
        [PathMap(Key = "StorageMode")]
        public static int StorageMode { get; set; }
        /// <summary>
        /// 短日期格式：yyyyMMdd，如：20161128
        /// </summary>
        public static string SHORT_DATE_FORMAT = "yyyyMMdd";
        /// <summary>
        /// PC用户加密串，Redis Key 前缀
        /// </summary>
        public static string PC_UNID_REIDS_KEY_PREFIX = "PC:Log:Unid:";
    }
}
