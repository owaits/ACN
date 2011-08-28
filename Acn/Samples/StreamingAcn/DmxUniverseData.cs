using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Helpers;

namespace StreamingAcn
{
    public class DmxUniverseData
    {
        private DmxUniverse universe = new DmxUniverse(1);

        public DmxUniverse Universe
        {
            get { return universe; }
            set { universe = value; }
        }

        public event EventHandler DmxDataChanged;

        public byte[] DmxData
        {
            get { return Universe.DmxData; }
            set
            {
                Universe.SetDmx(value);
            }
        }

        public void SetLevel(int channel, byte value)
        {
            if (universe.DmxData[channel] != value)
            {
                universe.SetDmx(channel, value);
                if (DmxDataChanged != null)
                    DmxDataChanged(this, EventArgs.Empty);
            }

        }
    }
}
