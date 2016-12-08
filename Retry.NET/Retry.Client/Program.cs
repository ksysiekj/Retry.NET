using Retry.NET;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Client
{
    class Program
    {
        private static int two = 2;

        static void Main()
        {
            RetryHandler.RetryForAsync<HttpListenerException>(async () => await CalculateAsync(), 4500, 350).Wait();

            Console.Read();
        }

        private static int Calculate()
        {
            Thread.Sleep(625);
            throw new HttpListenerException(two++);
        }

        private static async Task<int> CalculateAsync()
        {
            Thread.Sleep(625);

            await Task.FromResult(3);

            return 3;

            //throw new HttpListenerException(2);
        }
    }
}
