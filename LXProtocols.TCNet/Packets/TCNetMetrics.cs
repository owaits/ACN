using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// This packet is sent continuously by devices publishing timecode data to the network.
    /// </summary>
    /// <seealso cref="ProDJTap.Packets.DJTapHeader" />
    public class TCNetMetrics : TCNetDataHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetTime"/> class.
        /// </summary>
        public TCNetMetrics() : base(DataTypes.Metrics)
        {
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Gets or sets the manufacturer of the transmitting decide.
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the model of the transmitting device.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the play state of the deck.
        /// </summary>
        public DeckState LayerState { get; set; }

        /// <summary>
        /// This is the sync master status of the deck that sends info to ProDJ Tap. ProDJ tap is using this 
        /// status to follow the current active deck and allows auto cue to this deck.
        /// </summary>
        public ushort SyncMaster { get; set; }

        /// <summary>
        /// Gets or sets a value that represents the edge of each beat. At every beat this value is incremented between
        /// 1 and 4. 
        /// </summary>
        public ushort BeatMarker { get; set; }

        /// <summary>
        /// Gets or sets the length of the track.
        /// </summary>
        public uint TrackLength { get; set; }

        /// <summary>
        /// Gets or sets the current position within the track.
        /// </summary>
        public uint CurrentPosition { get; set; }

        /// <summary>
        /// This is play head speed on deck that sends info to ProDJ Tap.
        /// - Example: -0~65536 (Where 32768 = 100% speed, 0 = 0% Speed, 65536=200% speed)
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Current Beat Number
        /// </summary>
        public uint BeatNumber { get; set; }

        /// <summary>
        /// Gets or sets the current BPM of the track being played.
        /// </summary>
        public double BPM { get; set; }

        /// <summary>
        /// This is play head speed on deck that sends info to ProDJ Tap. (Used for live adjust.
        /// - Example: 0~65536 (Where 32768 = 100% speed, 0 = 0% Speed, 65536=200% speed)
        /// </summary>
        public double SpeedPitchBend { get; set; }

        /// <summary>
        /// This is track ID number of the track that is loaded on deck that sends info to ProDJ Tap.
        /// This is usually the database ID number.
        /// </summary>
        public uint TrackId { get; set; }

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

            data.ReadByte();
            LayerState = (DeckState)data.ReadByte();
            data.ReadByte();
            SyncMaster = data.ReadByte();
            data.ReadByte();
            BeatMarker = data.ReadByte();
            TrackLength = data.ReadNetwork32();
            CurrentPosition = data.ReadNetwork32();
            Speed = Math.Round((double)data.ReadNetwork32() / 32767.0, 3);
            data.ReadBytes(13);
            BeatNumber = data.ReadNetwork32();
            data.ReadBytes(51);
            BPM = (double)data.ReadNetwork32() / 100.0;
            SpeedPitchBend = Math.Round((double)data.ReadNetwork16() / 32767.0, 3);
            TrackId = data.ReadNetwork32();
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)0);
            data.Write((ushort)LayerState);
            data.Write(SyncMaster);
            data.Write(TrackLength);
            data.Write(CurrentPosition);
            data.Write((uint)(Speed * 32767));
            data.Write(new byte[1]);
            data.Write(BeatNumber);
            data.Write(new byte[51]);
            data.Write((uint)(BPM * 100.0));
            data.Write((ushort)(SpeedPitchBend * 32767));
            data.Write(TrackId);
        }

        #endregion

    }
}
