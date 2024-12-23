using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CCMSServer.Scripts.Databases.Structs;
using CCMSServer.Scripts.Configs.Structs;
using CCMSServer.Scripts.Configs;

namespace CCMSServer.Scripts.Databases
{
    internal class DBHandler
    {
        public const string ENCRYPTION_KEY = "개발조아"; // 비밀번호는 DB에 암호화하여 저장합니다. 암호화 및 복호화할 때의 키입니다.
        private const string LOG_FORMAT = "[DBHandler] {0}";

        protected static DBHandler _instance;
        public static DBHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBHandler();
                    _instance.Connect();
                }
                return _instance;
            }
        }

        protected MySqlConnection mySql;

        protected List<AccountDatabase> accountDBList = new List<AccountDatabase>();

        public virtual void Connect()
        {
            DatabaseConfig config = ConfigHandler.Instance.GetDatabaseConfig();
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder()
            {
                Server = config.IPAddress,
                UserID = config.UserID,
                Password = config.UserPassword,
                Database = config.DatabaseName,
            };

            try
            {
                mySql = new MySqlConnection(stringBuilder.ConnectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(LOG_FORMAT, e.Message);
            }
        }

        public virtual async Task<List<AccountDatabase>> GetAccountDataList()
        {
            await mySql.OpenAsync(new CancellationToken());

            string commandText = "SELECT * FROM " + AccountDatabase.TABLE_NAME;
            MySqlCommand command = new MySqlCommand(commandText, mySql);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read() == true)
            {
                AccountDatabase accountDB = new AccountDatabase(reader);
                accountDBList.Add(accountDB);
            }

            await mySql.CloseAsync();

            return accountDBList;
        }

        public virtual async Task<bool> TryInsertAccountData(string userName, string userID, string encryptedPassword, string createDate)
        {
            await mySql.OpenAsync(new CancellationToken());

            AccountDatabase newAccount = new AccountDatabase(userID, userName, encryptedPassword, createDate);
            MySqlCommand command = new MySqlCommand(newAccount.ToInsertQuery(), mySql);

            bool isCreated = command.ExecuteNonQuery() == 1;
            if (isCreated == true)
            {
                accountDBList.Add(newAccount);
            }

            await mySql.CloseAsync();
            return isCreated;
        }
    }
}
