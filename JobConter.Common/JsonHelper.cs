using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;

namespace JobCenter.Common
{
    public static class JsonHelper
    {
        public static String ToJson(object obj)
        {
            return JsonSerializer.SerializeToString(obj);
        }

        public static T ToObject<T>(string json)
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }
    }
}
