using CCMSServer.Scripts;
using CCMSServer.Scripts.Configs;
using CCMSServer.Scripts.Databases;
using System;

namespace CCMSServer
{
    internal class Program
    {
        private const string LOG_FORMAT = "[CCMSServer] ";

        static void Main(string[] args)
        {
            // 서버를 실행하기 위한 xml 파일을 읽어옵니다. (CCMSServer/CCMSServer/Configurations 참조)
            ConfigHandler.Instance.ReadAll();

            // DB 연결합니다.
            DBHandler.Instance.Connect();

            // 코어 서버를 실행합니다.
            TCP_CCMSServer.Instance.StartServer();

            // 콘솔로부터 입출력을 받습니다.
            string input = "";
            while ((input = Console.ReadLine()).ToLower().Equals("stop") == false)
            {
                if (input.ToLower().StartsWith("say ") == true)
                {
                    string message = input.Replace("say ", "");
                }
                else
                {
                    WriteLine(LOG_FORMAT, "Invalid command", ConsoleColor.Red);
                }
            }

            Environment.Exit(0);
        }

        public static void WriteLine(string LogFormat, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(LogFormat);

            Console.ResetColor();
            Console.WriteLine(message);
        }
    }
}
