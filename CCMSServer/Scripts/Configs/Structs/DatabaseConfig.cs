using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMSServer.Scripts.Configs.Structs
{
    public struct DatabaseConfig : IConfig<DatabaseConfig>
    {
        public string IPAddress;
        public string UserID;
        public string UserPassword;

        public string DatabaseName;
    }
}
