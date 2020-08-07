using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExEnterShow:CaExHeader
    {
        #region Setup and Initialisation

        public CaExEnterShow()
            : base(CaExContentCodes.EnterShow)
        {    
        }

        #endregion

        #region Packet Content

        public string Name { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Name = data.ReadUcs2();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteUcs2(Name);
        }

        #endregion
    }
}
