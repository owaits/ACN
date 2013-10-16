using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
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

            public LayerStatus(CitpBinaryReader data)
            {
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

                
            }

            public override void WriteData(CitpBinaryWriter data)
            {
                data.Write(LayerNumber);
                data.Write(PhysicalOutput);

                if (MsexVersion < 1.2)
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
                Layers.Add(new LayerStatus(data));
            }            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) Layers.Count);

            foreach (LayerStatus layer in Layers)
                layer.WriteData(data);
            
        }

        #endregion
    }
}
