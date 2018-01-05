using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm.Packets.Discovery;

namespace Acn.Rdm.Broker
{
    public enum DiscoveryStatus
    {
        Stopped,
        Running,
        Complete,
        Ready // ?? MD: Don't know what this is but wont build without it 
    }

    public abstract class RdmDiscovery
    {
        private UId searchRangeStart = UId.MinValue;
        private UId searchRangeEnd = UId.MaxValue;

        private List<UId> deviceTable = new List<UId>();

        private DiscoveryStatus status = DiscoveryStatus.Stopped;

        public DiscoveryStatus Status
        {
            get { return status; }
            protected set { status = value; }
        }

        public void StartDiscovery(bool fullDiscovery)
        {
            if (Status == DiscoveryStatus.Running)
                throw new InvalidOperationException("A previous discovery is still in progress. Please either stop that discovery or wait for it to complete.");

            searchRangeStart = UId.MaxValue;
            searchRangeEnd = UId.MinValue;
            
            Status = DiscoveryStatus.Running;

            if (fullDiscovery)
            {
                //Clear all known devices from the device table.
                deviceTable.Clear();
            }
            else
            {
                //Mute any known devices.
                List<UId> knownDevices = new List<UId>(deviceTable);
                deviceTable.Clear();        //Clear the device table as the mute reply will re-add the device if it still exists.            

                //Send a mute packet to all known devices.
                foreach (UId id in deviceTable)
                    MuteDevice(id);
            }

            //Broadcast a discovery message to all devices.
            Discover();
        }

        protected void NewDeviceFound(UId deviceId)
        {

        }

        #region Rdm

        protected void Discover()
        {
            DiscoveryUniqueBranch.Request discover = new DiscoveryUniqueBranch.Request();
            discover.Header.DestinationId = UId.Broadcast;
            discover.LowerBoundId = searchRangeStart;
            discover.UpperBoundId = searchRangeEnd;

            SendPacket(discover);

        }

        protected void MuteDevice(UId deviceId)
        {
            DiscoveryMute.Request mute = new DiscoveryMute.Request();
            mute.Header.DestinationId = deviceId;

            SendPacket(mute);
        }

        protected void UnMuteDevice(UId deviceId)
        {
            DiscoveryUnMute.Request mute = new DiscoveryUnMute.Request();
            mute.Header.DestinationId = deviceId;
            SendPacket(mute);
        }


        #endregion

        protected void ProcessPacket(RdmPacket packet)
        {

        }

        protected abstract void SendPacket(RdmPacket packet);
    }
}
