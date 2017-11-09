using Moq;
using NUnit.Framework;
using ZooKeeperNet;

namespace MultisourceConfiguration.Zookeeper.Tests
{
    [TestFixture]
    public class ZookeeperConfigSourceTests
    {
        [Test]
        public void SuccessfullyGetConfigurationProperty()
        {
            string testBasePath = "test/base/path/";
            Mock<IZooKeeper> zooKeeperMock = new Mock<IZooKeeper>();
            ZookeeperConfigSource configSource = new ZookeeperConfigSource(zooKeeperMock.Object, testBasePath);

            string testPropertyName = "testProperty";
            string testPropertyValue = "testValue";

            zooKeeperMock.Setup(x => x.GetData(testBasePath + testPropertyName, true, null))
                .Returns(System.Text.Encoding.UTF8.GetBytes(testPropertyValue));

            string obtainedPropertyValue;
            Assert.IsTrue(configSource.TryGetString("testProperty", out obtainedPropertyValue));
            Assert.AreEqual(testPropertyValue, obtainedPropertyValue);
        }

        [Test]
        public void SuccessfullyGetConfigurationPropertyWithoutFinalSlash()
        {
            string testBasePath = "test/base/path";
            Mock<IZooKeeper> zooKeeperMock = new Mock<IZooKeeper>();
            ZookeeperConfigSource configSource = new ZookeeperConfigSource(zooKeeperMock.Object, testBasePath);

            string testPropertyName = "testProperty";
            string testPropertyValue = "testValue";

            zooKeeperMock.Setup(x => x.GetData(testBasePath + "/" + testPropertyName, true, null))
                .Returns(System.Text.Encoding.UTF8.GetBytes(testPropertyValue));

            string obtainedPropertyValue;
            Assert.IsTrue(configSource.TryGetString("testProperty", out obtainedPropertyValue));
            Assert.AreEqual(testPropertyValue, obtainedPropertyValue);
        }

        [Test]
        public void NotFoundPropertyReturnsFalse()
        {
            string testBasePath = "test/base/path/";
            Mock<IZooKeeper> zooKeeperMock = new Mock<IZooKeeper>();
            ZookeeperConfigSource configSource = new ZookeeperConfigSource(zooKeeperMock.Object, testBasePath);

            string testPropertyName = "testProperty";

            zooKeeperMock.Setup(x => x.GetData(testBasePath + testPropertyName, true, null))
                .Throws(new KeeperException.NoNodeException());

            string obtainedPropertyValue;
            Assert.IsFalse(configSource.TryGetString("testProperty", out obtainedPropertyValue));
        }
    }
}
