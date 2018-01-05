using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlitchDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            bool show_help = false;
            int universe = 1, channel = 1, threshold = 150;
            string ipAddress = "10.0.0.101";
            var options = new OptionSet() {
                    { "u|universe:", "the universe to check" , (int t) => universe = t },
                    { "c|channel:", "the channel to listen to" , (int t) => channel = t },
                    { "t|threshold:", "The threshold in milliseconds above which to report an error" , (int t) => threshold = t },
                    { "i|ipAddress:", "The ip address to listen on" , (string t) => ipAddress = t },
                    { "h|help",  "show this message", v => show_help = v != null },
                };

            try
            {
                options.Parse(Environment.GetCommandLineArgs());
            }
            catch (OptionException oex)
            {
                Console.Write("glitchDetector: ");
                Console.WriteLine(oex.Message);
                Console.WriteLine("Try `glitchDetector --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(options);
                return;
            }

            Detector detector = new Detector() { Universe = universe, Channel = channel, Threshold = threshold, IpAddress = ipAddress };
            detector.Start();
        }

        static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: GlitchDetctor [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}
