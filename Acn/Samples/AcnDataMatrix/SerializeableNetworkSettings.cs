using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcnDataMatrix
{
    /// <summary>
    /// Store the network settings.
    /// </summary>
    [Serializable]
    public class SerializeableNetworkSettings
    {
        public SerializeableNetworkSettings(SettingsViewModel netSettings)
        {
            Net1Enabled = netSettings.Net1Enabled;
            Net1AdapterId = netSettings.Net1.InterfaceId;
            Net1UniverseCount = netSettings.Net1UniverseCount;
            Net1StartUniverse = netSettings.Net1StartUniverse;

            Net2Enabled = netSettings.Net2Enabled;
            Net2AdapterId = netSettings.Net2.InterfaceId;
            Net2UniverseCount = netSettings.Net2UniverseCount;
            Net2StartUniverse = netSettings.Net2StartUniverse;

            Width = netSettings.Width;
            Height = netSettings.Height;
            LockAspect = netSettings.LockAspect;

        }

        public bool Net1Enabled{get; set;}

        public string Net1AdapterId { get; set; }

        public int Net1UniverseCount { get; set; }

        public int Net1StartUniverse { get; set; }


        public bool Net2Enabled { get; set; }

        public string Net2AdapterId { get; set; }

        public int Net2UniverseCount { get; set; }

        public int Net2StartUniverse { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool LockAspect { get; set; }
    }
}
