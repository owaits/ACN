using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Routing
{
    public class RdmRouteBinding
    {
        private RdmRouter parent = null;

        #region Setup and Initialisation

        public RdmRouteBinding(RdmRouter parent, IRdmTransport transport, string name, string description, int priority)
        {
            this.parent = parent;

            Transport = transport;
            Name = name;
            Description = description;
            Priority = priority;
        }

        #endregion

        #region Information

        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        #endregion

        private IRdmTransport transport = null;

        public IRdmTransport Transport
        {
            get { return transport; }
            set 
            {
                if (transport != value)
                {
                    if (transport != null)
                    {
                        parent.UnBind(this);
                        transport.Starting -= transport_Started;
                        transport.Stoping -=transport_Stopped;
                    }

                    transport = value;

                    if (transport != null)
                    {
                        parent.Bind(this);
                        transport.Starting += transport_Started;
                        transport.Stoping += transport_Stopped;
                    }
                }                
            }
        }

        void transport_Stopped(object sender, EventArgs e)
        {
            parent.UnBind(this);
        }

        void transport_Started(object sender, EventArgs e)
        {
            parent.Bind(this);
        }

    }
}
