using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Helpers
{
    /// <summary>
    /// Useful extension methods for the ACN library.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Splits and enumerable into chunks that are specified by the chunk size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <returns>The chunks.</returns>
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
