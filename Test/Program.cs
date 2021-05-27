using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Test.HttpTest;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytes = new TestClass().TestMethond();
            Console.WriteLine(bytes);

        }
    }
}
