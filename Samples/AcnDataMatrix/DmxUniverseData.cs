using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Acn.Helpers;

namespace AcnDataMatrix
{
    public class DmxUniverseData
    {
        public DmxUniverseData()
        {
            Universe = new DmxUniverse(1);
        }

        private DmxUniverse universe = null;

        public DmxUniverse Universe
        {
            get { return universe; }
            set
            {
                if (universe != value)
                {

                    if (universe != null)
                    {
                        universe.DmxDataChanged -= universe_DmxDataChanged;
                    }

                    universe = value;

                    if (universe != null)
                    {
                        universe.DmxDataChanged += universe_DmxDataChanged;
                    }
                }
            }
        }

        void universe_DmxDataChanged(object sender, EventArgs e)
        {
            if (DmxDataChanged != null)
                DmxDataChanged(this, EventArgs.Empty);
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
            }

        }
    }
}
