using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net;
using System.Web;

namespace Retry.NET.Tests
{
    [TestFixture]
    public class RetryHandlerTests
    {
        private const int MilisecondsDelay = 250;
        private const int MaxAttempts = 3;

        [Test]
        public void Retry_MaxAttemptsArgumentIsNegative_ThrowsArgumentOutOfRangeException()
        {
            Action action = () => { RetryHandler.Retry<Exception>(() => { }, -4, MilisecondsDelay); };
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Retry_MaxAttemptsArgumentEqualsZero_ThrowsArgumentOutOfRangeException()
        {
            Action action = () => { RetryHandler.Retry<Exception>(() => { }, 0, MilisecondsDelay); };
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Retry_ActionArgumentIsNull_ThrowsArgumentOutOfRangeException()
        {
            Action action = () => { RetryHandler.Retry<Exception>(null, MaxAttempts, MilisecondsDelay); };
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Retry_ActionDoesntThrowAnyException_ThrowsArgumentOutOfRangeException()
        {
            Action action = () => { RetryHandler.Retry<HttpException>(() => { }, MaxAttempts, MilisecondsDelay); };
            action.ShouldNotThrow<HttpException>();
        }

        [Test]
        public void Retry_ExpectedToCatchHttpListenerException_ActionThrowsHttpException_ThrowsHttpException()
        {
            Action action = () => { RetryHandler.Retry<HttpListenerException>(() => { throw new HttpException(); }, MaxAttempts, MilisecondsDelay); };
            action.ShouldThrow<HttpException>();
        }

        [Test]
        public void Retry_ActionAlwaysThrowsHttpException_MaxAttemptExceedsAllowedMaxAttemptArgument_ThrowsHttpException()
        {
            Action action = () => { RetryHandler.Retry<HttpException>(() => { throw new HttpException(); }, MaxAttempts, MilisecondsDelay); };
            action.ShouldThrow<HttpException>();
        }

        [Test]
        public void Retry_ActionThrowsHttpException_ShouldRetryArgumentReturnsFalse_ThrowsHttpException()
        {
            Action action = () =>
            {
                RetryHandler.Retry<HttpException>(() => { throw new HttpException((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString()); }, MaxAttempts, MilisecondsDelay,
                    (ex) => ex.GetHttpCode() == (int)HttpStatusCode.ServiceUnavailable);
            };
            action.ShouldThrow<HttpException>().Which.GetHttpCode().Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
