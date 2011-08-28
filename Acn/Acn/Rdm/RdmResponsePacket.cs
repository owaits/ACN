using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm
{
    public abstract class RdmResponsePacket:RdmPacket
    {
        public RdmResponsePacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
        }

        public RdmResponseTypes ResponseType { get; set; }
    }
}
