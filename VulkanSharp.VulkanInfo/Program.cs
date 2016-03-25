using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulkanSharp.VulkanInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            InfoGenerator gen = new InfoGenerator(1, 0, 3);

            gen.DumpInfo(Console.OpenStandardOutput());

            Console.ReadKey();
        }
    }
}
