using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Acn.Packets.Sdt
{
    public class StdChannelParams : AcnPdu
    {
        public StdChannelParams()
            : base((int) StdVectors.ChannelParameters,1)
        {
        }

        #region Packet Contents

        public ChannelParameterBlock Channel { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            throw new NotImplementedException();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
