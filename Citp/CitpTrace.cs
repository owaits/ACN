using Citp.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Citp
{
    public static class CitpTrace
    {
        private static TraceSource trace = new TraceSource("Citp");

        public static void RxPacket(IPEndPoint source, CitpPacket packet)
        {
            trace.TraceEvent(TraceEventType.Verbose, 1, string.Format("{0}: {1}",source.ToString(), packet.ToString()));
            trace.Flush();
        }

        public static void TxPacket(IPEndPoint target, CitpPacket packet)
        {
            trace.TraceEvent(TraceEventType.Verbose, 2,string.Format("{0}: {1}",target.ToString(), packet.ToString()));
            trace.Flush();
        }
    }
}
