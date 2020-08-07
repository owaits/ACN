using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp
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

        public Guid PersonalityID { get; set; }

        public static bool TryParse(string value, out DmxDescriptor descriptor)
        {
            descriptor = new DmxDescriptor();

            string[] parts = value.Split('/');

            try
            {
                int position = 0;
                while(position < parts.Length)
                {
                    switch(parts[position].ToLower())
                    {
                        case "artnet":
                            descriptor.Protocol = parts[position];
                            descriptor.Net = int.Parse(parts[position + 1]);
                            descriptor.Universe = int.Parse(parts[position + 2]);
                            descriptor.Channel = int.Parse(parts[position + 3]);
                            break;
                        case "bsre1.31":
                            descriptor.Protocol = parts[position];
                            descriptor.Universe = int.Parse(parts[position + 1]);
                            descriptor.Channel = int.Parse(parts[position + 2]);
                            break;
                        case "personalityid":
                            descriptor.PersonalityID = Guid.Parse(parts[position + 1]);
                            break;
                    }
                    position++;
                }
            }
            catch (Exception)
            {
                return false;
            }            


            return true;
        }

        public override string ToString()
        {
            string str = Protocol;

            if (Net != INVALID && Protocol.ToLower() != "bsre1.31") str += '/' + Net.ToString();
            if (Universe != INVALID) str += '/' + Universe.ToString();
            if (Channel != INVALID) str += '/' + Channel.ToString();

            return str;
        }

        /// <summary>
        /// Generates the MSEX version 1.2 DMX string with the personality ID.
        /// </summary>
        /// <returns></returns>
        public string ToVersion1_2String()
        {
            string dmxSource = ToString();

            if (PersonalityID != null && PersonalityID != Guid.Empty)
            {
                dmxSource += "/PersonalityID/{" + PersonalityID.ToString() + "}";
            }

            return dmxSource;
        }
    }
}
