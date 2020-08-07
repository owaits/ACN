using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Rdm.Packets.DMX;

namespace LXProtocols.Acn.Rdm.Broker
{
    public class RdmSlot
    {
        public SlotInfo.SlotInformation Information { get; set; }

        public string Description { get; set; }

        public DefaultSlotValue.SlotValue DefaultValue { get; set; }
    }
}
