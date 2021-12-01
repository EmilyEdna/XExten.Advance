using XExten.Advance.AopFramework.AopAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.AopTest
{
    public class MyActionAttribute: AopBaseActionAttribute
    {
        public MyActionAttribute() : base()
        {

        }
        public MyActionAttribute(string Code):base(Code)
        {
        }
        public override void Before(string methodName, string classInfo,  object[] parameters)
        {
            Console.WriteLine(methodName);
            Console.WriteLine(string.Join(",", parameters));
        }
        public override object After(string methodName, string classInfo,  object result)
        {
            Console.WriteLine(methodName);
            Console.WriteLine(result);
            return result;
        }
    }
}
