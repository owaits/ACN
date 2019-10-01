using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Contains metadata of a layer
    /// </summary>
    /// <remarks>
    /// Unicast on update event or upon request.
    /// 
    /// This info can be requested from a node by sending a TCNet Request Data packet, with Datatype=4, Parameter 1=LAYER, Parameter 2=0
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetDataHeader" />
    public class TCNetMetaData:TCNetDataHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetOptIn"/> class.
        /// </summary>
        public TCNetMetaData() : base(DataTypes.MetaData)
        {
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Track ID number of the track that is loaded on layer. This is usually the source’s database ID number.
        /// </summary>
        public ushort TrackID { get; set; }

        /// <summary>
        /// Artist name of content loaded to layer -
        /// </summary>
        /// <remarks>
        /// Example: My Artist Name (Max 256 characters)
        /// </remarks>
        public string TrackArtist { get; set; }

        /// <summary>
        /// Track name of content loaded to layer.
        /// </summary>
        /// <remark>
        /// Example: My Track Title (Max 256 characters)
        /// </remark>
        public string TrackTitle { get; set; }

        /// <summary>
        /// Audio Key of track
        /// </summary>
        public ushort TrackKey { get; set; }

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
            TrackID = data.ReadNetwork16();
            TrackArtist = data.ReadNetworkString(256);
            TrackTitle = data.ReadNetworkString(256);
            //TrackKey = data.ReadNetwork16();
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)0);
            data.Write(TrackID);
            data.WriteToNetwork(TrackArtist,256);
            data.WriteToNetwork(TrackTitle, 256);
            data.WriteToNetwork(TrackKey);
        }

        #endregion
    }
}
