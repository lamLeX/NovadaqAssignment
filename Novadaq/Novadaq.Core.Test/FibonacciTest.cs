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

        private static TheoryData<long, long> GetFibonacciSequence()
        {
            return new TheoryData<long, long>()
            {
                {1, 1},
                {2, 1},
                {3, 2},
                {4, 3},
                {5, 5},
                {6, 8},
                {50, 12_586_269_025}
            };
        }

        [Theory]
        [MemberData(nameof(GetFibonacciSequence))]
        public void GetFibonacciAtAPositionTest(long n, long expectedValue)
        {
            //Assign
            var input = n;
            var expected = expectedValue;
            //Act
            var resultTest = FibonacciFinder.GetAt(input);
            //Assert
            Assert.Equal(resultTest, expected);
        }
    }
}
