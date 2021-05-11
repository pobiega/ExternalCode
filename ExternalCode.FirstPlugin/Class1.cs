using ExternalCode.Lib;
using System;

namespace ExternalCode.FirstPlugin
{
    public class FirstPluginOne : IExternalCode
    {
        public void Run(string data)
        {
            Console.WriteLine($"Hello from an externally provided dll! Here is your data: {data}");
        }
    }
}
