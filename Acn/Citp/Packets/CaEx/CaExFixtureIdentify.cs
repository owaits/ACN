using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp;
using Citp.IO;

namespace Citp.Packets.CaEx
{
    /// <summary>
    /// Use this message to set the fixture identifier from the capture Id.
    /// </summary>
    /// <seealso cref="Citp.Packets.CaEx.CaExHeader" />
    public class CaExFixtureIdentify:CaExHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="CaExFixtureIdentify"/> class.
        /// </summary>
        public CaExFixtureIdentify()
            : base(CaExContentCodes.FixtureIdentify)
        {    
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Capture ID to fixture identifier pair.
        /// </summary>
        public struct CaptureToFixtureLink
        {
            /// <summary>
            /// The capture unique ID.
            /// </summary>
            public Guid CaptureInstanceId;

            /// <summary>
            /// The CITP fixture identifier
            /// </summary>
            public int FixtureId;
        }

        private List<CaptureToFixtureLink> fixtureLinks = new List<CaptureToFixtureLink>();

        /// <summary>
        /// Gets a list of links between Capture Id's and fixture identifiers.
        /// </summary>
        /// <remarks>
        /// In order to tell capture the fixture identifier for a fixture you must fill this in so capture
        /// can assign Id's to the fixtures.
        /// </remarks>
        public List<CaptureToFixtureLink> FixtureLinks
        {
            get { return fixtureLinks; }
        }


        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the packet information from the specified stream.
        /// </summary>
        /// <param name="data">The stream to read the packet information from.</param>
        /// <remarks>
        /// Use to create a packet from a network stream.
        /// </remarks>
        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            FixtureLinks.Clear();

            int count = data.ReadUInt16();
            for (int n = 0; n < count;n++)
            {
                CaptureToFixtureLink link = new CaptureToFixtureLink()
                {
                    CaptureInstanceId = new Guid(data.ReadBytes(15)).FromNetwork(),
                    FixtureId = (int) data.ReadUInt32()
                };

                FixtureLinks.Add(link);
            }
        }

        /// <summary>
        /// Writes the information in this packet to the specified stream.
        /// </summary>
        /// <param name="data">The stream to write the packet information to.</param>
        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((ushort) FixtureLinks.Count);
            foreach(var link in FixtureLinks)
            {
                data.WriteGuid(link.CaptureInstanceId);
                data.Write((uint) link.FixtureId);
            }
        }

        #endregion
    }
}
