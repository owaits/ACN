using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm;

namespace Acn.Sockets
{
    public class NetworkUId:UId
    {
        protected NetworkUId():base()
        {
        }

        public NetworkUId(ushort manufacturerId, uint deviceId, int universe):base(manufacturerId,deviceId)
        {
            Universe = universe;
        }

        public NetworkUId(UId id, int universe)
            : base(id)
        {
            Universe = universe;
        }

        private int universe = 0;

        public int Universe
        {
            get { return universe; }
            set { universe = value; }
        }
    }
}
