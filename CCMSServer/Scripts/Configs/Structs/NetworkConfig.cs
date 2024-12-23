using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMSServer.Scripts.Configs.Structs
{
    public struct NetworkConfig : IConfig<NetworkConfig>
    {
        public string ServerIPAddress;
        public int ServerPortNumber;
    }
}
