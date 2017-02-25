using Retry.NET;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Retry.Client
{
    class Program
    {
        private static int two = 2;


        static void Main()
        {
            // cant use await here coz Main methos cannot be async so used Wait() method instead
            RetryHandler.RetryForAsync<WebException>(async () =>
             {
                 string content = await DownloadContentAsync();

                 Console.Write(content);

             }, 60000, 500, httpRequestException => httpRequestException.Status == WebExceptionStatus.Timeout
             || httpRequestException.Status == WebExceptionStatus.ConnectFailure).Wait();

            // cant use await here coz Main methos cannot be async so used Wait() method instead
            RetryHandler.RetryAsync<WebException>(async () =>
           {
               string content = await DownloadContentAsync();

               Console.Write(content);

           }, 5, 500, httpRequestException => httpRequestException.Status == WebExceptionStatus.Timeout
           || httpRequestException.Status == WebExceptionStatus.ConnectFailure).Wait();


            RetryHandler.Retry<WebException>(() =>
           {
               string content = DownloadContent();

               Console.Write(content);

           }, 6, 500, httpRequestException => httpRequestException.Status == WebExceptionStatus.Timeout
           || httpRequestException.Status == WebExceptionStatus.ConnectFailure, Logger);


            RetryHandler.RetryFor<WebException>(() =>
            {
                string content = DownloadContent();

                Console.Write(content);

            }, TimeSpan.FromSeconds(75), TimeSpan.FromSeconds(2), httpRequestException => httpRequestException.Status == WebExceptionStatus.Timeout
            || httpRequestException.Status == WebExceptionStatus.ConnectFailure, Logger);

            Console.Read();
        }

        private static void Logger(Exception exception)
        {
            // _logger.Error(exception);
        }

        private static async Task<string> DownloadContentAsync()
        {
            string page = "http://en.wikipedia.org/";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                return await content.ReadAsStringAsync();
            }
        }

        private static string DownloadContent()
        {
            string page = "http://en.wikipedia.org/";

            // do your stuff

            return page;
        }
    }
}
