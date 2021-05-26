using Autofac;
using XExten.Advance.AopFramework;
using XExten.Advance.LinqFramework;
using System;

namespace Test.AopTest
{
    public class TestClass
    {
        public void TestMethod()
        {

            var builder = new ContainerBuilder();
            var autofac = builder.Build();

            var xx = autofac.ResolveProxy<IMyInterface>().TestMethod(27);


            var ins1 = AopProxy.CreateProxyOfInherit<MyClass>();

            var ins2 = AopProxy.CreateProxyOfRealize<IMyInterface, MyClass>();

            var ins3 = AopProxy.CreateProxyOfInherit(typeof(MyClass));

            var ins4 = AopProxy.CreateProxyOfRealize(typeof(IMyInterface), typeof(MyClass));
        }


    }
}
