using System;
using System.Net;
using System.Threading.Tasks;

namespace Retry.Client
{
    class Program
    {
        private static int two = 2;

        static void Main()
        {
            Test().Wait();

            Console.Read();
        }

        private static async Task Test()
        {
            int counter = 0;

            await NET.RetryHandler.RetryAsync<HttpListenerException>(async () => { counter = await CalculateAsync(); }, 3, 200);
        }

        private static int Calculate()
        {
            throw new HttpListenerException(two++);
        }

        private static async Task<int> CalculateAsync()
        {
            await Task.FromResult(3);

            throw new HttpListenerException(two++);
        }
    }
}
