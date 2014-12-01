using Acn;
using Acn.Helpers;
using Acn.Packets.sAcn;
using Acn.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GlitchDetector
{
    class Detector
    {
        private StreamingAcnSocket socket = new StreamingAcnSocket(Guid.NewGuid(), "Streaming Glitch Detector");

        public int Universe { get; set; }
        public int Channel { get; set; }
        public int Threshold { get; set; }

        public string IpAddress { get; set; }

        private DmxUniverse recieveData = null;

        void socket_NewPacket(object sender, NewPacketEventArgs<StreamingAcnDmxPacket> e)
        {
            StreamingAcnDmxPacket dmxPacket = e.Packet as StreamingAcnDmxPacket;
            if (dmxPacket != null)
            {
                recieveData.SetDmx(dmxPacket.Dmx.Data);
            }
        }
        Timer labelingTimer;
        DateTime startTime;

        public void Start()
        {
            Console.WriteLine("Start listening on {0} {1}.{2} for {3}ms pauses", IpAddress, Universe, Channel, Threshold);
            socket.Open(IPAddress.Parse(IpAddress));
            socket.NewPacket += socket_NewPacket;

            socket.JoinDmxUniverse(Universe);

            recieveData = new DmxUniverse(Universe);
            recieveData.DmxDataChanged += universe_DmxDataChanged;

            startTime = DateTime.UtcNow;
            labelingTimer = new Timer(new TimerCallback((o) =>
            {
                var now = DateTime.UtcNow;
                Console.WriteLine("{0}   {1}", now, now - startTime);
            }), null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(1));

            while(true)
            {
                Thread.Sleep(1000);
            }
        }

        byte? lastValue = null;
        Stopwatch timer = new Stopwatch();

        void universe_DmxDataChanged(object sender, EventArgs e)
        {
            byte? oldValue = lastValue;
            lastValue = recieveData.DmxData[Channel];

            if(oldValue == null)
            {
                timer.Start();
            }
            else
            {
                if (oldValue != lastValue)
                {
                    var time = timer.Elapsed;
                    timer.Restart();
                    if (time.TotalMilliseconds > Threshold)
                    {
                        Console.WriteLine("{3} Took {0} for {1} -> {2}", time.TotalMilliseconds, oldValue, lastValue, DateTime.UtcNow);
                    }
                }
            }
        }

    }
}
