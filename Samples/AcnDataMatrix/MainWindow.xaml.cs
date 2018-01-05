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
using System.IO;
using Acn;
using Acn.Helpers;
using Acn.Sockets;
using System.Net;
using Acn.Packets.sAcn;
using Acn.Rdm.Packets.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Threading;
using Acn.IO;

namespace AcnDataMatrix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingsViewModel settings;
        private AcnHandler adapter1Handler;
        private AcnHandler adapter2Handler;
        private MatrixWindow example;
        private System.Timers.Timer aTimer;
        private Thread gameThread;
        private Thread socket1Thread;
        private Thread socket2Thread;
        int universes = 0;
        StreamWriter debugInput = new StreamWriter("debug.txt");
        private AutoResetEvent windowReady = new AutoResetEvent(false);

        Dictionary<int, byte> sequenceChecker = new Dictionary<int, byte>();

        public SettingsViewModel Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStartApp_Click(object sender, RoutedEventArgs e)
        {
                windowReady.Reset();
                if (Settings.Net1Enabled)
                {
                    universes += Settings.Net1UniverseCount;
                }

                if (Settings.Net2Enabled)
                {
                    universes += Settings.Net2UniverseCount;
                }

                gameThread = new Thread(GameWindowThread);
                gameThread.IsBackground = true;
                gameThread.Start();
                windowReady.WaitOne();    

                if (Settings.Net1Enabled)
                {
                    socket1Thread = new Thread(() => {
                        adapter1Handler = new AcnHandler(Settings.Net1, Settings.Net1StartUniverse, Settings.Net1UniverseCount);
                        foreach (StreamingAcnSocket socket in adapter1Handler.Sockets)
                        {
                            socket.NewPacket += new EventHandler<NewPacketEventArgs<Acn.Packets.sAcn.StreamingAcnDmxPacket>>(socket_NewPacket);
                        }
                    });
                    socket1Thread.Start();
                }

                if (Settings.Net2Enabled)
                {
                    socket2Thread = new Thread(() => {
                        adapter2Handler = new AcnHandler(Settings.Net2, Settings.Net2StartUniverse, Settings.Net2UniverseCount);
                        foreach (StreamingAcnSocket socket in adapter2Handler.Sockets)
                        {
                            socket.NewPacket += new EventHandler<NewPacketEventArgs<Acn.Packets.sAcn.StreamingAcnDmxPacket>>(socket_NewPacket);
                        }
                    });
                    socket2Thread.Start();
                    
                }
                
                // Create a timer with a 500ms second interval.
                aTimer = new System.Timers.Timer(500);

                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

                aTimer.Start();
            
        }

        private void Socket_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void GameWindowThread()
        {
            example = new MatrixWindow(universes, true, "ACN Matrix");
            example.Unload += new EventHandler<EventArgs>(StopTimer);
            windowReady.Set();
            example.Run();
            
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //load settings
            if (File.Exists("settings.bin"))
            {
                Settings = loadSettings("settings.bin");
            }
            else
            {
                Settings = new SettingsViewModel();
            }

            grdNetwork.DataContext = Settings;
            lostPacket.DataContext = Settings;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //save settings
            saveSettings("settings.bin", Settings);

            if (gameThread != null)
            {
                gameThread.Join();

            }

            if (socket1Thread != null)
            {
                socket1Thread.Join();

            }

            if (socket2Thread != null)
            {
                socket2Thread.Join();
            }

            //debugInput.Flush();
            debugInput.Close();
        }

        public void saveSettings(string fileName, SettingsViewModel settings)
        {
            FileStream settingStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            SerializeableNetworkSettings netSettings = new SerializeableNetworkSettings(Settings);
            bf.Serialize(settingStream, netSettings);
            settingStream.Close();
        }

        public SettingsViewModel loadSettings(string fileName)
        {
            try
            {
                FileStream settingStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                SerializeableNetworkSettings settings = (SerializeableNetworkSettings)bf.Deserialize(settingStream);
                settingStream.Close();
                SettingsViewModel tempSettings = new SettingsViewModel();
                tempSettings.LoadSetting(settings);
                return tempSettings;
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                return new SettingsViewModel();
            }
        }


        void socket_NewPacket(object sender, NewPacketEventArgs<StreamingAcnDmxPacket> e)
        {
            StreamingAcnDmxPacket dmxPacket = e.Packet as StreamingAcnDmxPacket;
            if (example != null)
            {
                example.UpdateData(dmxPacket.Framing.Universe - 1, dmxPacket.Dmx.Data);               
            }            
        }       

        // Specify what you want to happen when the Elapsed event is  
        // raised. 
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (example != null)
            {
                if (example.UpdateTime1 != 0)
                {
                    Settings.FrameRate = 1000 / example.UpdateTime1;
                }
                else
                {
                    Settings.FrameRate = 1000 ;
                }
            }
        }
        //private event EventHandler StopTimer;

        private void StopTimer(object source, EventArgs e)
        {
            if (aTimer != null)
            {
                aTimer.Stop();
                aTimer.Close();
            }
        }

        private void ExceptionHandler()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (example != null)
            {
                example.Width = Settings.Width;
                example.Height = Settings.Height;
            }
        }
    }
}
