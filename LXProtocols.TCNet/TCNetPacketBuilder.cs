using LXProtocols.TCNet.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// Creates packets from the header information.
    /// </summary>
    public class TCNetPacketBuilder
    { 
        private static Dictionary<MessageTypes, Func<TCNetPacket>> registeredPackets = new Dictionary<MessageTypes, Func<TCNetPacket>>();
        private static Dictionary<DataTypes, Func<TCNetPacket>> registeredDataPackets = new Dictionary<DataTypes, Func<TCNetPacket>>();
        private static Dictionary<uint, Func<TCNetPacket>> registeredApplicationPackets = new Dictionary<uint, Func<TCNetPacket>>();

        /// <summary>
        /// Register a TCNet packet type to be built and created by this builder.
        /// </summary>
        /// <param name="messageType">The TCNet message type ID to register.</param>
        /// <param name="create">A delegate function that creates an instance of the packet.</param>
        public static void RegisterPacket(MessageTypes messageType, Func<TCNetPacket> create)
        {
            registeredPackets[messageType] = create;
        }

        /// <summary>
        /// Register a TCNet data packet sub type to be built and created by this builder.
        /// </summary>
        /// <param name="messageType">The TCNet message type ID to register.</param>
        /// <param name="create">A delegate function that creates an instance of the packet.</param>
        public static void RegisterDataPacket(DataTypes dataType, Func<TCNetPacket> create)
        {
            registeredDataPackets[dataType] = create;
        }

        /// <summary>
        /// Register a TCNet application specific packet sub type to be built and created by this builder.
        /// </summary>
        /// <param name="messageType">The TCNet message type ID to register.</param>
        /// <param name="create">A delegate function that creates an instance of the packet.</param>
        public static void RegisterApplicationPacket(uint signature, Func<TCNetPacket> create)
        {
            registeredApplicationPackets[signature] = create;
        }

        /// <summary>
        /// Called when this class is first used in the software.
        /// </summary>
        static TCNetPacketBuilder()
        {
            TCNetPacketBuilder.RegisterPacket(MessageTypes.OptIn, (() => new TCNetOptIn()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.OptIn, (() => new TCNetOptOut()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.TimeSync, (() => new TCNetTimeSync()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.Error, (() => new TCNetError()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.DataRequest, (() => new TCNetDataRequest()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.ControlMessages, (() => new TCNetControl()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.TextData, (() => new TCNetTextData()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.KeyboardData, (() => new TCNetKeyboard()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.Time, (() => new TCNetTime()));
            TCNetPacketBuilder.RegisterPacket(MessageTypes.ApplicationSpecificData, (() => new TCNetApplicationSpecificData()));

            TCNetPacketBuilder.RegisterDataPacket(DataTypes.Metrics, (() => new TCNetMetrics()));
            TCNetPacketBuilder.RegisterDataPacket(DataTypes.MetaData, (() => new TCNetMetaData()));
            TCNetPacketBuilder.RegisterDataPacket(DataTypes.BeatGrid, (() => new TCNetBeatGrid()));
            TCNetPacketBuilder.RegisterDataPacket(DataTypes.Cue, (() => new TCNetCue()));
            TCNetPacketBuilder.RegisterDataPacket(DataTypes.SmallWaveform, (() => new TCNetSmallWaveform()));
            TCNetPacketBuilder.RegisterDataPacket(DataTypes.BigWaveform, (() => new TCNetBigWaveform()));
            TCNetPacketBuilder.RegisterDataPacket(DataTypes.LowResArtworkFile, (() => new TCNetLowResArtwork()));

        }

        /// <summary>
        /// Tries to create a packet from the header information.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="packet">The packet.</param>
        /// <returns></returns>
        internal static bool TryBuild(TCNetRecieveData data,DateTime timeStamp ,out TCNetPacket packet)
        {
            TCNetHeader header = new TCNetHeader(MessageTypes.None);
            packet = header;

            try
            {
                //We have read all the data.
                if (data.EndOfData())
                {
                    data.Reset();
                    return false;
                }

                //Check we have enough data to construct the header.
                if (data.Length - data.ReadPosition < TCNetHeader.PacketSize)
                    return false;

                //Read the packet header.
                header.ReadData(data.GetReader());

                //Ensure the header packet is valid
                if (!header.IsValid())
                {
                    //Purge data as it is probably corrupted.
                    data.Reset();        //Reset position to start so we dump the data.
                    return false;
                }

                //Read the sub packet
                Func<TCNetPacket> create;

                switch (header.MessageType)
                {
                    case MessageTypes.Data:
                        {
                            TCNetDataHeader dataHeader = new TCNetDataHeader(DataTypes.None);

                            //Read the packet header.
                            dataHeader.ReadData(data.GetReader());

                            if (!registeredDataPackets.TryGetValue(dataHeader.DataType, out create))
                                return false;
                        }
                        break;
                    case MessageTypes.ApplicationSpecificData:
                        {
                            TCNetApplicationSpecificData dataHeader = new TCNetApplicationSpecificData();

                            //Read the packet header.
                            dataHeader.ReadData(data.GetReader());

                            if (!registeredApplicationPackets.TryGetValue(dataHeader.ApplicationSignature, out create))
                                return false;
                        }
                        break;
                    default:
                        if (!registeredPackets.TryGetValue(header.MessageType, out create))
                            return false;
                        break;
                }
                
                //Create the sub packet
                packet = create();

                packet.RXTimeStamp = timeStamp;
                packet.ReadData(data.GetReader());
            }
            catch (EndOfStreamException)
            {
                return false;
            }

            return packet != null;
        }

    }
}
