using LXProtocols.Acn;
using LXProtocols.Acn.Helpers;
using LXProtocols.Acn.Packets.sAcn;
using LXProtocols.Acn.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private int packetCount = 0;
        private int errorCount = 0;

        public int Universe { get; set; }
        public int Channel { get; set; }
        public int Threshold { get; set; }

        /// <summary>
        /// Gets or sets whether each recieved DMX frame is printed to the trace output.
        /// </summary>
        public bool PrintTrace { get; set; }

        /// <summary>
        /// Gets or sets the path to a log file where the results will be written to.
        /// </summary>
        /// <remarks>
        /// The default is to write the output to the console, if a log file is specified the results will be written to that file too.
        /// </remarks>
        public string LogFile { get; set; }

        /// <summary>
        /// Gets or sets the IP Address of the local adapter to listen for DMX on.
        /// </summary>
        /// <remarks>
        /// If no adapter is specified the first suitable adapter will be used. You only need to set this if multiple adapters are active on the system.
        /// </remarks>
        public IPAddress IPAddress { get; set; } = IPAddress.Any;

        private DmxUniverse recieveData = null;

        /// <summary>
        /// Writes the results and messages to the trace output.
        /// </summary>
        /// <remarks>
        /// If a log file is specified the this will write to that file in addition to the console output.
        /// </remarks>
        /// <param name="message">The message to write to the trace output.</param>
        /// <param name="args">The arguments to add into the trace message.</param>
        protected void WriteTrace(string message, params object[] args)
        {
            string text = string.Format(message, args);

            Console.WriteLine(text);

            if(!string.IsNullOrEmpty(LogFile))
            {
                File.AppendAllText(LogFile, text + "\n");
            }
        }

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
            WriteTrace("Start listening on {0} {1}.{2} for {3}ms pauses", IPAddress, Universe, Channel, Threshold);
            socket.Open(IPAddress);
            socket.NewPacket += socket_NewPacket;

            socket.JoinDmxUniverse(Universe);

            recieveData = new DmxUniverse(Universe);
            recieveData.DmxDataChanged += universe_DmxDataChanged;

            startTime = DateTime.UtcNow;
            labelingTimer = new Timer(new TimerCallback((o) =>
            {
                var now = DateTime.UtcNow;
                WriteTrace("Still Alive at {0}  Run For {1} Checked {2} packets {3} errors", now, now - startTime,packetCount,errorCount);
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
            packetCount++;
            byte? oldValue = lastValue;
            lastValue = recieveData.DmxData[Channel];

            if (PrintTrace)
            {
                WriteTrace("[{0}.{1}] {2}", Universe, Channel, lastValue);
            }

            if (oldValue == null)
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
                        errorCount++;
                        WriteTrace("{3} Took {0} for {1} -> {2}", time.TotalMilliseconds, oldValue, lastValue, DateTime.UtcNow);
                    }
                }
            }
        }

    }
}
