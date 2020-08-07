using LXProtocols.Acn.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LXProtocols.TestTools
{
    /// <summary>
    /// Testing class used to listen in on ACN streams and work out if the right thing is sending.
    /// </summary>
    public class AcnListener : IDisposable
    {
        StreamingAcnSocket acnSocket;
        HashSet<Guid> senders = new HashSet<Guid>();
        Dictionary<Tuple<Guid, int>, byte[]> dmxValues = new Dictionary<Tuple<Guid, int>, byte[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AcnListener" /> class.
        /// </summary>
        /// <param name="universes">The universes to listen to.</param>
        /// <param name="ip">The ip.</param>
        public AcnListener(IEnumerable<int> universes, IPAddress ip)
        {
            Initialise(universes, ip);
        }

        /// <summary>
        /// Create without initalisation a new instance of the <see cref="AcnListener"/> class.
        /// </summary>
        protected AcnListener()
        {
        }

        /// <summary>
        /// Initialises the specified universes.
        /// </summary>
        /// <param name="universes">The universes.</param>
        /// <param name="ip">The ip.</param>
        protected virtual void Initialise(IEnumerable<int> universes, IPAddress ip)
        {
            acnSocket = new StreamingAcnSocket(Guid.NewGuid(), "Unit test");
            acnSocket.NewPacket += (o, e) =>
            {
                lock (senders)
                {
                    senders.Add(e.Packet.Root.SenderId);
                    dmxValues[new Tuple<Guid, int>(e.Packet.Root.SenderId, e.Packet.Framing.Universe)] = e.Packet.Dmx.Data;
                }
            };
            acnSocket.Open(ip);
            foreach (int universe in universes)
            {
                acnSocket.JoinDmxUniverse(universe);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (acnSocket != null)
            {
                acnSocket.Dispose();
            }
        }

        /// <summary>
        /// Gets the last value recieved for the source, universe and channel
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="universe">The universe.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The value</returns>
        public byte Get(Guid source, int universe, int channel)
        {
            lock (SyncRoot)
            {
                Assert.IsTrue(Contains(source), "Expected ACN source {0} missing", source);
                return dmxValues[new Tuple<Guid, int>(source, universe)][channel];
            }
        }

        /// <summary>
        /// Determines whether we have heard from the specifed source since the last Clear (or Listen)
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public bool Contains(Guid source)
        {
            lock (senders)
            {
                return senders.Contains(source);
            }
        }

        /// <summary>
        /// Listen to incomming ACN for 2 seconds
        /// </summary>
        public void Listen()
        {
            Listen(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Listens to incomming ACN for specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        public void Listen(TimeSpan time)
        {
            Clear();
            Thread.Sleep(time);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (senders)
            {
                senders.Clear();
                dmxValues.Clear();
            }
        }

        /// <summary>
        /// Gets an object to lock out updates while you check
        /// </summary>
        /// <value>
        /// The synchronize root.
        /// </value>
        public object SyncRoot
        {
            get { return senders; }
        }
    }
}
