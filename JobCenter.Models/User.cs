using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobCenter.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string RealName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public int IsAdmin { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastLoginTime { get; set; }
    }
}
