using MySqlConnector;

namespace CCMSServer.Scripts.Databases.Structs
{
    public struct AccountDatabase
    {
        public const string TABLE_NAME = "ccms_account";

        public string user_id;
        public string user_name;
        public string encrypted_password;

        public string create_date;

        public AccountDatabase(MySqlDataReader reader)
        {
            user_id = reader["user_id"].ToString();
            user_name = reader["user_name"].ToString();
            encrypted_password = reader["encrypted_password"].ToString();

            create_date = reader["create_date"].ToString();
        }

        public AccountDatabase(string userID, string userName, string encryptedPassword, string createDate)
        {
            user_id = userID;
            user_name = userName;
            encrypted_password = encryptedPassword;
            create_date = createDate;
        }

        public string ToInsertQuery()
        {
            return "INSERT INTO " + TABLE_NAME + "(user_id,user_name,encrypted_password,create_date) " +
                   "VALUES(\'" + user_id + "\',\'" + user_name + "\',\'" + encrypted_password + "\',\'" + create_date + "\')";
        }
    }
}