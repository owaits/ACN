using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExLiveViewImage:CaExHeader
    {
        #region Setup and Initialisation

        public CaExLiveViewImage()
            : base(CaExContentCodes.LiveViewImage)
        {    
        }

        #endregion

        #region Packet Content

        public LiveViewImageFormat Format { get; set; }

        public byte[] Data { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Format = (LiveViewImageFormat) data.ReadByte();
            uint dataSize = data.ReadUInt32();
            Data = data.ReadData((int) dataSize);

            //If we where not able to read enough data to fill the image data then throw EndOfStream so we can wait for more data.
            if (Data.Length != dataSize)
                throw new EndOfStreamException();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte) Format);
            data.Write(Data.Length);
            data.Write(Data);
        }

        #endregion
    }
}
