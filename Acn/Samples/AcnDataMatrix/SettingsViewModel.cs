using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Acn;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Acn.Helpers;
using Acn.Rdm.Packets.Net;
using System.Collections.ObjectModel;

namespace AcnDataMatrix
{
    /// <summary>
    /// Serializable network set-up.
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            SetUpSettingsDefault();
            
        }

        public void SetUpSettingsDefault()
        {
            //populate the observable collection with the card info. Populate Net Interfaces by default with IPv4 adapters.
            NetworkCards = new ObservableCollection<CardInfo>();
            int index = 0;
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        CardInfo card = new CardInfo(adapter, n);
                        NetworkCards.Add(card);

                        if (card.IpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (index == 0)
                            {
                                Net1 = card;
                                index++;
                            }
                            else if (index == 1)
                            {
                                Net2 = card;
                                index++;
                            }
                        }

                    }


                }
            }

            //set up network output defaults
            Net1StartUniverse = 1;
            Net1UniverseCount = 32;
            Net1Enabled = true;
            Net2StartUniverse = 1;
            Net2UniverseCount = 32;
            Net2Enabled = true;
        }

        public void LoadSetting(SerializeableNetworkSettings loadedSettings)
        {
            SetUpSettingsDefault();


            //Set up the interfaces and tell the UI they have changed just in case.
            Net1.SetUpInterfaceFromId(loadedSettings.Net1AdapterId); RaisePropertyChangedEvent("Net1Enabled");
            Net2.SetUpInterfaceFromId(loadedSettings.Net2AdapterId); RaisePropertyChangedEvent("Net2Enabled");
            
            
            //set up network output defaults
            Net1StartUniverse = loadedSettings.Net1StartUniverse;
            Net1UniverseCount = loadedSettings.Net1UniverseCount;
            Net1Enabled = loadedSettings.Net1Enabled;
            Net2StartUniverse = loadedSettings.Net2StartUniverse;
            Net2UniverseCount = loadedSettings.Net2UniverseCount;
            Net2Enabled = loadedSettings.Net2Enabled;

            //load width height and aspect lock
            Width = loadedSettings.Width;
            Height = loadedSettings.Height;
            LockAspect = loadedSettings.LockAspect;
        }

        private bool net1Enabled;

        public bool Net1Enabled
        {
            get { return net1Enabled; }
            set { if (net1Enabled != value) { net1Enabled = value; RaisePropertyChangedEvent("Net1Enabled"); } }
        }

        private CardInfo net1;

        public CardInfo Net1
        {
            get { return net1; }
            set { if (net1 != value) { net1 = value; RaisePropertyChangedEvent("Net1"); } }
        }

        private int net1StartUniverse;

        public int Net1StartUniverse
        {
            get { return net1StartUniverse; }
            set { if (net1StartUniverse != value) { net1StartUniverse = value; RaisePropertyChangedEvent("Net1StartUniverse"); } }
        }

        private int net1UniverseCount;

        public int Net1UniverseCount
        {
            get { return net1UniverseCount; }
            set { if (net1UniverseCount != value) { net1UniverseCount = value; RaisePropertyChangedEvent("Net1UniverseCount"); } }
        }

        private bool net2Enabled;

        public bool Net2Enabled
        {
            get { return net2Enabled; }
            set { if (net2Enabled != value) { net2Enabled = value; RaisePropertyChangedEvent("Net2Enabled"); } }
        }

        private CardInfo net2;

        public CardInfo Net2
        {
            get { return net2; }
            set { if (net2 != value) { net2 = value; RaisePropertyChangedEvent("Net2"); } }
        }

        private int net2StartUniverse;

        public int Net2StartUniverse
        {
            get { return net2StartUniverse; }
            set { if (net2StartUniverse != value) { net2StartUniverse = value; RaisePropertyChangedEvent("Net2StartUniverse"); } }
        }

        private int net2UniverseCount;

        public int Net2UniverseCount
        {
            get { return net2UniverseCount; }
            set { if (net2UniverseCount != value) { net2UniverseCount = value; RaisePropertyChangedEvent("Net2UniverseCount"); } }
        }

        private ObservableCollection<CardInfo> networkCards;

        public ObservableCollection<CardInfo> NetworkCards
        {
            get { return networkCards; }
            set { networkCards = value; }
        }

        private double frameRate = 0;
        public double FrameRate
        {
            get { return frameRate; }
            set { if (frameRate != value) { frameRate = value; RaisePropertyChangedEvent("FrameRate"); } }
        }

        private int width = 800;
        public int Width
        {
            get { return width;}
            set { if (width != value) { width = value; RaisePropertyChangedEvent("Width"); } }
        }

        private int height = 600;
        public int Height
        {
            get { return height; }
            set { 
                if (height != value) { 
                    height = value; 
                    RaisePropertyChangedEvent("Height"); 
                }
                
            }
        }

        private bool lockAspect = false;
        public bool LockAspect
        {
            get { return lockAspect; }
            set { if (lockAspect != value) { lockAspect = value; RaisePropertyChangedEvent("LockAspect"); } }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
