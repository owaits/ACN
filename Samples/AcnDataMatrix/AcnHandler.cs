using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LXProtocols.Acn.Sockets;
using System.Net;
using LXProtocols.Acn.Packets.sAcn;
using System.Net.NetworkInformation;
using LXProtocols.Acn;
using LXProtocols.Acn.Helpers;
using LXProtocols.Acn.Rdm.Packets.Net;
using System.ComponentModel;

namespace AcnDataMatrix
{
    public class AcnHandler : IDisposable
    {
        public AcnHandler(CardInfo networkCard, int startUniverse, int universeCount)
        {
            List<int> universes = new List<int>();
            for (int i = startUniverse; i < startUniverse + universeCount; i++)
            {
                universes.Add(i);
            }
            Start(networkCard, universes);
        }
        
        public AcnHandler(CardInfo networkCard, IEnumerable<int> universes)
        {
            Start(networkCard, universes);
            
        }

        public void Dispose()
        {
            Stop();
        }       

        //Streaing ACN bits
        private List<StreamingAcnSocket> sockets;
        private DmxUniverseData recieveData = new DmxUniverseData();
        private RdmNetEndPointExplorer acnPortExplorer;
        private DmxUniverseData sendData = new DmxUniverseData();
        public IProtocolFilter callBackClass;

        /*public StreamingAcnSocket Socket
        {
            get;
        }*/

        public List<StreamingAcnSocket> Sockets
        {
            get { return sockets;  }
            set { sockets = value;  }
        }
        

        #region Setup and Initialisation

        private void Start(CardInfo networkCard, IEnumerable<int> universes)
        {
            //socket = new StreamingAcnSocket(Guid.NewGuid(), "Acn Data Matrix", callBackClass);
            
            //socket.Open(networkCard.IpAddress);

            Sockets = new List<StreamingAcnSocket>();
            foreach (int universe in universes)
            {
                StreamingAcnSocket newSocket = new StreamingAcnSocket(Guid.NewGuid(), "Acn Data Matrix");
                newSocket.Open(networkCard.IpAddress);
                newSocket.JoinDmxUniverse(universe);
                Sockets.Add(newSocket);
            }

            acnPortExplorer = new RdmNetEndPointExplorer();
            acnPortExplorer.LocalAdapter = networkCard.IpAddress;
            acnPortExplorer.NewEndpointFound += acnPortExplorer_NewEndpointFound;
            acnPortExplorer.Start();

        }

        private void Stop()
        {

            if (acnPortExplorer != null)
            {
                acnPortExplorer.Stop();
                acnPortExplorer = null;
            }
                       
            if (Sockets != null)
            {
                foreach (StreamingAcnSocket curr in Sockets)
                {
                    curr.Close();
                }
                Sockets = null;
            }

        }


        #endregion

        #region RDMNet Ports

        private BindingList<RdmNetEndPoint> ports = new BindingList<RdmNetEndPoint>();

        void acnPortExplorer_NewEndpointFound(object sender, EventArgs e)
        {
            UpdatePorts();
        }

        private delegate void UpdatePortsHandler();

        private void UpdatePorts()
        {
            ports.Clear();
            foreach (RdmNetEndPoint port in acnPortExplorer.DiscoveredEndpoints)
                ports.Add(port);
        }

        #endregion

        void socket_NewPacket(object sender, NewPacketEventArgs<StreamingAcnDmxPacket> e)
        {
            StreamingAcnDmxPacket dmxPacket = e.Packet as StreamingAcnDmxPacket;
            
        }
    }
}
