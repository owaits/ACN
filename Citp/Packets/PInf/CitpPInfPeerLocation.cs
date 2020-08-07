using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.PInf
{
    public class CitpPInfPeerLocation : CitpPInfHeader
    {
        public new const string PacketType = "PLoc";

        #region Constructors

        public CitpPInfPeerLocation()
        {
            LayerContentType = PacketType;
        }

        public CitpPInfPeerLocation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public ushort ListeningTCPPort { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            ListeningTCPPort = data.ReadUInt16();
            Type = data.ReadUcs1();
            Name = data.ReadUcs1();
            State = data.ReadUcs1();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(ListeningTCPPort);
            data.WriteUcs1(Type);
            data.WriteUcs1(Name);
            data.WriteUcs1(State);
        }

        #endregion
    }
}
