using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Acn.Sockets;
using Acn.Rdm;
using Acn.Rdm.Packets;
using Acn.Rdm.Packets.Net;
using System.ComponentModel;
using Acn.Rdm.Packets.Product;
using System.Threading;
using Acn.Rdm.Broker;
using Acn.Rdm.Packets.DMX;
using Acn.Rdm.Packets.Control;
using Acn.Rdm.Packets.Configuration;
using Acn.Rdm.Packets.Power;
using RdmSnoop;

namespace RdmNetworkMonitor
{
    public class RdmDeviceBroker:RdmMessageBroker, INotifyPropertyChanged
    {
        IRdmSocket socket = null;

        public RdmDeviceBroker(IRdmSocket socket, UId id, RdmAddress address)
        {
            Id = id;
            Address = address;
            this.socket = socket;

            RegisterHandlers(this);
            
            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
        }

        #region Data Model
        
        #region Information

        [Category("Information")]
        public UId Id { get; protected set; }

        [Category("Information")]
        public RdmAddress Address { get; protected set; }

        [Category("Information")]
        public int SubDeviceId { get; protected set; }

        private List<RdmDeviceBroker> subDevices = new List<RdmDeviceBroker>();

        [Browsable(false)]
        public List<RdmDeviceBroker> SubDevices
        {
            get { return subDevices; }
        }

        #endregion  

        #region Product

        private string manufacturer = string.Empty;

        [Category("Product")]
        public string Manufacturer
        {
            get { return manufacturer; }
            protected set
            {
                if (manufacturer != value)
                {
                    manufacturer = value;
                    RaisePropertyChanged("Manufacturer");
                }
            }
        }

        private string model = string.Empty;

        [Category("Product")]
        public string Model
        {
            get { return model; }
            protected set 
            {
                if (model != value)
                {
                    model = value;
                    RaisePropertyChanged("Model");
                }                
            }
        }

        private string label = string.Empty;

        [Category("Product")]
        public string Label
        {
            get { return label; }
            set 
            {
                if (label != value)
                {
                    label = value;

                    DeviceLabel.Set setLabel = new DeviceLabel.Set();
                    setLabel.Label = value;
                    socket.SendRdm(setLabel, Address, Id);

                    RaisePropertyChanged("Label");
                }
            }
        }

        private DeviceInfo.GetReply deviceInformation = null;

        [Category("Product")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DeviceInfo.GetReply DeviceInformation
        {
            get { return deviceInformation; }
            set 
            {
                if (deviceInformation != value)
                {
                    deviceInformation = value;
                    RaisePropertyChanged("DeviceInformation");
                }
            }
        }



        #endregion

        #region DMX Information

        private int dmxAddress = 0;

        [Category("DMX")]
        public int DmxAddress
        {
            get { return dmxAddress; }
            set 
            {
                if (dmxAddress != value)
                {
                    dmxAddress = value;

                    SetDmxAddress(value);

                    RaisePropertyChanged("DmxAddress");
                }
            }
        }

        private string mode = string.Empty;

        [Category("DMX")]
        public string Mode
        {
            get { return mode; }
            protected set 
            {
                if (mode != value)
                {
                    mode = value;
                    RaisePropertyChanged("Mode");
                }
            }
        }

        private List<PersonalitySlotInformation> personality = new List<PersonalitySlotInformation>();

        [Category("DMX")]
        public List<PersonalitySlotInformation> Personality
        {
            get { return personality; }
        }




        #endregion

        #region Configuration

        private bool panInvert = false;

        [Category("Configuration")]
        public bool PanInvert
        {
            get { return panInvert; }
            set 
            {
                if (panInvert != value)
                {
                    panInvert = value;

                    PanInvert.Set invert = new PanInvert.Set();
                    invert.Inverted = value;
                    socket.SendRdm(invert, Address, Id);

                    RaisePropertyChanged("PanInvert");
                }
            }
        }

        private bool tiltInvert = false;

        [Category("Configuration")]
        public bool TiltInvert
        {
            get { return tiltInvert; }
            set
            {
                if (tiltInvert != value)
                {
                    tiltInvert = value;

                    TiltInvert.Set invert = new TiltInvert.Set();
                    invert.Inverted = value;
                    socket.SendRdm(invert, Address, Id);

                    RaisePropertyChanged("TiltInvert");
                }
            }
        }

        private bool panTiltSwap = false;

        [Category("Configuration")]
        public bool PanTiltSwap
        {
            get { return panTiltSwap; }
            set
            {
                if (panTiltSwap != value)
                {
                    panTiltSwap = value;

                    PanTiltSwap.Set invert = new PanTiltSwap.Set();
                    invert.Swapped = value;
                    socket.SendRdm(invert, Address, Id);

                    RaisePropertyChanged("PanTiltSwap");
                }
            }
        }
        

        #endregion

        #region History

        private int deviceHours = 0;

        [Category("History")]
        public int DeviceHours
        {
            get { return deviceHours; }
            protected set 
            {
                if (deviceHours != value)
                {
                    deviceHours = value;
                    RaisePropertyChanged("DeviceHours");
                }
            }
        }

        private int lampHours = 0;

        [Category("History")]
        public int LampHours
        {
            get { return lampHours; }
            protected set
            {
                if (lampHours != value)
                {
                    lampHours = value;
                    RaisePropertyChanged("LampHours");
                }
            }
        }

        private int powerCycles = 0;

        [Category("History")]
        public int PowerCycles
        {
            get { return powerCycles; }
            protected set
            {
                if (powerCycles != value)
                {
                    powerCycles = value;
                    RaisePropertyChanged("PowerCycles");
                }
            }
        }

        private int lampStrikes = 0;

        [Category("History")]
        public int LampStrikes
        {
            get { return lampStrikes; }
            protected set
            {
                if (lampStrikes != value)
                {
                    lampStrikes = value;
                    RaisePropertyChanged("LampStrikes");
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public void Identify(bool enabled)
        {
            IdentifyDevice.Set identify = new IdentifyDevice.Set();
            identify.IdentifyEnabled = enabled;
            socket.SendRdm(identify, Address, Id);
        }

        public void SetDmxAddress(int address)
        {
            DmxStartAddress.Set setAddress = new DmxStartAddress.Set();
            setAddress.DmxAddress = (short) address;
            socket.SendRdm(setAddress, Address, Id);
        }

        public void SetMode(int mode)
        {
            DmxPersonality.Set setMode = new DmxPersonality.Set();
            setMode.PersonalityIndex = (byte)mode;
            socket.SendRdm(setMode, Address, Id);
        }

        public void Reset()
        {
            ResetDevice.Set reset = new ResetDevice.Set();
            socket.SendRdm(reset, Address, Id);
        }

        public void SelfTest()
        {
            PerformSelfTest.Set test = new PerformSelfTest.Set();
            socket.SendRdm(test, Address, Id);
        }

        public void Power(PowerState.States state)
        {
            PowerState.Set power = new PowerState.Set();
            power.State = state;
            socket.SendRdm(power, Address, Id);
        }


        #endregion

        #region RDM Communication

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            //Exit if not from the target device or correct sub device.
            if (!SubDeviceUId.IsMatch(Id,e.Packet.Header.SourceId,e.Packet.Header.SubDevice))
                return;

            RdmPacket replyPacket = ProcessPacket(e.Packet);

            if (replyPacket != null)
            {
                socket.SendRdm(replyPacket, Address, Id);
            }
                

            DeviceInfo.GetReply info = e.Packet as DeviceInfo.GetReply;
            if (info != null)
            {
                DeviceInformation = info;

                RequestDetails();
                RequestLabel();
                RequestConfiguration();
                RequestHistory();
                RequestPersonality();

                if (!SubDeviceUId.IsSubDevice(Id))
                {
                    for (short n = 1; n <= info.SubDeviceCount; n++)
                    {
                        RdmDeviceBroker subDeviceBroker = new RdmDeviceBroker(socket, new SubDeviceUId(Id, n), Address);
                        SubDevices.Add(subDeviceBroker);
                        subDeviceBroker.Interogate();
                    }
                    
                    RaisePropertyChanged("SubDevices");
                }
            }

            ManufacturerLabel.GetReply manufacturer = e.Packet as ManufacturerLabel.GetReply;
            if (manufacturer != null)
            {
                Manufacturer = manufacturer.Label;
            }

            DeviceModelDescription.GetReply model = e.Packet as DeviceModelDescription.GetReply;
            if (model != null)
            {
                Model = model.Description;
            }

            DeviceLabel.GetReply label = e.Packet as DeviceLabel.GetReply;
            if (label != null)
            {
                this.label = label.Label;
                RaisePropertyChanged("Label");
            }
        }

        #region Packet Handlers

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DmxStartAddress)]
        private RdmPacket ProcessDmxStartAddress(RdmPacket packet)
        {
            DmxStartAddress.GetReply response = packet as DmxStartAddress.GetReply;
            if (response != null)
            {
                this.dmxAddress = response.DmxAddress;
                RaisePropertyChanged("DmxAddress");
            }

            return null;
        }

        [RdmMessage(RdmCommands.SetResponse, RdmParameters.DmxStartAddress)]
        private RdmPacket ProcessSetDmxStartAddress(RdmPacket packet)
        {
            return new DmxStartAddress.Get();
        }

        [RdmMessage(RdmCommands.SetResponse, RdmParameters.DmxPersonality)]
        private RdmPacket ProcessSetDmxPersonality(RdmPacket packet)
        {
            return new DeviceInfo.Get();
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.TiltInvert)]
        private RdmPacket ProcessTiltInvert(RdmPacket packet)
        {
            TiltInvert.GetReply response = packet as TiltInvert.GetReply;
            if (response != null)
                tiltInvert = response.Inverted;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.PanInvert)]
        private RdmPacket ProcessPanInvert(RdmPacket packet)
        {
            PanInvert.GetReply response = packet as PanInvert.GetReply;
            if (response != null)
                panInvert = response.Inverted;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.PanTiltSwap)]
        private RdmPacket ProcessPanTiltSwap(RdmPacket packet)
        {
            PanTiltSwap.GetReply response = packet as PanTiltSwap.GetReply;
            if (response != null)
                panTiltSwap = response.Swapped;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DeviceHours)]
        private RdmPacket ProcessDeviceHours(RdmPacket packet)
        {
            DeviceHours.GetReply response = packet as DeviceHours.GetReply;
            if (response != null)
                DeviceHours = response.DeviceHours;

            //return new DevicePowerCycles.Get();
            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DevicePowerCycles)]
        private RdmPacket ProcessPowerCycles(RdmPacket packet)
        {
            DevicePowerCycles.GetReply response = packet as DevicePowerCycles.GetReply;
            if (response != null)
                PowerCycles = response.PowerCycles;

            //return new LampHours.Get();
            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.LampHours)]
        private RdmPacket ProcessLampHours(RdmPacket packet)
        {
            LampHours.GetReply response = packet as LampHours.GetReply;
            if (response != null)
                LampHours = response.LampHours;

            //return new LampStrikes.Get();
            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.LampStrikes)]
        private RdmPacket ProcessLampStrikes(RdmPacket packet)
        {
            LampStrikes.GetReply response = packet as LampStrikes.GetReply;
            if (response != null)
                LampStrikes = response.LampStrikes;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DmxPersonalityDescription)]
        private RdmPacket ProcessPersonalityDescription(RdmPacket packet)
        {
            DmxPersonalityDescription.GetReply response = packet as DmxPersonalityDescription.GetReply;
            if (response != null)
            {
                Mode = response.Description;
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SlotInfo)]
        private RdmPacket ProcessSlotInfo(RdmPacket packet)
        {
            SlotInfo.GetReply response = packet as SlotInfo.GetReply;
            if (response != null)
            {
                foreach(SlotInfo.SlotInformation slot in response.Slots)
                {
                    PersonalitySlotInformation slotInfo = new PersonalitySlotInformation();
                    slotInfo.Id = slot.Id;
                    slotInfo.Offset = slot.Offset;
                    slotInfo.Type = slot.Type;
                    slotInfo.SlotLink = slot.SlotLink;

                    Personality.Add(slotInfo);

                    //Request the slot description.
                    SlotDescription.Get slotDescription = new SlotDescription.Get();
                    slotDescription.SlotOffset = slot.Offset;
                    socket.SendRdm(slotDescription, Address, Id);
                }
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SlotDescription)]
        private RdmPacket ProcessSlotDescription(RdmPacket packet)
        {
            SlotDescription.GetReply response = packet as SlotDescription.GetReply;
            if (response != null)
            {
                if (response.SlotOffset >= 0 && response.SlotOffset < Personality.Count)
                {
                    Personality[response.SlotOffset].Description = response.Description;
                }
            }

            return null;
        }

        #endregion

        public void Interogate()
        {
            DeviceInfo.Get getInfo = new DeviceInfo.Get();
            socket.SendRdm(getInfo, Address, Id);

            PortList.Get getPorts = new PortList.Get();
            socket.SendRdm(getPorts, Address, Id);
        }

        public void RequestDetails()
        {
            ManufacturerLabel.Get manufacturer = new ManufacturerLabel.Get();
            socket.SendRdm(manufacturer, Address, Id);

            DeviceModelDescription.Get model = new DeviceModelDescription.Get();
            socket.SendRdm(model, Address, Id);

            DmxStartAddress.Get dmxAddress = new DmxStartAddress.Get();
            socket.SendRdm(dmxAddress, Address, Id);
        }

        public void RequestLabel()
        {
            DeviceLabel.Get label = new DeviceLabel.Get();
            socket.SendRdm(label, Address, Id);
        }

        public void RequestDmxInformation()
        {
            ManufacturerLabel.Get manufacturer = new ManufacturerLabel.Get();
            socket.SendRdm(manufacturer, Address, Id);

            DeviceModelDescription.Get model = new DeviceModelDescription.Get();
            socket.SendRdm(model, Address, Id);
        }

        public void RequestConfiguration()
        {
            PanInvert.Get pan = new PanInvert.Get();
            socket.SendRdm(pan, Address, Id);

            TiltInvert.Get tilt = new TiltInvert.Get();
            socket.SendRdm(tilt, Address, Id);

            PanTiltSwap.Get swap = new PanTiltSwap.Get();
            socket.SendRdm(swap, Address, Id);
        }

        public void RequestHistory()
        {
            DeviceHours.Get hours = new DeviceHours.Get();
            socket.SendRdm(hours, Address, Id);

            DevicePowerCycles.Get cycles = new DevicePowerCycles.Get();
            socket.SendRdm(cycles, Address, Id);

            LampHours.Get lampHours = new LampHours.Get();
            socket.SendRdm(lampHours, Address, Id);

            LampStrikes.Get lampStrikes = new LampStrikes.Get();
            socket.SendRdm(lampStrikes, Address, Id);
        }

        public void RequestPersonality()
        {
            DmxPersonalityDescription.Get packet = new DmxPersonalityDescription.Get();
            packet.PersonalityIndex = DeviceInformation.DmxPersonality;
            socket.SendRdm(packet, Address, Id);

            SlotInfo.Get slotPacket = new SlotInfo.Get();
            socket.SendRdm(slotPacket, Address, Id);
        }

        #endregion


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        
    }
}
