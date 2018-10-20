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
            string[] a = { "hello", "hi" };
            object[] b = { "how are you", a };
            Console.WriteLine(JsonConvert.SerializeObject(b));
            Console.ReadLine();
        }
    }
}
