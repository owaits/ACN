using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sntp;
using NDesk.Options;

namespace SntpUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverAddress = "ntp2d.mcc.ac.uk";
            bool addressExplicit = false;
            bool setSystemTime = false;
            bool help = false;

            var p = new OptionSet() {
                { "s=|server=", "Server to request time from",  v => { serverAddress = v; addressExplicit = true; } },
                { "u|update", "Update the system clock with the recieved time", v => setSystemTime = v != null},
                { "h|?|help", "Print command line help",  v => help = v != null }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("SntpUpdate: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `SntpUpdate --help' for more information.");
                return;
            }

            if (!addressExplicit && extra.Count > 0)
            {
                serverAddress = extra[0];
            }

            if (help)
                p.WriteOptionDescriptions(Console.Out);
            else
            {
                SntpClient client = new SntpClient(serverAddress);
                NtpData data = client.GetTime(setSystemTime);
                Console.WriteLine(data.ToString());
            }
        }
    }
}
