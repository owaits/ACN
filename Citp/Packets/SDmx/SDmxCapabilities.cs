using Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.SDmx
{
    public enum DmxCapabilities
    {
        ChannelList = 1,
        ExternalSource = 2,
        PerUniverseExternalSources = 3,
        ArtNetExternalSources = 101,
        StreamingACNExternalSource = 102,
        ETCNet2ExternalSource = 103
    }

    public class SDmxCapabilities : SDmxHeader
    {
        public const string PacketType = "Capa";

                #region Setup and Initialisation

        public SDmxCapabilities()
            : base(PacketType)
        {    
        }

        #endregion

        #region Packet Content

        private List<DmxCapabilities> capabilities = new List<DmxCapabilities>();

        public List<DmxCapabilities> Capabilities
        {
            get { return capabilities; }
            set { capabilities = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int count = data.ReadUInt16();
            for (int n = 0; n < count; n++)
            {
                Capabilities.Add((DmxCapabilities) data.ReadUInt16());
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((ushort) Capabilities.Count);
            foreach (DmxCapabilities capability in Capabilities)
                data.Write((ushort)capability);
        }

        #endregion
    }
}
