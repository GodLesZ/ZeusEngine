using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Xml;

namespace Zeus.Library.Tests.XmlConfigurationTests {

    [TestFixture]
    public class CoreTest {

        [Test]
        public static void TestCorrectXmlStructure() {
            var conf = Factory.Create<Provider>("conf.xml");
            var dyncConf = conf.AsExpando().configuration;

            var node_category = dyncConf.category[0];
            Assert.True(node_category.attr == "test");
            Assert.True(node_category.settings.node == "some value");
            Assert.True(node_category.settings.foo == "value2");
            Assert.True(node_category.other.data == "1");
        }

    }

}
