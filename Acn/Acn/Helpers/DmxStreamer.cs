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
                        socket.SendDmx(universe.Universe, universe.DmxData);

                        universe.KeepAliveTime = 0;
                        universe.AliveTime = Math.Max(0, universe.AliveTime-1);
                    }
                    else
                    {
                        universe.KeepAliveTime++;
                    }
                }
                Thread.Sleep(25);
            }
        }



        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
