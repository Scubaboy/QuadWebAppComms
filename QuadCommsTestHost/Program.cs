using Nito.AsyncEx;
using QuadComms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadCommsTestHost
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        static async Task<int> MainAsync(string[] args)
        {
            var quadcomss = new CommsController(QuadComms.CommControllers.SupportedChannels.Comm);

            await quadcomss.CommsControllerAsync();

            return 0;
        }
    }
}
