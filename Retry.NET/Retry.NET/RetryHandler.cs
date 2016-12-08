using Retry.NET.Validation;
using System;
using System.Threading.Tasks;

namespace Retry.NET
{
    public static class RetryHandler
    {
        private const int ZERO = 0;

        public static void Retry<TException>(Action action, int maxAttempts, int millisecondsDelay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Assert.Positive(millisecondsDelay, nameof(millisecondsDelay));

            Retry<TException>(action, maxAttempts, TimeSpan.FromMilliseconds(millisecondsDelay), shouldRetry, exceptionLogger);
        }

        public static void RetryForever<TException>(Action action, int maxAttempts, int millisecondsDelay,
          Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Retry<TException>(action, Int32.MaxValue, TimeSpan.FromMilliseconds(millisecondsDelay), shouldRetry, exceptionLogger);
        }

        public static void RetryForever<TException>(Action action, int maxAttempts, TimeSpan delay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
           where TException : Exception
        {
            Retry<TException>(action, Int32.MaxValue, delay, shouldRetry, exceptionLogger);
        }

        public static void Retry<TException>(Action action, int maxAttempts, TimeSpan delay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {

            Assert.Positive(maxAttempts, nameof(maxAttempts));
            Assert.NotNull(action, nameof(action));

            bool shouldRetryNotNull = shouldRetry != null;

            for (;;) // while (true)
            {
                try
                {
                    action.Invoke();
                    break;
                }
                catch (TException exception)
                {
                    if ((shouldRetryNotNull && !shouldRetry(exception)) || ZERO == (--maxAttempts))
                    {
                        throw;
                    }

                    exceptionLogger?.Invoke(exception);

                    Task.Delay(delay).Wait();
                }
            }
        }

        public static async Task RetryAsync<TException>(Func<Task> operation, int maxAttempts, int millisecondsDelay,
            Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Assert.Positive(millisecondsDelay, nameof(millisecondsDelay));

            await RetryAsync<TException>(operation, maxAttempts, TimeSpan.FromMilliseconds(millisecondsDelay),
                      shouldRetry, exceptionLogger);
        }

        public static async Task RetryForeverAsync<TException>(Func<Task> operation, int millisecondsDelay,
          Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
          where TException : Exception
        {
            Assert.Positive(millisecondsDelay, nameof(millisecondsDelay));

            await RetryAsync<TException>(operation, int.MaxValue, TimeSpan.FromMilliseconds(millisecondsDelay),
                      shouldRetry, exceptionLogger);
        }

        public static async Task RetryForeverAsync<TException>(Func<Task> operation, TimeSpan delay,
            Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            await RetryAsync<TException>(operation, int.MaxValue, delay, shouldRetry, exceptionLogger);
        }

        public static async Task RetryAsync<TException>(Func<Task> operation, int maxAttempts, TimeSpan delay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
           where TException : Exception
        {

            Assert.Positive(maxAttempts, nameof(maxAttempts));
            Assert.NotNull(operation, nameof(operation));

            bool shouldRetryNotNull = shouldRetry != null;

            for (;;)// while (true)
            {
                try
                {
                    await operation();
                    break;
                }
                catch (TException exception)
                {
                    if ((shouldRetryNotNull && !shouldRetry(exception)) || ZERO == (--maxAttempts))
                    {
                        throw;
                    }

                    exceptionLogger?.Invoke(exception);

                    await Task.Delay(delay);
                }
            }
        }


        public static void RetryFor<TException>(Action action, TimeSpan duration, TimeSpan delay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Assert.NotNull(action, nameof(action));

            bool shouldRetryNotNull = shouldRetry != null;
            double durationTotalMs = duration.TotalMilliseconds;
            DateTime startDateTime = DateTime.Now;

            do
            {
                try
                {
                    action.Invoke();
                    break;
                }
                catch (TException exception)
                {
                    if ((shouldRetryNotNull && !shouldRetry(exception)) ||
                        (DateTime.Now - startDateTime).TotalMilliseconds > durationTotalMs)
                    {
                        throw;
                    }

                    exceptionLogger?.Invoke(exception);

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }


        public static void RetryFor<TException>(Action action, int totalMilliseconds, int millisecondsDelay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Assert.Positive(totalMilliseconds, nameof(totalMilliseconds));
            Assert.Positive(totalMilliseconds, nameof(millisecondsDelay));

            RetryFor(action, TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(millisecondsDelay),
                shouldRetry, exceptionLogger);
        }

        public static async Task RetryForAsync<TException>(Func<Task> operation, TimeSpan duration, TimeSpan delay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Assert.NotNull(operation, nameof(operation));

            bool shouldRetryNotNull = shouldRetry != null;
            double durationTotalMs = duration.TotalMilliseconds;
            DateTime startDateTime = DateTime.Now;

            do
            {
                try
                {
                    await operation();
                    break;
                }
                catch (TException exception)
                {
                    if ((shouldRetryNotNull && !shouldRetry(exception)) ||
                        (DateTime.Now - startDateTime).TotalMilliseconds > durationTotalMs)
                    {
                        throw;
                    }

                    exceptionLogger?.Invoke(exception);

                    await Task.Delay(delay);
                }
            } while (true);
        }

        public static async Task RetryForAsync<TException>(Func<Task> operation, int totalMilliseconds, int millisecondsDelay,
           Func<TException, bool> shouldRetry = null, Action<TException> exceptionLogger = null)
            where TException : Exception
        {
            Assert.Positive(totalMilliseconds, nameof(totalMilliseconds));
            Assert.Positive(totalMilliseconds, nameof(millisecondsDelay));

            await RetryForAsync(operation, TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(millisecondsDelay),
                  shouldRetry, exceptionLogger);
        }
    }
}
