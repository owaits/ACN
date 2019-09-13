using LXProtocols.TCNet.Packets;
using System;
using System.Collections.Generic;
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
                switch (header.ContentType)
                {
                    case MessageTypes.GWOffer:
                        packet = new GWOffer();                        
                        break;
                    case MessageTypes.Timecode:
                        packet = new TCNetTime();                        
                        break;
                    default:
                        return false;
                }

                packet.TimeStamp = timeStamp;
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
