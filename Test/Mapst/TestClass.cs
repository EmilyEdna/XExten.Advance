using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Mapst
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
            var data = t.Adapt<Traget>();
        }
    }

}
