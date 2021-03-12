using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [Interceptor]
    public class MyClass
    {
        [MyAction]
        public virtual string TestMethod(int age)
        {
            return age.ToString();
        }
    }
}
