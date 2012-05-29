using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Routing
{
    public class RdmRouteBinding
    {
        #region Setup and Initialisation

        public RdmRouteBinding(IRdmTransport transport, string name, string description, int priority)
        {
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
            set { transport = value; }
        }

    }
}
