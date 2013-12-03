using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexServerInformation: CitpMsexHeader
    {
        public const string PacketType = "SInf";

        #region Constructors

        public CitpMsexServerInformation():base()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexServerInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public string UUID { get; set; }

        public string ProductName { get; set; }

        public byte ProductVersionMajor { get; set; }

        public byte ProductVersionMinor { get; set; }

        public byte ProductVersionBugfix { get; set; }

        private List<UInt16> supportedMsexVersions = new List<ushort>();

        public List<UInt16> SupportedMsexVersions 
        {
            get { return supportedMsexVersions; }
        }

        public UInt16 SupportedLibraryTypes { get; set; }

        private List<string> thumbnailFormats = new List<string>();

        public List<string> ThumbnailFormats 
        {
            get { return thumbnailFormats; }
        }

        private List<string> streamFormats = new List<string>();

        public List<string> StreamFormats
        {
            get { return streamFormats; }
        }

        private List<DmxDescriptor> dmxLayers = new List<DmxDescriptor>();

        public List<DmxDescriptor> DmxLayers
        {
            get { return dmxLayers; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            if (MsexVersion > 1.1)  UUID = data.ReadUcs1();
            ProductName = data.ReadUcs2();
            ProductVersionMajor = data.ReadByte();
            ProductVersionMinor = data.ReadByte();

            if (MsexVersion > 1.1)
            {
                ProductVersionBugfix = data.ReadByte();

                int versionCount = data.ReadByte();
                for (int n = 0; n < versionCount; n++)
                    SupportedMsexVersions.Add(data.ReadUInt16());

                SupportedLibraryTypes = data.ReadUInt16();

                int thumbnailFormatsCount = data.ReadByte();
                for (int n = 0; n < thumbnailFormatsCount; n++)
                    ThumbnailFormats.Add(data.ReadCookie());

                int streamFormatsCount = data.ReadByte();
                for (int n = 0; n < streamFormatsCount; n++)
                    StreamFormats.Add(data.ReadCookie());
            }

            int layerCount = data.ReadByte();
            for (int n = 0; n < layerCount; n++)
            {
                DmxDescriptor dmx;
                if (DmxDescriptor.TryParse(data.ReadUcs1(), out dmx))
                    DmxLayers.Add(dmx);
            }
                
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            if (MsexVersion > 1.1) data.WriteUcs1(UUID);
            data.WriteUcs2(ProductName);
            data.Write(ProductVersionMajor);
            data.Write(ProductVersionMinor);

            if (MsexVersion > 1.1)
            {
                data.Write(ProductVersionBugfix);

                data.Write((byte) SupportedMsexVersions.Count);
                foreach (UInt16 version in SupportedMsexVersions)
                    data.Write(version);

                data.Write(SupportedLibraryTypes);

                data.Write((byte) ThumbnailFormats.Count);
                foreach (string format in ThumbnailFormats)
                    data.WriteCookie(format);

                data.Write((byte) StreamFormats.Count);
                foreach (string format in StreamFormats)
                    data.WriteCookie(format);
            }

            data.Write((byte) DmxLayers.Count);
            foreach (DmxDescriptor layer in DmxLayers)
                data.WriteUcs1(layer.ToString());
        }

        #endregion
    }
}
