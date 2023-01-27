﻿using System;
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
        }

        [Browsable(false)]
        public RdmResponseTypes ResponseType 
        { 
            get { return (RdmResponseTypes) Header.PortOrResponseType;  } 
            set { Header.PortOrResponseType = (byte) value; } 
        }
    }
}
