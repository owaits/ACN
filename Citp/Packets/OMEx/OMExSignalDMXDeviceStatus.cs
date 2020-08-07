using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.OMEx
{
    public class OMExSignalDMXDeviceStatus:OMExHeader
    {
        public new const string PacketType = "SDDS";

        #region Setup and Initialisation

        public OMExSignalDMXDeviceStatus()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

        public string StatusIdentifier { get; set; }

        public byte Severity { get; set; }

        public string Category { get; set; }

        public string ShortText { get; set; }

        public string LongText { get; set; }

        private List<DeviceInformation> devices = new List<DeviceInformation>();

        public List<DeviceInformation> Devices
        {
            get { return devices; }
            set { devices = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            StatusIdentifier = data.ReadUcs2();
            Severity = data.ReadByte();
            Category = data.ReadUcs2();
            ShortText = data.ReadUcs2();
            LongText = data.ReadUcs2();

            int deviceCount = data.ReadUInt16();
            Devices.Clear();
            for (int n = 0; n < deviceCount; n++)
            {
                DeviceInformation device = new DeviceInformation()
                {
                    DMXConnectionsString = data.ReadUcs1()
                };
                Devices.Add(device);
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteUcs2(StatusIdentifier);
            data.Write(Severity);
            data.WriteUcs2(Category);
            data.WriteUcs2(ShortText);
            data.WriteUcs2(LongText);

            data.Write((ushort)Devices.Count);
            foreach (DeviceInformation device in Devices)
                data.WriteUcs1(device.DMXConnectionsString);
        }

        #endregion
    }
}
