using CCMSServer.Scripts.Configs.Structs;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CCMSServer.Scripts.Configs
{
    internal class ConfigReader<T> where T : IConfig<T>
    {
        private const string LOG_FORMAT = "[ConfigReader] {0}";

        private T Result;
        private bool isRead = false;

        internal void Read(string configName)
        {
            if (isRead == false)
            {
                if (configName.Contains(".xml") == false)
                {
                    configName += ".xml";
                }

                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
                appDirectory = appDirectory.Replace("bin\\Debug\\", "Configurations\\" + configName);
#else
            appDirectory = appDirectory.Replace("bin\\Release\\", "Configurations\\" + configName);
#endif

                Console.WriteLine(LOG_FORMAT, "read [" + typeof(T).Name + "] from : " + appDirectory);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StreamReader streamReader = new StreamReader(appDirectory);

                Result = (T)xmlSerializer.Deserialize(streamReader);

                streamReader.Close();
                isRead = true;
            }
        }

        internal T GetConfig()
        {   
            if (isRead == false)
            {
                throw new NullReferenceException();
            }

            return Result;
        }
    }
}
