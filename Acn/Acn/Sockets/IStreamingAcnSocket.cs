using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Sockets
{
    [ComVisible(true)]
    [Guid("018FF255-DCCF-420F-A067-C5BD4EA2BEBF")]
    public interface IStreamingAcnSocket
    {
        #region Traffic

        void JoinDmxUniverse(int universe);

        void DropDmxUniverse(int universe);

        void SendDmx(int universe, byte[] dmxData);

        /// <summary>
        /// Sends a DMX frame over streaming ACN.
        /// </summary>
        /// <remarks>
        /// The dmxData must include the start code. Please use the overload with startCode specified
        /// if the data does not include the start code.
        /// </remarks>
        /// <param name="universe">The streaming ACN universe between 1 and 3000.</param>
        /// <param name="dmxData">The DMX data including the start code.</param>
        /// <param name="priority">The sACN priority for the DMX data.</param>
        void SendDmx(int universe, byte[] dmxData, byte priority);

        /// <summary>
        /// Sends a DMX frame over streaming ACN.
        /// </summary>
        /// <param name="universe">The streaming ACN universe between 1 and 3000.</param>
        /// <param name="startCode"></param>
        /// <param name="dmxData">The DMX data including the start code.</param>
        /// <param name="priority">The sACN priority for the DMX data.</param>
        void SendDmx(int universe, byte startCode, byte[] dmxData, byte priority);


        #endregion

    }
}
