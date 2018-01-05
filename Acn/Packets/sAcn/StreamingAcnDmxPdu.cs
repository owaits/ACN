using Acn.Packets.Dmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.sAcn
{
    /// <summary>
    /// The DMX data PDU for streaming ACN.
    /// </summary>
    public class StreamingAcnDmxPdu:DmpSetProperty
    {
        private byte startCode = byte.MaxValue;

        /// <summary>
        /// Gets or sets the DMX start code.
        /// </summary>
        /// <remarks>
        /// This allows the start code to be written to the packet seperately to the DMX data. If set
        /// to byte.MaxValue then the DMX data is assumed to contain the start code. The default is that
        /// start code is part of the data.
        /// </remarks>
        public byte StartCode
        {
            get { return startCode; }
            set { startCode = value; }
        }

        /// <summary>
        /// Gets the length of the property.
        /// </summary>
        /// <value>
        /// The length of the property.
        /// </value>
        public override short PropertyLength
        {
            get
            {
                if (StartCode != 0xFF)
                    return (short) (base.PropertyLength + 1);
                return base.PropertyLength;
            }
        }

        /// <summary>
        /// Writes the content.
        /// </summary>
        protected override void WriteContent(IO.AcnBinaryWriter data)
        {
            if (StartCode != 0xFF)
                data.Write(StartCode);
            base.WriteContent(data);
        }
    }
}
