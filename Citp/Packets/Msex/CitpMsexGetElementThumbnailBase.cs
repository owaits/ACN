using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    /// <summary>
    /// A base class for CitpMsexGetElementLibraryThumbnail and CitpMsexGetElementThumbnail 
    /// The messages share much of the same data.
    /// </summary>
    public abstract class CitpMsexGetElementThumbnailBase:CitpMsexHeader
    {
        #region Constructors

        public CitpMsexGetElementThumbnailBase()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGetElementThumbnailBase(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion
        #region Packet Content

        public string ThumbnailFormat { get; set; }

        public UInt16 ThumbnailWidth { get; set; }

        public UInt16 ThumbnailHeight { get; set; }

        public ThumbnailOptions ThumbnailFlags { get; set; }

        public MsexElementType LibraryType { get; set; }

        
        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);                
        }

        #endregion
    }
}
