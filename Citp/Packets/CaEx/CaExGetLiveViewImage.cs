using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;
using Citp;

namespace Citp.Packets.CaEx
{
    public enum LiveViewImageFormat
    {
        JPEG = 1
    }

    public class CaExGetLiveViewImage:CaExHeader
    {
        #region Setup and Initialisation

        public CaExGetLiveViewImage()
            : base(CaExContentCodes.GetLiveViewImage)
        {    
        }

        #endregion

        #region Packet Content

        public LiveViewImageFormat Format { get; set; }

        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public float CameraPositionX { get; set; }
        public float CameraPositionY { get; set; }
        public float CameraPositionZ { get; set; }

        public float CameraFocusX { get; set; }
        public float CameraFocusY { get; set; }
        public float CameraFocusZ { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Format = (LiveViewImageFormat)data.ReadByte();
            Width = data.ReadUInt16();
            Height = data.ReadUInt16();
            CameraPositionX = data.ReadSingle();
            CameraPositionY = data.ReadSingle();
            CameraPositionZ = data.ReadSingle();
            CameraFocusX = data.ReadSingle();
            CameraFocusY = data.ReadSingle();
            CameraFocusZ = data.ReadSingle();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte)Format);
            data.Write(Width);
            data.Write(Height);
            data.Write(CameraPositionX);
            data.Write(CameraPositionY);
            data.Write(CameraPositionZ);
            data.Write(CameraFocusX);
            data.Write(CameraFocusY);
            data.Write(CameraFocusZ);
        }

        #endregion
    }
}
