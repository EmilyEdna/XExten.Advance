using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Test.HttpTest;
using XExten.Advance.StaticFramework;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
           var html = new TestClass().TestMethod();
          var  x =  SyncStatic.HText(html);
        }
    }
}
