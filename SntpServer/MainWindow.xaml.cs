using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using System.Net;

namespace SntpServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        LXProtocols.Acn.Sntp.SntpServer server;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Grab an IP address - need to write a chooser for this
            var localAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .FirstOrDefault()
                .Address;

            ListenAddress = localAddress;

            server = new LXProtocols.Acn.Sntp.SntpServer() { NetworkAdapter = localAddress };
            server.Open();

            spinner.IsIndeterminate = server.IsOpen();
           
        }



        public IPAddress ListenAddress
        {
            get { return (IPAddress)GetValue(ListenAddressProperty); }
            set { SetValue(ListenAddressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListenAddress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListenAddressProperty =
            DependencyProperty.Register("ListenAddress", typeof(IPAddress), typeof(MainWindow), new UIPropertyMetadata(null));



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server.Close();
        }
    }
}
