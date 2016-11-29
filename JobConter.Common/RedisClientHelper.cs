using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ServiceStack.Redis;

namespace JobCenter.Common
{
    public class RedisClientHelper
    {
        public PooledRedisClientManager pcm;

        private PooledRedisClientManager GetDefaultManager()
        {
            lock (this)
            {
                if (pcm == null)
                {
                    pcm = new PooledRedisClientManager(writeServerList, readServerList,
                        new RedisClientManagerConfig()
                        {
                            MaxWritePoolSize = 1000,
                            MaxReadPoolSize = 20000,
                            AutoStart = true
                        }, 0, 50, 2);
                    pcm.ConnectTimeout = 1000;
                    pcm.IdleTimeOutSecs = 20;
                    //pcm.PoolTimeout = 2000;
                }
            }
            return pcm;
        }

        private string serverGroup;

        private string writeServersSetting
        {
            get { return serverGroup + "-redis-w"; }
        }

        private string ReadServersSetting
        {
            get { return serverGroup + "-redis-r"; }
        }

        public IEnumerable<string> writeServerList
        {
            get
            {
                return SplitString(ConfigurationManager.AppSettings[writeServersSetting], ",");
            }
        }

        public IEnumerable<string> readServerList
        {
            get { return SplitString(ConfigurationManager.AppSettings[ReadServersSetting], ","); }
        }

        public RedisClientHelper(string serverGroup)
        {
            this.serverGroup = serverGroup;
        }
        /// <summary>
        /// 获取只读链接
        /// </summary>
        /// <returns></returns>
        public IRedisClient GetReadClient()
        {
            IRedisClient client = GetDefaultManager().GetReadOnlyClient();
            client.Password = ConfigurationManager.AppSettings["RedisConnectionAuth"];
            return client;
        }
        /// <summary>
        /// 获取可写、可读链接
        /// </summary>
        /// <returns></returns>
        public IRedisClient GetClient()
        {
            IRedisClient client = GetDefaultManager().GetClient();
            client.Password = ConfigurationManager.AppSettings["RedisConnectionAuth"];

            return client;
        }

        public void DisposeClient()
        {
            IRedisClient client = GetDefaultManager().GetClient();
            client.Dispose();
        }

        #region "帮助拓展"
        /// <summary>
        /// 获取Redis存储类型为string 的数据 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            using (var client = GetReadClient())
            {
                var isHave = client.ContainsKey(key);
                try
                {
                    if (isHave) return client.Get<T>(key);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    //client.Dispose();
                }
                return default(T);
            }
        }

        /// <summary>
        /// 获取Redis存储类型为string 的数据 ,如果不存在就用函数返回的值,自动加入缓存.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> fun, DateTime expiresAt) where T : class
        {
            var chacheData = Get<T>(key);
            if (chacheData != null) return chacheData;
            var saveData = fun();
            using (var client = GetClient())
            {
                client.Set<T>(key, saveData, expiresAt);
                //client.Dispose();
                return saveData;
            }
        }

        /// <summary>
        /// 移除key对应的Redis缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            using (var client = GetClient())
            {
                client.Remove(key);
                //client.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 分割配置的IP
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        private static IEnumerable<string> SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        private static RedisClientHelper commonRedisClientHelper = new RedisClientHelper("common");
        public static RedisClientHelper CommonInstance
        {
            get
            {
                return commonRedisClientHelper;
            }
        }

        private static RedisClientHelper pcRedisClientHelper = new RedisClientHelper("pc");
        public static RedisClientHelper PcInstance
        {
            get
            {
                return pcRedisClientHelper;
            }
        }

        private static RedisClientHelper appRedisClientHelper = new RedisClientHelper("app");
        public static RedisClientHelper AppInstance
        {
            get
            {
                return appRedisClientHelper;
            }
        }

        private static RedisClientHelper loginPCRedisClientHelper = new RedisClientHelper("login-pc");
        public static RedisClientHelper LoginPCInstance
        {
            get
            {
                return loginPCRedisClientHelper;
            }
        }

        private static RedisClientHelper loginAppRedisClientHelper = new RedisClientHelper("login-app");
        public static RedisClientHelper LoginAppInstance
        {
            get
            {
                return loginAppRedisClientHelper;
            }
        }
    }
}
