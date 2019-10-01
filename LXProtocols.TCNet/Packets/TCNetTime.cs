using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet.Packets
{
    public class TCNetTime : TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetOptIn"/> class.
        /// </summary>
        public TCNetTime() : base(MessageTypes.Time)
        {
            Layer1 = new Layer();
            Layer2 = new Layer();
            Layer3 = new Layer();
            Layer4 = new Layer();

            LayerA = new Layer();
            LayerB = new Layer();
            LayerM = new Layer();
            LayerC = new Layer();
        }

        #endregion

        #region Layer Status

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Layer
        {
            public TimeSpan CurrentTime { get; set; }
            public TimeSpan TotalTime { get; set; }
            public byte BeatMarker { get; set; }
            public DeckState State { get; set; }
            public bool OnAir { get; set; }

            public override string ToString()
            {
                return State.ToString();
            }
        }


        #endregion

        #region Packet Content

        public Layer Layer1 { get; private set; }

        public Layer Layer2 { get; private set; }

        public Layer Layer3 { get; private set; }

        public Layer Layer4 { get; private set; }

        public Layer LayerA { get; private set; }

        public Layer LayerB { get; private set; }

        public Layer LayerM { get; private set; }

        public Layer LayerC { get; private set; }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the data from a memory buffer into this packet object.
        /// </summary>
        /// <param name="data">The data to be read.</param>
        /// <remarks>
        /// Decodes the raw data into usable information.
        /// </remarks>
        public override void ReadData(TCNetBinaryReader data)
        {
            base.ReadData(data);

            Layer[] layers = new Layer[] { Layer1, Layer2, Layer3, Layer4, LayerA, LayerB, LayerM, LayerC };

            //Current Time
            foreach(var layer in layers)
                layer.CurrentTime = data.ReadNetworkTime();

            //Total Time
            foreach (var layer in layers)
                layer.TotalTime = data.ReadNetworkTime();

            //Beat Marker
            foreach (var layer in layers)
                layer.BeatMarker = data.ReadByte();

            //Layer State
            foreach (var layer in layers)
                layer.State = (DeckState) data.ReadByte();

            //On Air
            foreach (var layer in layers)
                layer.OnAir = data.ReadBoolean();

            data.ReadBytes(42);
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            Layer[] layers = new Layer[] { Layer1, Layer2, Layer3, Layer4, LayerA, LayerB, LayerM, LayerC };

            //Current Time
            foreach (var layer in layers)
                data.WriteToNetwork(layer.CurrentTime);

            //Total Time
            foreach (var layer in layers)
                data.WriteToNetwork(layer.TotalTime);

            //Beat Marker
            foreach (var layer in layers)
                data.Write(layer.BeatMarker);

            //Layer State
            foreach (var layer in layers)
                data.Write((byte) layer.State);

            //On Air
            foreach (var layer in layers)
                data.Write(layer.OnAir);

            data.Write(new byte[42]);
        }

        #endregion
    }
}
