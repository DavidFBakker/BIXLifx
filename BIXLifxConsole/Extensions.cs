using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIXLifxConsole
{
    public static class Extensions
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> action)
        {
            return Task.WhenAll(sequence.Select(action));
        }
    }
}