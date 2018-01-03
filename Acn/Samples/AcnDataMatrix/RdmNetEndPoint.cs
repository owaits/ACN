using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel;
using Acn.Rdm;
using Acn.Sockets;
using Acn.Rdm.Packets.Net;

namespace AcnDataMatrix
{
    public class RdmNetEndPoint : RdmEndPoint, INotifyPropertyChanged
    {
        public RdmNetEndPoint(IPEndPoint ipEndPoint, int endpointId)
            : base(ipEndPoint, endpointId)
        {
        }

        private string manufacturerLabel = string.Empty;

        public string ManufacturerLabel
        {
            get { return manufacturerLabel; }
            set
            {
                if (manufacturerLabel != value)
                {
                    manufacturerLabel = value;
                    RaisePropertyChanged("ManufacturerLabel");
                }
            }
        }


        public string deviceLabel = string.Empty;

        public string DeviceLabel
        {
            get { return deviceLabel; }
            set
            {
                if (deviceLabel != value)
                {
                    deviceLabel = value;
                    RaisePropertyChanged("DeviceLabel");
                }
            }
        }

        public string portLabel = string.Empty;

        public string PortLabel
        {
            get { return portLabel; }
            set
            {
                if (portLabel != value)
                {
                    portLabel = value;
                    RaisePropertyChanged("PortLabel");
                }
            }
        }

        public string patched = "No";

        public string Patched
        {
            get { return patched; }
            set
            {
                if (patched != value)
                {
                    patched = value;
                    RaisePropertyChanged("Patched");
                }
            }
        }

        public bool identify = false;

        public bool Identify
        {
            get { return identify; }
            set
            {
                if (identify != value)
                {
                    identify = value;
                    RaisePropertyChanged("Identify");
                }
            }
        }

        public EndpointMode.EndpointModes direction = EndpointMode.EndpointModes.Disabled;

        public EndpointMode.EndpointModes Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    RaisePropertyChanged("Direction");
                }
            }
        }

        public int acnUniverse = 0;

        public int AcnUniverse
        {
            get { return acnUniverse; }
            set
            {
                if (acnUniverse != value)
                {
                    acnUniverse = value;
                    Patched = (acnUniverse == 0 ? "No" : "Yes");

                    RaisePropertyChanged("AcnUniverse");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RdmNetEndpointComparer : IEqualityComparer<RdmNetEndPoint>
    {

        public bool Equals(RdmNetEndPoint x, RdmNetEndPoint y)
        {
            return x.Id.Equals(y.Id) && x.Universe.Equals(y.Universe);
        }

        public int GetHashCode(RdmNetEndPoint obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
