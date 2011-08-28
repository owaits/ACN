using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace StreamingAcn
{
    public class CardInfo
    {
        public CardInfo(NetworkInterface info, int addressIndex)
        {
            Interface = info;
            AddressIndex = addressIndex;
        }

        public NetworkInterface Interface { get; set; }

        public int AddressIndex { get; set; }

        public IPAddress IpAddress
        {
            get
            {
                return Interface.GetIPProperties().UnicastAddresses[AddressIndex].Address;
            }
        }

        public override string ToString()
        {
            return string.Format("{1}:  {0}",Interface.Description,IpAddress.ToString());
        }
    }
}
