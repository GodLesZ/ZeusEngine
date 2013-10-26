using System;
using System.IO;
using NUnit.Framework;
using Zeus.Library.Scripting;

namespace Zeus.Library.Tests.ScriptCompilerTests {

    [TestFixture]
    public class CoreTest {

        [Test]
        public void TestBaseCompile() {
            var compiler = new ScriptCompiler("Zeus.Library.Tests.ScriptCompilerTests", "Output/Scripts.dll");
            compiler.SourceAssemblies.Add(Path.GetFullPath(Environment.CurrentDirectory + "/Scripts/TestScript.cs"));
            Assert.True(compiler.Compile(/*debug*/true, /*cache*/false));
        }
         
    }

}