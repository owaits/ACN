using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// Gets the TCNet data types for the data sub-packet.
    /// </summary>
    [Flags]
    public enum DataTypes:byte
    {
        /// <summary>
        /// The unknown data type.
        /// </summary>
        None = 0,
        /// <summary>
        /// The metrics about the current track and play state for each deck.
        /// </summary>
        Metrics = 2,
        /// <summary>
        /// The meta data about the current playing track.
        /// </summary>
        MetaData = 4,
        /// <summary>
        /// The beat grid information.
        /// </summary>
        BeatGrid = 8,
        /// <summary>
        /// The current cue information.
        /// </summary>
        Cue = 12,
        /// <summary>
        /// The small waveform data.
        /// </summary>
        SmallWaveform = 16,
        /// <summary>
        /// The big waveform data including waveform and frequency colour.
        /// </summary>
        BigWaveform = 32,
        /// <summary>
        /// The low resource artwork file representing the current track.
        /// </summary>
        LowResArtworkFile = 128,
        /// <summary>
        /// The authentication handshake data.
        /// </summary>
        Authentication = 255
    }
}
