using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using LXProtocols.Acn.Sockets;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Rdm.Packets;
using LXProtocols.Acn.Rdm.Packets.Net;
using System.ComponentModel;
using LXProtocols.Acn.Rdm.Packets.Product;
using System.Threading;
using LXProtocols.Acn.Rdm.Broker;
using LXProtocols.Acn.Rdm.Packets.DMX;
using LXProtocols.Acn.Rdm.Packets.Control;
using LXProtocols.Acn.Rdm.Packets.Configuration;
using LXProtocols.Acn.Rdm.Packets.Power;
using RdmSnoop;
using LXProtocols.Acn.Rdm.Packets.Parameters;
using RdmSnoop.Brokers;
using System.Runtime.CompilerServices;

namespace RdmNetworkMonitor
{
    public class RdmDeviceBroker:RdmMessageBroker, INotifyPropertyChanged
    {
        IEnumerable<IRdmSocket> sockets = null;

        #region Setup and Iniiaalisation
               
        public RdmDeviceBroker(IEnumerable<IRdmSocket> sockets, UId id, RdmEndPoint address)
        {
            Id = id;
            Address = address;
            this.sockets = sockets;

            RegisterHandlers(this);

            foreach (var socket in sockets)
            {
                socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
            }

        }

        #endregion

        #region Data Model
        
        #region Information

        [Category("Information")]
        public UId Id { get; protected set; }

        [Category("Information")]
        public RdmEndPoint Address { get; protected set; }

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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    foreach (var socket in sockets)
                        socket.SendRdm(setLabel, Address, Id);

                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    SetDmxAddress(value);
                }
            }
        }

        private int selectedMode = 0;

        [Category("DMX")]
        public int SelectedMode
        {
            get { return selectedMode; }
            protected set
            {
                if (selectedMode != value)
                {
                    selectedMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int? availableModes = null;

        [Category("DMX")]
        public int AvailableModes
        {
            get 
            {
                if (availableModes == null && deviceInformation != null)
                    return deviceInformation.DmxPersonalityCount;

                return availableModes ?? 0;
            }
            protected set
            {
                if (availableModes != value)
                {
                    availableModes = value;
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    foreach (var socket in sockets)
                        socket.SendRdm(invert, Address, Id);

                    RaisePropertyChanged();
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
                    foreach (var socket in sockets)
                        socket.SendRdm(invert, Address, Id);

                    RaisePropertyChanged();
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
                    foreach (var socket in sockets)
                        socket.SendRdm(invert, Address, Id);

                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Parameters

        private Dictionary<RdmParameters, ParameterInformation> parameters = new Dictionary<RdmParameters, ParameterInformation>();

        public List<ParameterInformation> Parameters
        {
            get { return new List<ParameterInformation>(parameters.Values); }
        }

        #endregion

        #endregion

        #region Commands

        public void Identify(bool enabled)
        {
            IdentifyDevice.Set identify = new IdentifyDevice.Set();
            identify.IdentifyEnabled = enabled;
            foreach (var socket in sockets)
                socket.SendRdm(identify, Address, Id);
        }

        public void SetDmxAddress(int address)
        {
            DmxStartAddress.Set setAddress = new DmxStartAddress.Set();
            setAddress.DmxAddress = (short) address;
            foreach (var socket in sockets)
                socket.SendRdm(setAddress, Address, Id);
        }

        public void SetMode(int mode)
        {
            DmxPersonality.Set setMode = new DmxPersonality.Set();
            setMode.PersonalityIndex = (byte)mode;
            foreach (var socket in sockets)
                socket.SendRdm(setMode, Address, Id);
        }

        public void Reset()
        {
            ResetDevice.Set reset = new ResetDevice.Set();
            foreach (var socket in sockets)
                socket.SendRdm(reset, Address, Id);
        }

        public void SelfTest()
        {
            PerformSelfTest.Set test = new PerformSelfTest.Set();
            foreach (var socket in sockets)
                socket.SendRdm(test, Address, Id);
        }

        public void Power(PowerState.States state)
        {
            PowerState.Set power = new PowerState.Set();
            power.State = state;
            foreach (var socket in sockets)
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

            SetParameterStatus(e.Packet.Header.ParameterId, ParameterStatus.Valid);

            if (replyPacket != null)
            {
                foreach (var socket in sockets)
                    socket.SendRdm(replyPacket, Address, Id);
            }

            SupportedParameters.GetReply parameterReply = e.Packet as SupportedParameters.GetReply;
            if (parameterReply != null)
            {
                RequestDetails(parameterReply);
                RequestLabel(parameterReply);
                RequestConfiguration(parameterReply);
                RequestHistory(parameterReply);
                RequestPersonality(parameterReply);
            }

            DeviceInfo.GetReply info = e.Packet as DeviceInfo.GetReply;
            if (info != null)
            {
                DeviceInformation = info;

                DmxPersonalityDescription.Get packet = new DmxPersonalityDescription.Get();
                packet.PersonalityIndex = DeviceInformation.DmxPersonality;
                SendRdm(packet);

                if (!SubDeviceUId.IsSubDevice(Id))
                {
                    if (SubDevices.Count != info.SubDeviceCount)
                    {
                        SubDevices.Clear();
                        for (short n = 1; n <= info.SubDeviceCount; n++)
                        {
                            RdmDeviceBroker subDeviceBroker = new RdmDeviceBroker(sockets, new SubDeviceUId(Id, n), Address);
                            SubDevices.Add(subDeviceBroker);
                            subDeviceBroker.Interogate();
                        }
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

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DmxPersonality)]
        private RdmPacket ProcessPersonality(RdmPacket packet)
        {
            DmxPersonality.GetReply response = packet as DmxPersonality.GetReply;
            if (response != null)
            {
                SelectedMode = (int) response.CurrentPersonalityIndex;
                AvailableModes = (int)response.PersonalityCount;

                return new DmxPersonalityDescription.Get()
                {
                    PersonalityIndex = response.CurrentPersonalityIndex
                };
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DmxPersonalityDescription)]
        private RdmPacket ProcessPersonalityDescription(RdmPacket packet)
        {
            DmxPersonalityDescription.GetReply response = packet as DmxPersonalityDescription.GetReply;
            if (response != null)
            {
                if(response.PersonalityIndex == SelectedMode)
                {
                    Mode = response.Description;
                }
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
                    foreach (var socket in sockets)
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

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SupportedParameters)]
        private RdmPacket ProcessSupportedParameters(RdmPacket packet)
        {
            SupportedParameters.GetReply response = packet as SupportedParameters.GetReply;
            if (response != null)
            {
                if(response.HasParameter( RdmParameters.ParameterDescription))
                {
                    foreach (RdmParameters pid in response.ParameterIds.Where(item => item.IsManufacturerPID()))
                    {
                        ParameterDescription.Get descriptionPacket = new ParameterDescription.Get();
                        descriptionPacket.ParameterId = pid;
                        foreach (var socket in sockets)
                            socket.SendRdm(descriptionPacket, Address, Id);
                    }
                }
                else
                {
                    parameters = response.ParameterIds.Select(item => new ParameterInformation(item)).ToDictionary(item => item.ParameterId);
                }

            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.ParameterDescription)]
        private RdmPacket ProcessParameterDescription(RdmPacket packet)
        {
            ParameterDescription.GetReply response = packet as ParameterDescription.GetReply;
            if (response != null)
            {
                parameters[response.ParameterId] = new ParameterInformation(response);
            }

            return null;
        }

        #endregion

        protected void SendRdm(RdmPacket packet)
        {
            if (IsParameterStatus(packet.Header.ParameterId, ParameterStatus.Empty))
            {
                SetParameterStatus(packet.Header.ParameterId, ParameterStatus.Pending);
                //packet.Header.PortOrResponseType = (byte) Address.Universe;
                foreach(var socket in sockets)
                    socket.SendRdm(packet, Address, Id);
            }
        }
        

        public void Interogate()
        {
            RequestParameters();
            RequestDeviceInformation();

            //Only for RDMNet Endpoint Zero devices.
            if (Address.Universe == 0)
            {
                //Get a list of endpoints supported by this device.
                EndpointList.Get getPorts = new EndpointList.Get();
                SendRdm(getPorts);
            }
        }

        public void RequestDeviceInformation()
        {
            DeviceInfo.Get getInfo = new DeviceInfo.Get();
            SendRdm(getInfo);
        }

        public void RequestDetails(SupportedParameters.GetReply supportedParameters)
        {
            if(supportedParameters.HasParameter(RdmParameters.ManufacturerLabel))
            {
                ManufacturerLabel.Get manufacturer = new ManufacturerLabel.Get();
                SendRdm(manufacturer);
            }

            if (supportedParameters.HasParameter(RdmParameters.DeviceModelDescription))
            {
                DeviceModelDescription.Get model = new DeviceModelDescription.Get();
                SendRdm(model);
            }

            DmxStartAddress.Get dmxAddress = new DmxStartAddress.Get();
            SendRdm(dmxAddress);
        }

        public void RequestLabel(SupportedParameters.GetReply supportedParameters)
        {
            if (supportedParameters.HasParameter(RdmParameters.DeviceLabel))
            {
                DeviceLabel.Get label = new DeviceLabel.Get();
                SendRdm(label);
            }
        }

        public void RequestDmxInformation(SupportedParameters.GetReply supportedParameters)
        {
            if (supportedParameters.HasParameter(RdmParameters.ManufacturerLabel))
            {
                ManufacturerLabel.Get manufacturer = new ManufacturerLabel.Get();
                SendRdm(manufacturer);
            }

            if (supportedParameters.HasParameter(RdmParameters.DeviceModelDescription))
            {
                DeviceModelDescription.Get model = new DeviceModelDescription.Get();
                SendRdm(model);
            }
        }

        public void RequestConfiguration(SupportedParameters.GetReply supportedParameters)
        {
            if (supportedParameters.HasParameter(RdmParameters.PanInvert))
            {
                PanInvert.Get pan = new PanInvert.Get();
                SendRdm(pan);
            }

            if (supportedParameters.HasParameter(RdmParameters.TiltInvert))
            {
                TiltInvert.Get tilt = new TiltInvert.Get();
                SendRdm(tilt);
            }

            if (supportedParameters.HasParameter(RdmParameters.PanTiltSwap))
            {
                PanTiltSwap.Get swap = new PanTiltSwap.Get();
                SendRdm(swap);
            }
        }

        public void RequestHistory(SupportedParameters.GetReply supportedParameters)
        {
            if (supportedParameters.HasParameter(RdmParameters.DeviceHours))
            {
                DeviceHours.Get hours = new DeviceHours.Get();
                SendRdm(hours);
            }

            if (supportedParameters.HasParameter(RdmParameters.DevicePowerCycles))
            {
                DevicePowerCycles.Get cycles = new DevicePowerCycles.Get();
                SendRdm(cycles);
            }

            if (supportedParameters.HasParameter(RdmParameters.LampHours))
            {
                LampHours.Get lampHours = new LampHours.Get();
                SendRdm(lampHours);
            }

            if (supportedParameters.HasParameter(RdmParameters.LampStrikes))
            {
                LampStrikes.Get lampStrikes = new LampStrikes.Get();
                SendRdm(lampStrikes);
            }
        }

        public void RequestPersonality(SupportedParameters.GetReply supportedParameters)
        {
            if (supportedParameters.HasParameter(RdmParameters.DmxPersonality))
            {
                DmxPersonality.Get packet = new DmxPersonality.Get();
                SendRdm(packet);
            }

            if (supportedParameters.HasParameter(RdmParameters.SlotInfo))
            {
                SlotInfo.Get slotPacket = new SlotInfo.Get();
                SendRdm(slotPacket);
            }
        }

        public void RequestParameters()
        {
            SupportedParameters.Get packet = new SupportedParameters.Get();
            SendRdm(packet);
        }

        #endregion


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        
    }
}
