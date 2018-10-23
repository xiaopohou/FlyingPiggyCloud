using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace 测试单元
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 4 * 1024 * 1024;
            int b = 7 * 1024 * 1024;
            Console.WriteLine((a + b - 1) / a);
            Console.ReadLine();
        }
    }
}
