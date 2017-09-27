using System;

namespace Novadaq.Core
{
    public static class FibonacciFinder
    {
        public static int GetAt(int n)
        {
            if (n < 1)
            {
                throw new ArgumentException("The position in the sequence should be greater than or equal 1");
            }
            if (n == 1)
            {
                return 1;
            }
            return n + GetAt(n - 1);
        }
    }
}
