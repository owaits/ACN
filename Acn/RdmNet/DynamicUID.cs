using LXProtocols.Acn.Rdm;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.RdmNet
{
    public struct DynamicUId
    {
        public UId DynamicId { get; set; }

        public UId ResponderId { get; set; }
    }
}
