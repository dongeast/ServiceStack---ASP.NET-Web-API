using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        private static readonly int _numberOfRequestsToSend = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start . . .");
            Console.WriteLine();
            Console.ReadKey();

            long webApi = 0;
            long serviceStack = 0;
            for (int i = 0; i < 1000; i++)
            {
                webApi += TestWebApi();
                serviceStack += TestServiceStack();
                Console.WriteLine("------------------------------------------------------------");
            }

            Console.WriteLine(string.Format("ServiceStack Total {0} ms", webApi));
            Console.WriteLine(string.Format("Web API      Total {0} ms", serviceStack));

            Console.WriteLine();
            Console.WriteLine("Press any key to exit . . .");
            Console.ReadKey();
        }

        private static long TestServiceStack()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _numberOfRequestsToSend; i++)
            {
                string output = string.Empty;
                using (var httpRequest = new xNet.Net.HttpRequest())
                {
                    xNet.Net.HttpResponse httpResponse = httpRequest.Get("http://localhost:1231/beans?format=json");
                    output = httpResponse.ToString();
                }
            }
            sw.Stop();
            Console.WriteLine(string.Format("ServiceStack {0} ms", sw.ElapsedMilliseconds).PadLeft(15));
            return sw.ElapsedMilliseconds;
        }

        private static long TestWebApi()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _numberOfRequestsToSend; i++)
            {
                string output = string.Empty;
                using (var httpRequest = new xNet.Net.HttpRequest())
                {
                    xNet.Net.HttpResponse httpResponse = httpRequest.Get("http://localhost:47503/api/Beans/GetAllProducts");
                    output = httpResponse.ToString();
                }
            }
            sw.Stop();
            Console.WriteLine(string.Format("Web API      {0} ms", sw.ElapsedMilliseconds).PadLeft(15));
            return sw.ElapsedMilliseconds;
        }
    }
}
