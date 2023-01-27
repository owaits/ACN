using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm
{
    public abstract class RdmRequestPacket : RdmPacket
    {
        public RdmRequestPacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
            //For a controller requesting information to port ID should be 1 or more.
            Header.PortOrResponseType = 1;
        }

        /// <summary>
        /// Gets or sets the ID of the ort this request is being made on between 1-255.
        /// </summary>
        /// <remarks>
        /// When a controller is sending messages it should set the port ID to the DMX port the message is being sent from.
        /// </remarks>
        [Browsable(false)]
        public int PortId
        {
            get { return (int)Header.PortOrResponseType; }
            set { Header.PortOrResponseType = (byte)value; }
        }
    }
}
