using System.Buffers;
using System.IO;

namespace LXProtocols.Acn
{
    /// <summary>
    /// A helper class to make getting and using rendted <c>byte[]</c>s easier.
    /// </summary>
    public static class ArrayPoolHelper
    {
        /// <summary>
        /// The array pool we use for the buffers used when writing packets.
        /// </summary>
        public static readonly ArrayPool<byte> BufferPool = ArrayPool<byte>.Shared;

        /// <summary>
        /// Creates a memory stream with backing buffer initialised of the specified size from the
        /// <see cref="BufferPool"/>.
        /// </summary>
        /// <param name="bufferLength">The length of the backing buffer.</param>
        /// <param name="buffer">The backing buffer. (returned)</param>
        /// <returns>
        /// A new <see cref="MemoryStream"/> instance whose backing buffer comes from the
        /// <see cref="BufferPool"/>.
        /// </returns>
        /// <remarks>
        /// You MUST ensure that once the returned buffer is longer needed that it is placed back
        /// into the <see cref="BufferPool"/> via <c>BufferPool.Return(buffer)</c>.
        /// </remarks>
        public static MemoryStream CreateMemoryStream(int bufferLength, out byte[] buffer)
        {
            buffer = BufferPool.Rent(bufferLength);
            return new MemoryStream(buffer, 0, buffer.Length, writable: true, publiclyVisible: true);
        }
    }
}
