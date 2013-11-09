using System;
using NUnit.Framework;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Yaml;

namespace Zeus.Library.Tests.YamlConfigurationTests {

    [TestFixture]
    public class CoreTest {

        [Test]
        public static void TestYamlDataTypesInExpandoString() {
            var conf = Factory.Create<Provider>("conf.yaml");
            var dynConf = conf.FirstAsExpando();

            Assert.IsInstanceOf<string>(dynConf.node_test_types.sub_string);
            Assert.True(dynConf.node_test_types.sub_string == "some string");
        }

        [Test]
        public static void TestYamlDataTypesInExpandoInteger() {
            var conf = Factory.Create<Provider>("conf.yaml");
            var dynConf = conf.FirstAsExpando();

            Assert.IsInstanceOf<int>(dynConf.node_test_types.sub_int);
            Assert.True(dynConf.node_test_types.sub_int == 1337);
        }

        [Test]
        public static void TestYamlDataTypesInExpandoFloat() {
            var conf = Factory.Create<Provider>("conf.yaml");
            var dynConf = conf.FirstAsExpando();

            Assert.IsInstanceOf<float>(dynConf.node_test_types.sub_float);
            Assert.True(((float)dynConf.node_test_types.sub_float).Equals (8.15f));
        }

    }

}