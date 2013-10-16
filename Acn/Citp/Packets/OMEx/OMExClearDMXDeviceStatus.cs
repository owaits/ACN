using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.OMEx
{
    public class OMExClearDMXDeviceStatus:OMExHeader
    {
        public const string PacketType = "CDDS";

        #region Setup and Initialisation

        public OMExClearDMXDeviceStatus()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

        public string StatusIdentifier { get; set; }

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

            data.Write((ushort)Devices.Count);
            foreach (DeviceInformation device in Devices)
                data.WriteUcs1(device.DMXConnectionsString);
        }

        #endregion
    }
}
