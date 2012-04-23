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

namespace RdmNetworkMonitor
{
    public class RdmDeviceBroker:INotifyPropertyChanged
    {
        IRdmSocket socket = null;

        public RdmDeviceBroker(IRdmSocket socket, UId id, RdmAddress address)
        {
            Id = id;
            Address = address;
            this.socket = socket;

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
        }

        #region Information

        public UId Id { get; set; }

        public RdmAddress Address { get; set; }

        private List<short> ports = new List<short>();

        public List<short> Ports
        {
            get { return ports; }
            set 
            {
                if (ports != value)
                {
                    ports = value;
                    RaisePropertyChanged("Ports");
                }
            }
        }

        private string manufacturer = string.Empty;

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

        public string Label
        {
            get { return label; }
            set 
            {
                if (label != value)
                {
                    label = value;
                    RaisePropertyChanged("Label");
                }
            }
        }

        #endregion

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (e.Packet.Header.SourceId.Equals(Id))
            {
                DeviceInfo.GetReply info = e.Packet as DeviceInfo.GetReply;
                if (info != null)
                {
                    RequestDetails();
                    RequestLabel();
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
                    Label = label.Label;
                }

                PortList.Reply ports = e.Packet as PortList.Reply;
                if (ports != null)
                {
                    Ports = ports.PortNumbers;
                }
            }
        }

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
        }

        public void RequestLabel()
        {
            DeviceLabel.Get label = new DeviceLabel.Get();
            socket.SendRdm(label, Address, Id);
        }

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
