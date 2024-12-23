using CCMSServer.Scripts.Configs;
using CCMSServer.Scripts.Configs.Structs;
using CCMSServer.Scripts.Databases.Structs;
using CCMSServer.Scripts.Databases;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CCMSServer.Scripts
{
    internal class TCP_CCMSServer
    {
        private const string LOG_FORMAT = "[TCP_CCMSServer] {0}";

        private static TCP_CCMSServer _instance;
        public static TCP_CCMSServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TCP_CCMSServer();
                }
                return _instance;
            }
        }

        private TCPServerRunner serverRunner;

        // 서버는 실행하는 PC의 IP 주소를 사용하되, 포트번호는 NetworkConfig에서 지정된 것을 사용.
        public void StartServer()
        {
            NetworkConfig config = ConfigHandler.Instance.GetNetworkConfig();
            serverRunner = new TCPServerRunner(config.ServerPortNumber, OnReceivedFromServer);

            Console.WriteLine(LOG_FORMAT, "Server started on " + config.ServerIPAddress + " : " + config.ServerPortNumber);
        }

        private async void OnReceivedFromServer(TcpClient client, string message)
        {
            Console.WriteLine(LOG_FORMAT, "OnReceivedFromClient(), message : <color=white>" + message + "</color>");

            string pattern = @"_ProtocolType"":\s*(\d+)"; // _ProtocolType":100, _ProtocolType":200, _ProtocolType":201, ...
            Match match = Regex.Match(message, pattern);
            int protocolIndex = int.Parse(match.Groups[1].Value);

            ProtocolType type = (ProtocolType)protocolIndex;
            if (type == ProtocolType.CreateAccount_100)
            {
                RequestCreateAccount requestedData = JsonConvert.DeserializeObject<RequestCreateAccount>(message);
                Console.WriteLine(LOG_FORMAT, requestedData.ToString());

                List<AccountDatabase> accountList = await DBHandler.Instance.GetAccountDataList();
                List<AccountDatabase> foundAccountList = accountList.FindAll(PredicateFunc);

                bool PredicateFunc(AccountDatabase account)
                {
                    return account.user_id.Equals(requestedData.UserID) == true ||
                           account.user_name.Equals(requestedData.UserName) == true;
                }

                ResponseCreateAccount dataToSend;
                if (foundAccountList.Count == 0)
                {
                    bool isCreated = await DBHandler.Instance.TryInsertAccountData(requestedData.UserName,
                                                                                   requestedData.UserID,
                                                                                   requestedData.Password,
                                                                                   requestedData.CreateDateTime);

                    if (isCreated == true)
                    {
                        dataToSend = new ResponseCreateAccount(0, "create user successfully");
                    }
                    else
                    {
                        dataToSend = new ResponseCreateAccount(-1, "invalid data");
                    }
                }
                else
                {
                    dataToSend = new ResponseCreateAccount(-1, "user id already exist");
                }

                string json = dataToSend.AsJson();
                serverRunner.SendMessageTo(client, json);
            }
            else if (type == ProtocolType.IsCreatableAccount_101)
            {
                RequestIsCreatableAccount requestedData = JsonConvert.DeserializeObject<RequestIsCreatableAccount>(message);
                Console.WriteLine(LOG_FORMAT, requestedData.ToString());
            }

            else if (type == ProtocolType.Login_110)
            {
                RequestLogin requestedData = JsonConvert.DeserializeObject<RequestLogin>(message);
                Console.WriteLine(LOG_FORMAT, requestedData.ToString());

                List<AccountDatabase> accountList = await DBHandler.Instance.GetAccountDataList();
                List<AccountDatabase> foundAccountList = accountList.FindAll(PredicateFunc);

                bool PredicateFunc(AccountDatabase account)
                {
                    return account.user_id.Equals(requestedData.UserID) == true &&
                           account.encrypted_password.Equals(requestedData.Password) == true;
                }

                ResponseLogin dataToSend;
                bool isSuccess = foundAccountList.Count != 0;
                if (isSuccess == true)
                {
                    string userName = foundAccountList[0].user_name;
                    dataToSend = new ResponseLogin(userName, 0, "login successfully");
                }
                else
                {
                    dataToSend = new ResponseLogin("", 1, "account doesn't exist");
                }

                await Task.Yield();

                if (isSuccess == true)
                {
                    // 기존 채팅 로그를 DB에서 불러옴
                    // List<ChattingLogDatabase> chattingLogList = await TM_DBHandler.Instance.GetChattingLogList();
                    // ResponseChattingLog dataToSendChattingLogList = new ResponseChattingLog(chattingLogList);
                    // string chattingLogListJson = dataToSendChattingLogList.AsJson();

                    // runner.SendMessageAll(todayMenuLogListJson);

                    // 기존 예약명단을 DB에서 불러옴
                    // List<TodayMenuDatabase> todayMenuLogList = await DBHandler.Instance.GetTodayMenuList();
                    // ResponseTodayMenuLog dataToSendTodayMenuLogList = new ResponseTodayMenuLog(todayMenuLogList);
                    // string todayMenuLogListJson = dataToSendTodayMenuLogList.AsJson();
                    
                    // runner.SendMessageTo(client, todayMenuLogListJson);
                    
                    // 접속자에게 접속했음을 알림
                    // SendAdminChatToAll(foundAccountList[0].user_name + "님이 접속하였습니다");
                    // connectedClientNameDic.Add(client, dataToSend.UserName);
                }

                string json = dataToSend.AsJson();
                serverRunner.SendMessageTo(client, json);
            }
            else if (type == ProtocolType.Logout_111)
            {
                RequestLogout requestedData = JsonConvert.DeserializeObject<RequestLogout>(message);
                Console.WriteLine(LOG_FORMAT, requestedData.ToString());

                ResponseLogout dataToSend = new ResponseLogout();
                string json = dataToSend.AsJson();

                serverRunner.SendMessageTo(client, json);
            }
        }
    }
}
