using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobCenter.Models
{
    public class Log
    {
        /// <summary>
        /// 运行日志
        /// </summary>
        public class RunLog
        {
            public long Id { get; set; }

            public string JobID { get; set; }

            public string Remark { get; set; }

            public string Description { get; set; }

            public DateTime CreateTime { get; set; }

            public string JobName { get; set; }

            public string ClassName { get; set; }
        }

        /// <summary>
        /// 出错日志
        /// </summary>
        public class ErrorLog
        {
            public long Id { get; set; }

            public DateTime CreateTime { get; set; }

            public string Thread { get; set; }

            public string Level { get; set; }

            public string Logger { get; set; }

            public string Message { get; set; }

            public string Exception { get; set; }

            public string Name { get; set; }
        }
    }
}
