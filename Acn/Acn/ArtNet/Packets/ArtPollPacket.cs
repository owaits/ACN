using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    internal class ArtPollPacket:ArtNetPacket
    {
        public ArtPollPacket(ArtNetRecieveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        private byte talkToMe=0;

        public byte TalkToMe
        {
            get { return talkToMe; }
            protected set { talkToMe = value; }
        }

        #endregion

        public override void ReadData(System.IO.BinaryReader data)
        {
            base.ReadData(data);

            TalkToMe = data.ReadByte();
        }

    }
}
