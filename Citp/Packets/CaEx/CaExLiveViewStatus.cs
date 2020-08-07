using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.CaEx
{
    public enum LiveViewAvailability
    {
        None = 0,
        Alpha = 1,
        Beta = 2,
        Gamma = 3
    }

    public class CaExLiveViewStatus:CaExHeader
    {
        #region Setup and Initialisation

        public CaExLiveViewStatus()
            : base(CaExContentCodes.LiveViewStatus)
        {    
        }

        #endregion

        #region Packet Content

        public LiveViewAvailability Availability { get; set; }

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
            Availability = (LiveViewAvailability)data.ReadByte();
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
            data.Write((byte) Availability);
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
