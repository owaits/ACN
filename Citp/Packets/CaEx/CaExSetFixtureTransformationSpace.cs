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
    /// The different types or position/orientation space to use with reference to fixture position and orientation
    /// </summary>
    public enum TransformSpaces:byte
    {
        /// <summary>
        /// Fixtures are hung with a direction along (0, -1, 0). Conventional fixtures have their up along (0, 0, -1) and moving heads have their display in (0, 0, 1).
        /// </summary>
        Native = 1,
        /// <summary>
        /// As Native, but moving heads have their neutral pan "home" along (0, 0, -1).
        /// </summary>
        PanHome = 2    
    }

    /// <summary>
    /// Sent to visualisation software to determine the position and orientation space they should use when sending coordinates and orientation. 
    /// After recieving this request further messages should use the requested transformation.
    /// </summary>
    /// <remarks>
    /// The Capture visualiser uses the base position as the origin for all positions and orientations. However often the beam origin can be a more useful origin and so this is where PanHome can be useful.
    /// </remarks>
    /// <seealso cref="Citp.Packets.CaEx.CaExHeader" />
    public class CaExSetFixtureTransformationSpace:CaExHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="CaExSetFixtureTransformationSpace"/> class.
        /// </summary>
        public CaExSetFixtureTransformationSpace()
            : base(CaExContentCodes.SetFixtureTransformationSpace)
        {    
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Gets or sets the transformation space to use when sending future messages. Please see the <see cref="TransformSpaces"/> for a description of the different transform spaces.
        /// </summary>
        /// <remarks>
        /// The value specified here will change the value reported in all future positions a orientations.
        /// </remarks>
        public TransformSpaces TransformationSpace { get; set; }

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

            TransformationSpace = (TransformSpaces) data.ReadByte();
        }

        /// <summary>
        /// Writes the information in this packet to the specified stream.
        /// </summary>
        /// <param name="data">The stream to write the packet information to.</param>
        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) TransformationSpace);
        }

        #endregion
    }
}
