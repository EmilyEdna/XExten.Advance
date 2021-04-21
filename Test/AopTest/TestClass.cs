using Autofac;
using Synctool.AopFramework;
using Synctool.LinqFramework;
using System;
using Xunit;

namespace Test.AopTest
{
    public class TestClass
    {
        [Fact]
        public void TestMethod()
        {

            var builder = new ContainerBuilder();
            var autofac = builder.Build();

          var xx =  autofac.ResolveProxy<IMyInterface>().TestMethod(27);


            var ins1 = AopProxy.CreateProxyOfInherit<MyClass>();
            Assert.True(ins1.TestMethod(27) == "27");

            var ins2 = AopProxy.CreateProxyOfRealize<IMyInterface, MyClass>();
            Assert.True(ins2.TestMethod(27) == "27");

            var ins3 = AopProxy.CreateProxyOfInherit(typeof(MyClass));
            Assert.True(((MyClass)ins3).TestMethod(27) == "27");

            var ins4 = AopProxy.CreateProxyOfRealize(typeof(IMyInterface), typeof(MyClass));
            Assert.True(((IMyInterface)ins4).TestMethod(27) == "27");
        }


    }
}
