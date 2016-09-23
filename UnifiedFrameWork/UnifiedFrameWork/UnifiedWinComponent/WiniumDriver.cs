using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winium.Desktop.Driver
{
    public class WiniumDriver
    {
        public static Thread winiumDriverThread;
        public static void WiniumInitiate()
        {
            winiumDriverThread = new Thread(UseWiniumDriver);
            winiumDriverThread.Start();
        }

        public static void WiniumFinalise()
        {
            if (winiumDriverThread.IsAlive)
            {
                winiumDriverThread.Abort();
            }
        }
        public static void UseWiniumDriver()
        {
            var listeningPort = 9999;
            var options = new CommandLineOptions();
            try
            {
                var listener = new Listener(listeningPort);
                Listener.UrnPrefix = options.UrlBase;

                // Console.WriteLine("Starting Windows Desktop Driver on port {0}\n", listeningPort);

                listener.StartListening();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Failed to start driver: {0}", ex);
                throw;
            }
        }
    }
    
}
