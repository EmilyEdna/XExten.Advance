using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace Test.MapperTest
{
    public class TestClass
    {

        public void TestList()
        {
            List<Source> ls = new List<Source>();
            ls.Add(new Source
            {
                Name = "lz",
                Pwd = "lz"
            });

            var data = ls.Adapt<List<Traget>>();
        }
        public void Test()
        {
            var t = new Source
            {
                Name = "lz",
                Pwd = "lz"
            };
            var data = t.ToMapper<Traget>();
        }
    }

}
