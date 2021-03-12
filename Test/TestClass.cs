using Synctool.AopFramework;
using System;
using Xunit;

namespace Test
{
    public class TestClass
    {
        [Fact]
        public void TestMethod()
        {
          var inst =  AopProxy.CreateProxyOfInherit<MyClass>();
            inst.TestMethod(27);
        }
    }
}
