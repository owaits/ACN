using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;
using System.Threading;
using Acn.Helpers;

namespace Acn
{
    public class DmxStreamer:IDisposable
    {
        private StreamingAcnSocket socket = null;
        private Thread dmxThread = null;
        private Dictionary<int, DmxUniverse> universes = new Dictionary<int, DmxUniverse>();

        public DmxStreamer(StreamingAcnSocket socket)
        {
            this.socket = socket;
            Streaming = false;
        }

        public bool Streaming { get; protected set; }

        private int priority = 100;

        /// <summary>
        /// Gets or sets the priority of the sACN DMX stream.
        /// </summary>
        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        private TimeSpan synchonizationLatency = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the time to wait after all DMX packets have been sent and the sync packet is transmitted.
        /// </summary>
        /// <remarks>
        /// You would normally want this to be set to zero but adding in latency to the sync packet can improve synchronisation
        /// with the expense of latency.
        /// </remarks>
        public TimeSpan SynchonizationLatency
        {
            get { return synchonizationLatency; }
            set { synchonizationLatency = value; }
        }


        private TimeSpan discoveryInterval = new TimeSpan(0, 0, 10);

        /// <summary>
        /// Gets or sets the discovery interval between discovery messages being sent.
        /// </summary>
        /// <remarks>
        /// This is defined as 10s by the protocol but may be configurable. You can set this to
        /// zero to disable the discovery message.
        /// </remarks>
        public TimeSpan DiscoveryInterval
        {
            get { return discoveryInterval; }
            set { discoveryInterval = value; }
        }

        public void AddUniverse(DmxUniverse universe)
        {
            lock (universes)
            {
                if (universes.ContainsKey(universe.Universe))
                    throw new InvalidOperationException("This universe is already being streamed.");

                universes.Add(universe.Universe, universe);
            }
        }

        public void RemoveUniverse(int universe)
        {
            lock (universes)
            {
                universes.Remove(universe);
            }
        }

        public void Start()
        {
            if (dmxThread != null)
                throw new InvalidOperationException("DMX output has already started!");

            if (universes.Count == 0)
                throw new InvalidOperationException("There are no DMX universes to stream. Please add a universe first.");

            Streaming = true;
            dmxThread = new Thread(new ThreadStart(SendDmx));
            dmxThread.Start();
        }

        public void Stop()
        {
            if (dmxThread != null)
            {
                Streaming = false;
                dmxThread.Join();
                dmxThread = null;
            }
        }

        private void SendDmx()
        {
            while (Streaming)
            {
                foreach (DmxUniverse universe in universes.Values)
                {
                    if (universe.AliveTime > 0 || universe.KeepAliveTime > 36)
                    {
                        socket.SendDmx(universe.Universe, universe.DmxData, (byte) Priority);

                        universe.KeepAliveTime = 0;
                        universe.AliveTime = Math.Max(0, universe.AliveTime-1);
                    }
                    else
                    {
                        universe.KeepAliveTime++;
                    }
                }

                //If we are in sync mode then wait for the sync delay and then send a sync message.
                if(socket.SynchronizationAddress > 0)
                {
                    Thread.Sleep(SynchonizationLatency);
                    socket.SendSynchronize();
                }

                //At the discovery interval, periodically send a discovery message so clients know what universes we are sending.
                if (DiscoveryInterval != TimeSpan.Zero && DateTime.Now.Subtract(lastDiscovery) > DiscoveryInterval)
                {
                    SendDiscovery();
                }

                Thread.Sleep(25);
            }
        }


        private DateTime lastDiscovery = DateTime.MinValue;

        /// <summary>
        /// Sends the discovery message to listening clients.
        /// </summary>
        /// <remarks>
        /// This is periodically sent at DiscoveryInterval but if the universes change you may wish
        /// to force an update.
        /// </remarks>
        public void SendDiscovery()
        {
            socket.SendDiscovery(universes.Keys);
            lastDiscovery = DateTime.Now;
        }



        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
