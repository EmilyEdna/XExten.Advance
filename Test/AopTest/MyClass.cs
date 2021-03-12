using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.AopTest
{
    [Interceptor]
    public class MyClass: IMyInterface
    {
        [MyAction]
        public virtual string TestMethod(int age)
        {
            return age.ToString();
        }
    }
    public interface IMyInterface
    {
        string TestMethod(int age);
    }
}
