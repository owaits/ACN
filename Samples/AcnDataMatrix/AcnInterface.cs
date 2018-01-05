using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acn.Sockets;
using System.Net;
using Acn.Packets.sAcn;
using System.Net.NetworkInformation;
using Acn;
using Acn.Helpers;
using Acn.Rdm.Packets.Net;
using System.Collections.ObjectModel;
namespace AcnDataMatrix
{
    public class AcnInterface
    {
        #region ACN Declares
        //Streaing ACN bits
        private StreamingAcnSocket socket;// = new StreamingAcnSocket(Guid.NewGuid(), "Streaming ACN Snoop");
        private DmxStreamer dmxOutput;
        private DmxUniverseData recieveData = new DmxUniverseData();
        private RdmNetEndPointExplorer acnPortExplorer;
        private DmxUniverseData sendData = new DmxUniverseData();

        ObservableCollection<CardInfo> cards = new ObservableCollection<CardInfo>();
        private int selectedUniverse = 1;
        private int selectedChannel = 1;


        public int SelectedChannel
        {
            get { return selectedChannel; }
            set { selectedChannel = value; }
        }
        public int SelectedUniverse
        {
            get { return selectedUniverse; }
            set
            {
                if (selectedUniverse != value)
                {
                    selectedUniverse = value;

                    foreach (int universe in new List<int>(socket.DmxUniverses))
                    {
                        socket.DropDmxUniverse(universe);
                        dmxOutput.RemoveUniverse(universe);
                    }

                    socket.JoinDmxUniverse(selectedUniverse);

                    sendData.Universe = new DmxUniverse(selectedUniverse);
                    dmxOutput.AddUniverse(sendData.Universe);
                }
            }
        }
        #endregion
    }
}
