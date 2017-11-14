using MultiSourceConfiguration.Config.ConfigSource;
using org.apache.zookeeper;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MultisourceConfiguration.Zookeeper
{
    public class ZookeeperConfigSource : IStringConfigSource
    {
        private readonly ZooKeeper _zooKeeper;
        private readonly string _basePath;

        /// <summary>
        /// Create a new instance of <see cref="ZookeeperConfigSource"/>, given a series
        /// of ZooKeeper endpoints (host:port) and a time out.
        /// </summary>
        /// <param name="zooKeeper">ZooKeeper client.</param>
        /// <param name="sessionTimeout">The ZooKeeper session timeout.</param>
        public ZookeeperConfigSource(ZooKeeper zooKeeper, string basePath)
        {
            if (zooKeeper == null)
                throw new ArgumentNullException("zooKeeper");
            if (basePath == null)
                throw new ArgumentNullException("basePath");

            _zooKeeper = zooKeeper;
            _basePath = basePath;
            if (_basePath.EndsWith("/"))
            {
                _basePath = _basePath.Remove(_basePath.LastIndexOf("/"), 1);
            }
        }

        public bool TryGetString(string property, out string value)
        {
            try
            {
                var dataTask = _zooKeeper.getDataAsync(_basePath + "/" + property, true);
                dataTask.Wait();
                byte[] data = dataTask.Result.Data;
                value = Encoding.UTF8.GetString(data);
                return true;
            }
            catch(AggregateException ex)
            {
                KeeperException keeperException = ex.InnerException as KeeperException;
                if (keeperException != null && keeperException.getCode() == KeeperException.Code.NONODE)
                {
                    value = null;
                    return false;
                }
                throw;
            }
        }
    }
}
