using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp;
using Citp.IO;

namespace Citp.Packets.CaEx
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
            Data = data.ReadBytes((int) dataSize);
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
