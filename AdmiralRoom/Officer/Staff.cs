using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    class Staff
    {
        public static Staff Current { get; } = new Staff();
        public void Start(int port = 62534)
        {
            FiddlerApplication.Startup(port, FiddlerCoreStartupFlags.ChainToUpstreamGateway);

        }
        public void Stop()
        {
            FiddlerApplication.Shutdown();
        }
    }
}
