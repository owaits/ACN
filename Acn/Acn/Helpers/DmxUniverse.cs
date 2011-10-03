using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Helpers
{
    public class DmxUniverse
    {
        public event EventHandler DmxDataChanged;

        public DmxUniverse(int universe)
        {
            Universe = universe;
        }

        public int Universe { get; protected set; }

        private byte[] dmxData = new byte[513];

        public byte[] DmxData
        {
            get { return dmxData; }
            protected set { dmxData = value; }
        }

        private int aliveTime = 0;

        public int AliveTime
        {
            get { return aliveTime; }
            set { aliveTime = value; }
        }

        private int keepAliveTime = 0;

        public int KeepAliveTime
        {
            get { return keepAliveTime; }
            set { keepAliveTime = value; }
        }

        public void SetDmx(byte[] value)
        {
            DmxData = dmxData;
            aliveTime = 3;

            RaiseDmxDataChanged();
        }

        public void SetDmx(int address, byte value)
        {
            this.dmxData[address] = value;
            aliveTime = 3;

            RaiseDmxDataChanged();
        }

        protected void RaiseDmxDataChanged()
        {
            if (DmxDataChanged != null)
                DmxDataChanged(this, EventArgs.Empty);

        }

    }
}
