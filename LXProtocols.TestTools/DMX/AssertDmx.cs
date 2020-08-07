using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using LXProtocols.Acn.Sockets;
using LXProtocols.Acn.Packets.sAcn;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LXProtocols.TestTools
{
    public static class AssertDmx
    {
        public static Guid TestSenderId = Guid.Parse("{A4702338-E564-45FA-BDE3-7C3795B3A6C5}");

        #region Recording

        private static List<StreamingAcnDmxPacket> recordedPackets = null;

        public static void Record(Guid senderId, List<int> universes)
        {
            try 
	        {	        
	            recordedPackets = new List<StreamingAcnDmxPacket>();
                ProcessDmxInput(senderId, universes, 2500, (packet) => { recordedPackets.Add(packet); });
	        }
	        catch (TimeoutException)
	        {
		        //This will ocurr if no DMX is on any of the universe.
	        }
        }

        public static void Stop()
        {
            recordedPackets = null;
        }

        public static bool IsUsingRecording
        {
            get { return recordedPackets != null; }
        }

        private static void ProcessRecordedInput(Guid senderId, List<int> universes, Action<StreamingAcnDmxPacket> handler)
        {
            foreach (StreamingAcnDmxPacket packet in recordedPackets)
            {
                if ((senderId == Guid.Empty || senderId == packet.Root.SenderId) && universes.Contains(packet.Framing.Universe))
                {
                    handler(packet);
                }
            }
        }

        #endregion


        public static bool IsActive(int universe)
        {
            bool active = false;
            try
            {
                ProcessInput(Guid.Empty, universe, 40, (packet) => { active = true; });
            }
            catch (TimeoutException)
            {
                return false;
            }

            return active;
        }

        public static byte[] CaptureDmx(int universe)
        {
            byte[] capturedDmx = null;
            ProcessInput(Guid.Empty,universe, 2500 ,(packet) => {
                capturedDmx = packet.Dmx.Data;
            });

            return capturedDmx;
        }



        public static void LevelEqual(int universe, int address, byte level)
        {
            ProcessInput(Guid.Empty,universe, 2500, (packet) =>
            {
                Assert.AreEqual(level, packet.Dmx.Data[address], string.Format("DMX levels do not match for universe {0} address {1}.", universe,address));
            });
        }


        public static void UniverseEqual(int universe, params byte[] levels)
        {
            ProcessInput(Guid.Empty, universe, 2500 ,(packet) => {
                Assert.IsTrue(packet.Dmx.Data.SequenceEqual(levels),string.Format("DMX values do not match for universe {0}.",universe));
            });
        }


        public static void UniverseAsHexEqual(int universe, string hexLevels)
        {
            byte[] data = hexLevels.Split(' ').Select<string, byte>(item => byte.Parse(item, NumberStyles.AllowHexSpecifier)).ToArray();
            UniverseEqual(universe, data);
        }

        private static void ProcessInput(Guid senderId, int universe, int timeout, Action<StreamingAcnDmxPacket> handler)
        {
            ProcessInput(senderId, new List<int>() { universe },timeout,handler);
        }

        private static void ProcessInput(Guid senderId, List<int> universes, int timeout, Action<StreamingAcnDmxPacket> handler)
        {
            if (IsUsingRecording)
                ProcessRecordedInput(senderId, universes, handler);
            else
                ProcessDmxInput(senderId, universes, timeout, handler);
        }

        private static void ProcessDmxInput(Guid senderId, List<int> universes, int timeout, Action<StreamingAcnDmxPacket> handler)
        {
            ManualResetEvent waitForDMX = new ManualResetEvent(false);
            Exception socketException = null;

            StreamingAcnSocket acnSocket = new StreamingAcnSocket(Guid.NewGuid(), "DMX Assert");
            try
            {
                acnSocket.UnhandledException += (object sender, UnhandledExceptionEventArgs e)
                =>
                {
                    socketException = (Exception) e.ExceptionObject;
                    waitForDMX.Set();
                };

                acnSocket.NewPacket += (object sender, NewPacketEventArgs<Acn.Packets.sAcn.StreamingAcnDmxPacket> e) =>
                {
                    if((senderId == Guid.Empty || senderId == e.Packet.Root.SenderId) &&
                        (universes.Contains(e.Packet.Framing.Universe)))
                    {
                        try
                        {
                            handler(e.Packet);

                            universes.Remove(e.Packet.Framing.Universe);
                            if(universes.Count == 0)
                                waitForDMX.Set();
                        }
                        finally
                        {
                            if(universes.Count == 0)
                                acnSocket.Close();
                        }                       
                    }                
                };
                acnSocket.Open(IPAddress.Any);
                foreach(int universe in universes)
                    acnSocket.JoinDmxUniverse(universe);

                if (!waitForDMX.WaitOne(timeout))
                    throw new TimeoutException(string.Format("A timeout ocurred whil waiting for DMX on universe {0}.", string.Join(",",universes)));

                if (socketException != null)
                    throw socketException;
            }
            finally
            {
                acnSocket.Dispose();
            }
        }
    }
}
