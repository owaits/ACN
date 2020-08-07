using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexElementLibraryUpdated:CitpMsexHeader
    {
        public const string PacketType = "ELUp";

        #region Constructors

        public CitpMsexElementLibraryUpdated()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexElementLibraryUpdated(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Content type of updated library.
        /// </summary>
        public MsexElementType LibraryType { get; set; }

        /// <summary>
        /// Library that has been updated.
        /// </summary>
        public byte LibraryNumber
        {
            get { return LibraryId.ToNumber(); }
        }

        private CitpMsexLibraryId libraryId = new CitpMsexLibraryId();

        /// <summary>
        /// Library that has been updated.
        /// </summary>
        public CitpMsexLibraryId LibraryId
        {
            get { return libraryId; }
            set { libraryId = value; }
        }

        private MsexUpdateFlags updateFlags = MsexUpdateFlags.None;

        public MsexUpdateFlags UpdateFlags
        {
            get { return updateFlags; }
            set { updateFlags = value; }
        }

        private BitArray affectedElements = null;

        /// <summary>
        /// Which elements have been affected
        /// </summary>
        /// <remarks>
        /// E.g. the following test will be true if the element or library indexed by ItemIndex has changed:
        /// ItemSet[ItemIndex / 8] & (1 &lt;&lt; (ItemIndex % 8))
        /// </remarks>
        public BitArray AffectedElements
        {
            get { return affectedElements;}
            set { affectedElements = value; }
        }

        private BitArray affectedLibraries = null;

        /// <summary>
        /// Which sub-libraries have been affected
        /// </summary>
        /// <remarks>
        /// E.g. the following test will be true if the element or library indexed by ItemIndex has changed:
        /// ItemSet[ItemIndex / 8] & (1 &lt;&lt; (ItemIndex % 8))
        /// </remarks>
        public BitArray AffectedLibraries
        {
            get { return affectedLibraries; }
            set { affectedLibraries = value; }
        }


        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            LibraryType = (MsexElementType)data.ReadByte();

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                LibraryId.ParseNumber(data.ReadByte());
            else
                LibraryId = data.ReadMsexLibraryId();

            UpdateFlags = (MsexUpdateFlags) data.ReadByte();

            if (MsexVersion >= CitpMsexVersions.Msex12Version)
            {
                AffectedElements = new BitArray(data.ReadBytes(32));
                AffectedLibraries = new BitArray(data.ReadBytes(32));
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)LibraryType);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                data.Write(LibraryNumber);
            else
                data.WriteMsexLibraryId(LibraryId);

            data.Write((byte) UpdateFlags);

            if (MsexVersion >= CitpMsexVersions.Msex12Version)
            {
                byte[] elementsData = new byte[32];
                AffectedElements.CopyTo(elementsData,0);
                data.Write(elementsData);

                byte[] libraryData = new byte[32];
                AffectedLibraries.CopyTo(libraryData, 0);
                data.Write(libraryData);
            }

        }

        #endregion
    }
}
