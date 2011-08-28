using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.DMX
{
    public enum SlotTypes
    {
        Primary = 0,
        Fine = 1,
        Timing = 2,
        Speed = 3,
        Control = 4,
        Index = 5,
        Rotation = 6,
        IndexRotation = 7,
        Undefined = 0xff
    }

    public enum SlotIds
    {
        Intensity = 1,
        IntensityMaster = 2,
        Pan = 0x101,
        Tilt = 0x102
    }

    /// <summary>
    /// This parameter is used to retrieve basic information about the functionality of the DMX512 slots
    /// used to control the device.
    /// </summary>
    public class SlotInfo
    {
        public struct SlotInformation
        {
            public short Offset { get; set; }

            public SlotTypes Type { get; set; }

            public SlotIds Id { get; set; }           
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.SlotInfo)
            {
            }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SlotInfo)
            {
                Slots = new List<SlotInformation>();
            }

            public List<SlotInformation> Slots { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Slots.Clear();
                for (int n = 0; n < Header.ParameterDataLength / 5; n++)
                {
                    SlotInformation slot = new SlotInformation();
                    slot.Offset = data.ReadNetwork16();
                    slot.Type = (SlotTypes) data.ReadByte();
                    slot.Id = (SlotIds) data.ReadNetwork16();
                    Slots.Add(slot);
                }
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                foreach (SlotInformation slot in Slots)
                {
                    data.WriteNetwork(slot.Offset);
                    data.WriteNetwork((byte) slot.Type);
                    data.WriteNetwork((byte) slot.Id);
                }
            }

            #endregion
        }
    }
}
