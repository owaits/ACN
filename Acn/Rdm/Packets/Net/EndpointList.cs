using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    /// <summary>
    /// This parameter is used to retrieve a packed list of all endpoints that exist on an E1.33 device, with the exception of the Management Endpoint.
    /// </summary>
    /// <remarks>
    /// The list of Endpoint IDs shall not include the Management Endpoint ID. If the device does not have any Endpoints (other than the Management Endpoint) then it shall return a PDL of 0.
    /// </remarks>
    public class EndpointList
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.EndpointList)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                //Parameter Data Empty
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                //Parameter Data Empty
            }
        }

        public class Reply:RdmResponsePacket
        {
            public Reply():base(RdmCommands.GetResponse,RdmParameters.EndpointList)
            {             
            }

            private int listChangeNumber = 0;

            public int ListChangeNumber
            {
                get { return listChangeNumber; }
                set { listChangeNumber = value; }
            }

            public List<short> PhysicalEndpointIDs { get; private set; } = new List<short>();

            public List<short> VirtualEndpointIDs { get; protected set; } = new List<short>();

            protected override void ReadData(RdmBinaryReader data)
            {
                ListChangeNumber = data.ReadNetwork32();

                List<short> physicalEndpoints = new List<short>();
                List<short> virtualEndpoints = new List<short>();
                for (int n = 0; n < ((Header.ParameterDataLength-4)/3); n++)
                {
                    short endpointId = data.ReadNetwork16();
                    bool physicalEndpoint = (data.ReadByte() != 0);

                    if (physicalEndpoint)
                        physicalEndpoints.Add(endpointId);
                    else
                        virtualEndpoints.Add(endpointId);
                }

                VirtualEndpointIDs = virtualEndpoints;
                PhysicalEndpointIDs = physicalEndpoints;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(ListChangeNumber);
                foreach (short endpointId in VirtualEndpointIDs)
                {
                    data.WriteNetwork(endpointId);
                    data.Write((byte)0);
                }

                foreach (short endpointId in PhysicalEndpointIDs)
                {
                    data.WriteNetwork(endpointId);
                    data.Write((byte)1);
                }
            }
        }
    }
}
