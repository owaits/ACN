using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm.Packets.DMX;

namespace Acn.Rdm.Broker
{
    public class RdmPersonality:List<RdmSlot>
    {
        public void Add8BitChannel(SlotIds id, string description, byte defaultValue)
        {
            RdmSlot slot = new RdmSlot();
            slot.Information = new SlotInfo.SlotInformation((short) this.Count, id);
            slot.Description = description;
            slot.DefaultValue = new DefaultSlotValue.SlotValue((short) this.Count,defaultValue);
            Add(slot);
        }

        public void Add8BitChannel(SlotTypes linkType, int linkOffset, string description, byte defaultValue)
        {
            RdmSlot slot2 = new RdmSlot();
            slot2.Information = new SlotInfo.SlotInformation((short)this.Count, linkType, (short)linkOffset);
            slot2.Description = description;
            slot2.DefaultValue = new DefaultSlotValue.SlotValue((short)this.Count, defaultValue);
            Add(slot2);
        }

        public void Add16BitChannel(SlotIds id, string description, byte defaultValue)
        {
            RdmSlot slot1 = new RdmSlot();
            slot1.Information = new SlotInfo.SlotInformation((short)this.Count, id);
            slot1.Description = description;
            slot1.DefaultValue = new DefaultSlotValue.SlotValue((short)this.Count, defaultValue);
            Add(slot1);

            RdmSlot slot2 = new RdmSlot();
            slot2.Information = new SlotInfo.SlotInformation((short)(this.Count + 1), SlotTypes.Fine, (short)this.Count);
            slot2.Description = description;
            slot2.DefaultValue = new DefaultSlotValue.SlotValue((short)this.Count, defaultValue);
            Add(slot2);
        }

        public void Add16BitChannel(SlotTypes linkType, int linkOffset, string description, byte defaultValue)
        {
            RdmSlot slot1 = new RdmSlot();
            slot1.Information = new SlotInfo.SlotInformation((short)this.Count, linkType, (short)linkOffset);
            slot1.Description = description;
            slot1.DefaultValue = new DefaultSlotValue.SlotValue((short)this.Count, defaultValue);
            Add(slot1);

            RdmSlot slot2 = new RdmSlot();
            slot2.Information = new SlotInfo.SlotInformation((short)(this.Count + 1), SlotTypes.Fine, (short)this.Count);
            slot2.Description = description;
            slot2.DefaultValue = new DefaultSlotValue.SlotValue((short)this.Count, defaultValue);
            Add(slot2);
        }

        [RdmMessage(RdmCommands.Get, RdmParameters.SlotInfo)]
        protected RdmPacket GetSlotInfo(RdmPacket packet)
        {
            SlotInfo.Get requestPacket = packet as SlotInfo.Get;
            if (requestPacket != null)
            {
                SlotInfo.GetReply replyPacket = new SlotInfo.GetReply();

                foreach (RdmSlot slot in this)
                {
                    replyPacket.Slots.Add(slot.Information);
                }

                return replyPacket;
            }

            return null;
        }

        [RdmMessage(RdmCommands.Get, RdmParameters.SlotDescription)]
        protected RdmPacket GetSlotDescription(RdmPacket packet)
        {
            SlotDescription.Get requestPacket = packet as SlotDescription.Get;
            if (requestPacket != null)
            {
                SlotDescription.GetReply replyPacket = new SlotDescription.GetReply();

                if(requestPacket.SlotOffset >=0 && requestPacket.SlotOffset < this.Count)
                {
                    replyPacket.Description = this[requestPacket.SlotOffset].Description;
                }

                return replyPacket;
            }

            return null;
        }

        [RdmMessage(RdmCommands.Get, RdmParameters.DefaultSlotValue)]
        protected RdmPacket GetDefaultSlotValue(RdmPacket packet)
        {
            DefaultSlotValue.Get requestPacket = packet as DefaultSlotValue.Get;
            if (requestPacket != null)
            {
                DefaultSlotValue.GetReply replyPacket = new DefaultSlotValue.GetReply();
                foreach (RdmSlot slot in this)
                {
                    replyPacket.DefaultValues.Add(slot.DefaultValue);
                }
            }

            return null;
        }
    }
}
