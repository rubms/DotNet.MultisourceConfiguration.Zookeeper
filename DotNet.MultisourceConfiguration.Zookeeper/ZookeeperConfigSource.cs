﻿using MultiSourceConfiguration.Config.ConfigSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace MultisourceConfiguration.Zookeeper
{
    public class ZookeeperConfigSource : IStringConfigSource
    {
        private readonly IZooKeeper _zooKeeper;
        private readonly string _basePath;

        /// <summary>
        /// Create a new instance of <see cref="ZookeeperConfigSource"/>, given a series
        /// of ZooKeeper endpoints (host:port) and a time out.
        /// </summary>
        /// <param name="zooKeeper">ZooKeeper client.</param>
        /// <param name="sessionTimeout">The ZooKeeper session timeout.</param>
        public ZookeeperConfigSource(IZooKeeper zooKeeper, string basePath)
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
                value = Encoding.UTF8.GetString(_zooKeeper.GetData(_basePath + "/" + property, false, null));
                return true;
            }
            catch(KeeperException ex)
            {
                if (ex.ErrorCode == KeeperException.Code.NONODE)
                {
                    value = null;
                    return false;
                }
                throw;
            }
        }
    }
}
