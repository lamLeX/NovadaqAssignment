using System;
using System.Threading;

namespace Novadaq.Core
{
    public static class FibonacciFinder
    {
        public static long GetAt(long n, CancellationToken ct)
        {
            if (n < 1)
            {
                throw new ArgumentException("The position in the sequence should be greater than or equal 1");
            }
            if (n < 3)
            {
                return 1;
            }

            //Stop the recursive if cancelation requested
            ct.ThrowIfCancellationRequested();

            return GetAt(n - 1, ct) + GetAt(n - 2, ct);
        }

        public static long GetAt(long n)
        {
            return GetAt(n, CancellationToken.None);
        }
    }
}
