using System;
using System.Security.Cryptography;
using System.Text;

namespace CCMSServer.Scripts.Utills
{
    internal static class ExtentionMethods
    {
        public static string ToEncryptAES(this string originalText, string key)
        {
            RijndaelManaged rijndaelCipher = GetRijndaelCipher(key);
            byte[] textBytes = Encoding.UTF8.GetBytes(originalText);

            string encrypted = Convert.ToBase64String(rijndaelCipher.CreateEncryptor().TransformFinalBlock(textBytes, 0, textBytes.Length));
            return encrypted;
        }

        public static string ToDecryptAES(this string targetText, string key)
        {
            RijndaelManaged rijndaelCipher = GetRijndaelCipher(key);
            byte[] encryptedData = Convert.FromBase64String(targetText);
            byte[] decryptedAsBytes = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(decryptedAsBytes);
        }

        private static RijndaelManaged GetRijndaelCipher(string key)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];

            int _length = passwordBytes.Length;
            if (_length > keyBytes.Length)
            {
                _length = keyBytes.Length;
            }
            Array.Copy(passwordBytes, keyBytes, _length);

            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }
    }
}
