using CCMSServer.Scripts;
using CCMSServer.Scripts.Configs;
using System;

namespace CCMSServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 서버를 실행하기 위한 xml 파일을 읽어옵니다. (CCMSServer/CCMSServer/Configurations 참조)
            ConfigHandler.Instance.ReadAll();

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
                    Console.WriteLine("[Console] Invalid command.");
                }
            }

            Environment.Exit(0);
        }
    }
}
