using CCMSServer.Scripts.Databases;
using CCMSServer.Scripts.Utills;
using Newtonsoft.Json;

namespace CCMSServer.Scripts
{
    public enum ProtocolType
    {
        none = -1,

        Connected_0 = 0,
        Disconnected_1 = 1,

        CreateAccount_100 = 100,
        IsCreatableAccount_101 = 101,

        Login_110 = 110,
        Logout_111 = 111
    }

    public class BaseTCPSocket
    {
        public ProtocolType _ProtocolType = ProtocolType.none;

        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class RequestCreateAccount : BaseTCPSocket
    {
        public string UserName;
        public string UserID;
        public string Password;
        public string CreateDateTime;

        public RequestCreateAccount(string userName, string userID, string password, string createDateTime)
        {
            _ProtocolType = ProtocolType.CreateAccount_100;

            UserName = userName;
            UserID = userID;
            Password = password;
            CreateDateTime = createDateTime;
        }

        public override string ToString()
        {
            return "[ UserName : " + UserName + " / UserID : " + UserID + " / Password : " + Password.ToEncryptAES(DBHandler.ENCRYPTION_KEY) + " / CreateDateTime : " + CreateDateTime + " / _ProtocolType : <color=yellow>" + _ProtocolType + "</color> ]";
        }
    }

    public class RequestIsCreatableAccount : BaseTCPSocket
    {
        public string UserID;
        public string Password;

        public RequestIsCreatableAccount(string userID, string password)
        {
            _ProtocolType = ProtocolType.IsCreatableAccount_101;

            UserID = userID;
            Password = password;
        }

        public override string ToString()
        {
            return "[ UserID : " + UserID + " / Password : " + Password + " / _ProtocolType : <color=yellow>" + _ProtocolType + "</color> ]";
        }
    }

    public class RequestLogin : BaseTCPSocket
    {
        public string UserID;
        public string Password; // Decrypted

        public RequestLogin(string userID, string password)
        {
            _ProtocolType = ProtocolType.Login_110;

            UserID = userID;
            Password = password;
        }

        public override string ToString()
        {
            return "[ UserID : " + UserID + " / Password : " + Password.ToEncryptAES(DBHandler.ENCRYPTION_KEY) + " / _ProtocolType : <color=yellow>" + _ProtocolType + "</color> ]";
        }
    }

    public class RequestLogout : BaseTCPSocket
    {
        public string UserName;
        public string UserID;

        public RequestLogout(string userID, string userName)
        {
            _ProtocolType = ProtocolType.Logout_111;

            UserID = userID;
            UserName = userName;
        }

        public override string ToString()
        {
            return "[ UserID : " + UserID + " / _ProtocolType : <color=yellow>" + _ProtocolType + "</color> ]";
        }
    }

    public class ResponseCreateAccount : BaseTCPSocket
    {
        public int Result; // -1 => 실패 (이미 있는 계정 or DB 문제)
                           // 0 => 성공

        public string Message;

        public ResponseCreateAccount(int result, string message)
        {
            _ProtocolType = ProtocolType.CreateAccount_100;

            Result = result;
            Message = message;
        }
    }

    public class ResponseLogin : BaseTCPSocket
    {
        public string UserName;
        public int Result; // -1 => 실패 (없는 계정 정보)
                           // 0 => 성공
        public string Message;

        public ResponseLogin(string userName, int result, string message)
        {
            _ProtocolType = ProtocolType.Login_110;

            UserName = userName;
            Result = result;
            Message = message;
        }
    }

    public class ResponseLogout : BaseTCPSocket
    {
        public ResponseLogout()
        {
            _ProtocolType = ProtocolType.Logout_111;
        }
    }
}