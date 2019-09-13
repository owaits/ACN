using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// The track play state of the deck.
    /// </summary>
    public enum DeckState
    {
        Idle = 0, 
        Playing = 2,
        Looping = 4,
        Paused = 5,
        Stopped = 6,
        CueButtonDown = 7, 
        PlatterDown = 8,
        FastForward = 9,
        FastRewind = 10,
        Hold = 11
    }

    /// <summary>
    /// This packet is sent continuously by devices publishing timecode data to the network.
    /// </summary>
    /// <seealso cref="ProDJTap.Packets.DJTapHeader" />
    public class TCNetTime: TCNetHeader
    {        
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetTime"/> class.
        /// </summary>
        public TCNetTime():base(MessageTypes.Timecode)
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
        /// Gets or sets the deck or master that this timecode refers to.
        /// </summary>
        /// <remarks>
        /// 1 to 4 = Decks 1-4
        /// 6,7 = Layer A, Layer B
        /// 8 = Master
        /// </remarks>
        public ushort DeckId { get; set; }

        /// <summary>
        /// Gets or sets the play state of the deck.
        /// </summary>
        public DeckState DeckState { get; set; }

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
        public uint LocationMarker { get; set; }

        /// <summary>
        /// This is play head speed on deck that sends info to ProDJ Tap.
        /// - Example: -0~65536 (Where 32768 = 100% speed, 0 = 0% Speed, 65536=200% speed)
        /// </summary>
        public double SpeedValue { get; set; }

        /// <summary>
        /// Gets or sets the frame rate of the timecode.
        /// </summary>
        public ushort TCType { get; set; }

        /// <summary>
        /// This is the type of timecode send by ProDJ Tap
        /// - Values: 0=Stopped, 1=Running, 2=Force Resync
        /// </summary>
        public ushort TCState { get; set; }

        /// <summary>
        /// Gets or sets the timecode hours.
        /// </summary>
        public ushort TCHours { get; set; }

        /// <summary>
        /// Gets or sets the timecode minutes.
        /// </summary>
        public ushort TCMinutes { get; set; }

        /// <summary>
        /// Gets or sets the timecode seconds.
        /// </summary>
        public ushort TCSeconds { get; set; }

        /// <summary>
        /// Gets or sets the timecode frmaes.
        /// </summary>
        public ushort TCFrames { get; set; }

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
        public ushort TrackId { get; set; }

        /// <summary>
        /// Gets or sets the artist for the current track.
        /// </summary>
        public string TrackArtist { get; set; }

        /// <summary>
        /// Gets or sets the title of the current track.
        /// </summary>
        public string TrackTitle { get; set; }

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

            Brand = ASCIIEncoding.ASCII.GetString(data.ReadBytes(8));
            Model = ASCIIEncoding.ASCII.GetString(data.ReadBytes(8));
            DeckId = data.ReadNetwork16();
            DeckState = (DeckState) data.ReadNetwork16();
            SyncMaster = data.ReadNetwork16();
            BeatMarker = data.ReadNetwork16();
            TrackLength = data.ReadUInt32();
            LocationMarker = data.ReadUInt32();
            SpeedValue = Math.Round((double) data.ReadUInt16() / 32767.0,3);
            data.ReadBytes(2);
            TCType = data.ReadNetwork16();
            TCState = data.ReadNetwork16();
            TCHours = data.ReadNetwork16();
            TCMinutes = data.ReadNetwork16();
            TCSeconds = data.ReadNetwork16();
            TCFrames = data.ReadNetwork16();
            data.ReadBytes(56);
            BPM = (double) data.ReadUInt32() / 100.0;
            SpeedPitchBend = Math.Round((double)data.ReadUInt16() / 32767.0, 3);
            TrackId = data.ReadNetwork16();
            TrackArtist = ASCIIEncoding.ASCII.GetString(data.ReadBytes(256)).TrimEnd('\0');
            TrackTitle = ASCIIEncoding.ASCII.GetString(data.ReadBytes(256)).TrimEnd('\0');
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(ASCIIEncoding.ASCII.GetBytes(Brand.PadRight(8)));
            data.Write(ASCIIEncoding.ASCII.GetBytes(Model.PadRight(8)));
            data.Write(DeckId);
            data.Write((ushort) DeckState);
            data.Write(SyncMaster);
            data.Write(TrackLength);
            data.Write(LocationMarker);
            data.Write((ushort) (SpeedValue * 32767));
            data.Write(new byte[2]);
            data.Write(TCType);
            data.Write(TCState);
            data.Write(TCHours);
            data.Write(TCMinutes);
            data.Write(TCSeconds);
            data.Write(TCFrames);
            data.Write(new byte[56]);
            data.Write((uint) (BPM * 100.0));
            data.Write((ushort)(SpeedValue * 32767));
            data.Write(TrackId);
            data.Write(ASCIIEncoding.ASCII.GetBytes(TrackArtist.PadRight(64)));
            data.Write(ASCIIEncoding.ASCII.GetBytes(TrackTitle.PadRight(64)));
        }

        #endregion

    }
}
