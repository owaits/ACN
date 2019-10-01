using LXProtocols.TCNet;
using LXProtocols.TCNet.Packets;
using LXProtocols.TCNet.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LXProtocols.TCNetViewer
{
    public partial class ProDJTapViewer : Form
    {
        private TCNetSocket socket= new TCNetSocket(NodeType.Slave);

        public ProDJTapViewer()
        {
            InitializeComponent();

            downloadLayerSelect.SelectedIndex = 1;
            layerSelect.SelectedIndex = 1;

            CardInfo firstCard = null;
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        CardInfo card = new CardInfo(adapter, n);
                        if (card.IpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            networkCardSelect.Items.Add(card);
                        }

                        firstCard = card;
                    }
                }
            }

            if (firstCard != null)
                Start(firstCard);
        }

        private void Start(CardInfo networkCard)
        {
            socket = new TCNetSocket(NodeType.Slave);
            socket.NodeName = "TITAN01";
            socket.VendorName = "AVOLITES";
            socket.DeviceName = "TITAN";
            socket.DeviceVersion = Assembly.GetEntryAssembly().GetName().Version;
            socket.NewPacket += socket_NewPacket;
            socket.NewDeviceFound += Socket_NewDeviceFound;
            socket.DeviceLost += Socket_DeviceLost;

            socket.Open(networkCard.IpAddress,networkCard.SubnetMask);

        }

        private void Socket_DeviceLost(object sender, TCNetDeviceEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<TCNetDeviceEventArgs>(Socket_NewDeviceFound), sender, e);
                return;
            }

            deviceSelect.Items.Remove(e.Device);
        }

        private void Socket_NewDeviceFound(object sender, TCNetDeviceEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler<TCNetDeviceEventArgs>(Socket_NewDeviceFound), sender, e);
                return;
            }

            if (e.Device.NodeType != NodeType.Slave)
            {
                deviceSelect.Items.Add(e.Device);

                if (deviceSelect.Items.Count == 1)
                    deviceSelect.SelectedItem = e.Device;
            }
                
        }

        private void Stop()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }

        private TCNetTime timecodePacket = null;

        public TCNetTime TimecodePacket
        {
            get { return timecodePacket;  }
            set
            {
                if(timecodePacket != value)
                {
                    timecodePacket = value;

                    if (InvokeRequired)
                        BeginInvoke(new Action(UpdateTCInfo));
                }
            }
        }

        private void UpdateTCInfo()
        {
            tcInfo.SelectedObject = TimecodePacket;
            tcInfo.ExpandAllGridItems();
        }

        void socket_NewPacket(object sender, NewPacketEventArgs<TCNetPacket> e)
        {
            if(e.Packet is TCNetTime)
                TimecodePacket = (TCNetTime) e.Packet;
            if (e.Packet is TCNetError)
                BeginInvoke(new Action<TCNetError>(ProcessError),e.Packet);

            if (e.Packet is TCNetBigWaveform)
                DumpWaveform("BigWaveform.txt",((TCNetBigWaveform) e.Packet).WaveformData);

            if (e.Packet is TCNetSmallWaveform)
                DumpWaveform("SmallWaveform.txt", ((TCNetSmallWaveform)e.Packet).WaveformData);


        }

        private void DumpWaveform(string filename, byte[] data)
        {
            File.AppendAllText(@"d:\temp\" + filename, string.Join(",", data.Select(b => "0x" + b.ToString("X"))));
        }

        private void ProcessError(TCNetError error)
        {
            //Filter out authentication success errors.
            if(error.Code != 0xFF)
                 MessageBox.Show($"Error {error.Code} reported on layer {error.LayerID} for data {error.DataType}");
        }

        private void networkCardSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            CardInfo networkCard = networkCardSelect.SelectedItem as CardInfo;

            if (networkCard != null)
            {
                try
                {
                    Stop();
                    Start(networkCard);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Unable to use selected Network interface.");
                }

            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;
            socket.Send(device, new TCNetControl()
            {
                Step = TCNetControl.Steps.Initialize,
                ControlPath = $"layer/{layerSelect.SelectedIndex}/state={((int)DeckState.Playing).ToString()};"
            });
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;
            socket.Send(device, new TCNetControl()
            {
                Step = TCNetControl.Steps.Initialize,
                ControlPath = $"layer/{layerSelect.SelectedIndex}/state={((int) DeckState.Paused).ToString()};"
            });
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;
            socket.Send(device, new TCNetControl()
            {
                Step = TCNetControl.Steps.Initialize,
                ControlPath = $"layer/{layerSelect.SelectedIndex}/state={((int) DeckState.Stopped).ToString()};"
            });
        }

        private void MetricsDownload_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;
            socket.Send(device, new TCNetDataRequest()
            {
                DataType = DataTypes.Metrics,
                LayerID = (byte)downloadLayerSelect.SelectedIndex
            });
        }

        private void DownloadMetaData_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;
            socket.Send(device, new TCNetDataRequest()
            {
                DataType = DataTypes.MetaData,
                LayerID = (byte)downloadLayerSelect.SelectedIndex
            });
        }

        private void DownloadSmallWaveform_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;

            if (device == null)
                MessageBox.Show("Please select a device?");

            socket.Send(device, new TCNetDataRequest()
            {
                DataType = DataTypes.SmallWaveform,
                LayerID = (byte)downloadLayerSelect.SelectedIndex
            });

        }

        private void DownloadBigWaveform_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;
            socket.Send(device, new TCNetDataRequest()
            {
                DataType = DataTypes.BigWaveform,
                LayerID = (byte)downloadLayerSelect.SelectedIndex
            });
        }

        private void BeatGrid_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;

            if (device == null)
                MessageBox.Show("Please select a device?");

            socket.Send(device, new TCNetDataRequest()
            {
                DataType = DataTypes.BeatGrid,
                LayerID = (byte)downloadLayerSelect.SelectedIndex
            });
        }

        private void CueData_Click(object sender, EventArgs e)
        {
            TCNetDevice device = deviceSelect.SelectedItem as TCNetDevice;

            if (device == null)
                MessageBox.Show("Please select a device?");

            socket.Send(device, new TCNetDataRequest()
            {
                DataType = DataTypes.Cue,
                LayerID = (byte)downloadLayerSelect.SelectedIndex
            });
        }
    }
}
