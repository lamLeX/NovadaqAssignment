using System;

namespace Novadaq.Core
{
    public static class FibonacciFinder
    {
        public static long GetAt(long n)
        {
            if (n < 1)
            {
                throw new ArgumentException("The position in the sequence should be greater than or equal 1");
            }
            if (n < 3)
            {
                return 1;
            }
            return GetAt(n - 1) + GetAt(n - 2);
        }
    }
}
