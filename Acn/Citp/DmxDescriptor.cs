using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp
{
    /// <summary>
    /// Contains information about the patching of a CITP Device
    /// </summary>
    public class DmxDescriptor
    {
        public const int INVALID = -1;

        public string Protocol;
        public int Net;
        public int Universe;
        public int Channel;
        public bool UnicodeString;

        public static bool TryParse(string value, out DmxDescriptor descriptor)
        {
            descriptor = new DmxDescriptor();

            string[] parts = value.Split('/');

            descriptor.Protocol = parts[0];
            descriptor.Net = INVALID;
            descriptor.Universe = INVALID;
            descriptor.Channel = INVALID;
            if (parts.Length > 1)
            {
                int.TryParse(parts[parts.Length - 1], out descriptor.Channel);
                if (descriptor.Channel == INVALID) return false;
                if (parts.Length > 2)
                {
                    int.TryParse(parts[parts.Length - 2], out descriptor.Universe);
                    if (descriptor.Universe == INVALID) return false;
                    if (parts.Length > 3)
                    {
                        int.TryParse(parts[parts.Length - 3], out descriptor.Net);
                        if (descriptor.Net == INVALID || parts.Length > 4) return false;
                    }
                }
            }
            return true;
        }

        public override string ToString()
        {
            string str = Protocol;

            if (Net != INVALID) str += '/' + Net;
            if (Universe != INVALID) str += '/' + Universe;
            if (Channel != INVALID) str += '/' + Channel;

            return str;
        }
    }
}
