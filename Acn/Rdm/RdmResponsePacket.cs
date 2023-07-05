using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LXProtocols.Acn.Rdm
{
    public abstract class RdmResponsePacket:RdmPacket
    {
        public RdmResponsePacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
            //Set the default response type to Ack, this is done here due to the header being shared between port ID and response type.
            ResponseType = RdmResponseTypes.Ack;
        }

        [Browsable(false)]
        public RdmResponseTypes ResponseType 
        { 
            get { return (RdmResponseTypes) Header.PortOrResponseType;  } 
            set { Header.PortOrResponseType = (byte) value; } 
        }
    }
}
