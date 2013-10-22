using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp;
using Citp.IO;

namespace Citp.Packets.CaEx
{
    public enum RecorderClearingAvailability
    {
        Unsupported = 1,
        Unavailable = 1,
        Available = 2
    }

    public class CaExSetRecorderClearingCapabilities : CaExHeader
    {
        #region Setup and Initialisation

        public CaExSetRecorderClearingCapabilities()
            : base(CaExContentCodes.SetRecorderClearingCapabilities)
        {    
        }

        #endregion

        #region Packet Content

        public RecorderClearingAvailability Availability { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Availability = (RecorderClearingAvailability) data.ReadByte();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte) Availability);
        }

        #endregion
    }
}
