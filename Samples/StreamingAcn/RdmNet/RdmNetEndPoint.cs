using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Acn.Rdm;
using Acn.Sockets;
using Acn.Rdm.Packets.Net;
using System.Threading;

namespace StreamingAcn
{
    public class RdmNetEndPoint : RdmEndPoint, INotifyPropertyChanged
    {
        private SynchronizationContext threadContext = null;

        public RdmNetEndPoint(IPEndPoint ipEndPoint, int endpointId)
            : base(ipEndPoint, endpointId)
        {
            threadContext = SynchronizationContext.Current;
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

        public string portLabel = null;

        public string PortLabel
        {
            get { return portLabel; }
            set
            {
                if (portLabel != value)
                {
                    string oldValue = portLabel;                    

                    portLabel = value;

                    RaisePropertySet("PortLabel", oldValue, value);
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

        public EndpointMode.EndpointModes? direction = null;

        public EndpointMode.EndpointModes? Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    EndpointMode.EndpointModes? oldValue = direction;                    

                    direction = value;

                    RaisePropertySet("Direction", oldValue, value);
                    RaisePropertyChanged("Direction");
                }
            }
        }

        public int? acnUniverse = null;

        public int? AcnUniverse
        {
            get { return acnUniverse; }
            set
            {
                if (acnUniverse != value)
                {
                    int? oldValue = acnUniverse;
                    

                    acnUniverse = value;
                    Patched = (acnUniverse == 0 ? "No" : "Yes");

                    RaisePropertySet("AcnUniverse", oldValue, value);
                    RaisePropertyChanged("AcnUniverse");
                }
            }
        }

        public event PropertyChangedEventHandler PropertySet;

        protected void RaisePropertySet(string propertyName, object oldValue, object newValue)
        {
            if(oldValue != null)
            {
                if (PropertySet != null)
                    PropertySet(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            threadContext.Post(new SendOrPostCallback((state) =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }),null);
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
