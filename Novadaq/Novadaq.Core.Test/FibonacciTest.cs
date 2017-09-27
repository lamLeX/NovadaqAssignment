using System;
using Xunit;

namespace Novadaq.Core.Test
{
    public class FibonacciTest
    {

        [Fact]
        public void UserEnter0ShouldThrowExceptionTest()
        {
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                FibonacciFinder.GetAt(0);
            });
        }
    }
}
