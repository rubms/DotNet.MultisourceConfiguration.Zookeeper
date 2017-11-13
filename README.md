# DotNet.MultisourceConfiguration.Zookeeper

[![NuGet Version](https://img.shields.io/nuget/v/DotNet.MultiSourceConfiguration.Zookeeper.svg?style=flat)](https://www.nuget.org/packages/DotNet.MultiSourceConfiguration.Zookeeper)

Zookeeper configuration source for the [DotNet.MultiSourceConfiguration](https://www.nuget.org/packages/DotNet.MultiSourceConfiguration) library, based on the [ZooKeeperNetEx](https://www.nuget.org/packages/ZooKeeperNetEx/) ZooKeeper client.

## How to use it

You can add ZookeeperConfigSource as a configuration source for your IConfigurationBuilder (from [DotNet.MultiSourceConfiguration](https://www.nuget.org/packages/DotNet.MultiSourceConfiguration)). You need to provide an instance of ZooKeeper (from [ZooKeeperNetEx](https://www.nuget.org/packages/ZooKeeperNetEx/)) to the ZookeeperConfigSource, for it to be able to connect ZooKeeper. In addition you need to provide a _base path_, which is the base path in ZooKeeper for the nodes containing your configuration properties.
```C#
    using MultisourceConfiguration;
    using MultisourceConfiguration.Zookeeper;
    using org.apache.zookeeper;
    
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            ZooKeeper zooKeeper = new ZooKeeper("localhost:2181", 30000, new ZooKeeperConfigWatcher());
            configurationBuilder.AddSources(new ZookeeperConfigSource(zooKeeper, "/path/to/config/in/zookeeper"));
            
            TestConfigurationDto configurationInterface = configurationBuilder.Build<TestConfigurationDto>();
            ...
        }
    }
    
    class ZooKeeperConfigWatcher: Watcher 
    {
        // This method will get invoked every time the ZooKeeper status or configuration values change.
        // This way you can dinamically react to changes in configuration.
        public Task process(WatchedEvent @event)
        {
			return new Task( () => {} );
        }
    }
```

Assuming that the test configuration DTO used in this example is like this:
```C#
    public class TestConfigurationDto
    {
        // By default properties are not required
        [Property("test.int.property")]
        public int? IntProperty { get; set; }
    }
```
when the configuration is built using `configurationBuilder.Build<TestConfigurationDto>()`, DotNet.MultisourceConfiguration.Zookeeper will target ZooKeeper (using the IZooKeeper client instance you provided to it), and will attempt to retrieve the data from a configuration node with path `/path/to/config/in/zookeeper/test.int.property`. **DotNet.MultisourceConfiguration.Zookeeper assumes that all the string configuration data in ZooKeeper is stored in UTF-8.**

## Bootstrapping the ZooKeeper Configuration

You will very probably want to read the ZooKeeper connection string (comma separated list of `host:port`) and the session timeout from configuration, instead of hard-coding it. You may read that configuration from your IConfigurationBuilder, taking advantage of other configuration sources, like `AppSettingsSource`, `EnvironmentVariableSource` or `CommandLineSource`.
```C#
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddSources(
                new AppSettingsSource(), new EnvironmentVariableSource(), new CommandLineSource(args));
        
        // MyZooKeeperConfig must be a class you define that will hold the configuration for ZooKeeper.
        MyZooKeeperConfig myZooKeeperConfig = configurationBuilder.Build<MyZooKeeperConfig>();

        ZooKeeper zooKeeper = new ZooKeeper(myZooKeeperConfig.ConnectionString, myZooKeeperConfig.SessionTimeOut.Value, new ZooKeeperConfigWatcher());
        configurationBuilder.AddSources(new ZookeeperConfigSource(zooKeeper, "/path/to/config/in/zookeeper"));
```


