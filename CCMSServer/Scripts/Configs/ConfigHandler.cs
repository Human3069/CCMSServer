using CCMSServer.Scripts.Configs.Structs;
using System;

namespace CCMSServer.Scripts.Configs
{
    internal class ConfigHandler
    {
        protected static ConfigHandler _instance;
        public static ConfigHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigHandler();
                }
                return _instance;
            }
        }

        public ConfigHandler()
        {
            NetworkConfigReader = new ConfigReader<NetworkConfig>();
            DatabaseConfigReader = new ConfigReader<DatabaseConfig>();
        }

        private ConfigReader<NetworkConfig> NetworkConfigReader;
        private ConfigReader<DatabaseConfig> DatabaseConfigReader;

        public void ReadAll()
        {
            NetworkConfigReader.Read("NetworkConfig");
            DatabaseConfigReader.Read("DatabaseConfig");
        }

        public NetworkConfig GetNetworkConfig()
        {
            return NetworkConfigReader.GetConfig();
        }

        public DatabaseConfig GetDatabaseConfig()
        {
            return DatabaseConfigReader.GetConfig();
        }
    }
}
