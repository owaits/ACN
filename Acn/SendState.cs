using System.Net.Sockets;

namespace LXProtocols.Acn
{
    /// <summary>
    /// A simple state class for sent packets.
    /// </summary>
    /// <remarks>
    /// Used so we can return the buffer to the <see cref="ArrayPoolHelper.BufferPool"/> after use.
    /// </remarks>
    public sealed class SendState
    {
        public Socket Socket;
        public byte[] Buffer;
    }
}
