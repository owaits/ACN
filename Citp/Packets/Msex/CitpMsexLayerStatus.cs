using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexLayerStatus:CitpMsexHeader
    {
        public const string PacketType = "LSta";

        #region Constructors

        public CitpMsexLayerStatus()
            : base()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexLayerStatus(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        private List<LayerStatus> layers = new List<LayerStatus>();

        public List<LayerStatus> Layers
        {
            get { return layers; }
        }

        public class LayerStatus:CitpMsexHeader
        {
            public LayerStatus()
            { 
            }

            public LayerStatus(CitpBinaryReader data, Version msexVersion)
            {
                MsexVersion = msexVersion;
                ReadData(data);
            }

            public byte LayerNumber;

            public byte PhysicalOutput;

            public byte MediaLibraryNumber = 0;

            public byte MediaLibraryType;

            public CitpMsexLibraryId MediaLibraryId = new CitpMsexLibraryId();

            public byte MediaNumber;

            public string MediaName = string.Empty;

            public UInt32 MediaPosition;

            public UInt32 MediaLength;

            public byte MediaFPS;

            public UInt32 LayerStatusFlags;

            public override void ReadData(CitpBinaryReader data)
            {
                LayerNumber = data.ReadByte();
                PhysicalOutput = data.ReadByte();
                if (MsexVersion < CitpMsexVersions.Msex12Version)
                {
                    MediaLibraryNumber = data.ReadByte();
                }
                else
                {
                    MediaLibraryType = data.ReadByte();
                    MediaLibraryId = data.ReadMsexLibraryId();
                }
                MediaNumber = data.ReadByte();
                MediaName = data.ReadUcs2();
                MediaPosition = data.ReadUInt32();
                MediaLength = data.ReadUInt32();
                MediaFPS = data.ReadByte();
                LayerStatusFlags = data.ReadUInt32();
            }

            public override void WriteData(CitpBinaryWriter data)
            {
                data.Write(LayerNumber);
                data.Write(PhysicalOutput);

                if (MsexVersion < CitpMsexVersions.Msex12Version)
                {
                    data.Write(MediaLibraryNumber);
                }
                else
                {
                    data.Write(MediaLibraryType);
                    data.WriteMsexLibraryId(MediaLibraryId);
                }

                data.Write(MediaNumber);
                data.WriteUcs2(MediaName);
                data.Write(MediaPosition);
                data.Write(MediaLength);
                data.Write(MediaFPS);
                data.Write(LayerStatusFlags);               
            }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            byte layerCount = data.ReadByte();
            for(int n=0;n<layerCount;n++)
            {
                Layers.Add(new LayerStatus(data, MsexVersion));
            }            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) Layers.Count);

            foreach (LayerStatus layer in Layers)
            {
                layer.MsexVersion = MsexVersion; //Avoid the layers sending a different version to the packet.
                layer.WriteData(data);
            }
            
        }

        #endregion
    }
}
