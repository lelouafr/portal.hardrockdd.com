using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DB.Infrastructure.Services
{
#pragma warning disable SYSLIB0022 // Type or member is obsolete
    public static class AESEncryption
    {
        private const string secretKey = "qX3YY/Fqs4A93kDy7N2wd64PWPdgTiAkzciiIvcWpDM=";
        #region Static Functions

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="PlainText">Text to be encrypted</param>
        /// <param name="Password">Password to encrypt with</param>
        /// <returns>An encrypted string</returns>
        public static string Encrypt(string PlainText, string Password = "")
        {
            string Salt = "Kosher";
            string HashAlgorithm = "SHA256";
            int PasswordIterations = 100;
            string InitialVector = "OFRna73m*aze01xY";
            int KeySize = 256;
            if (string.IsNullOrEmpty(Password))
            {
                Password = secretKey;
            }
            if (string.IsNullOrEmpty(PlainText))
                return "";
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            var SymmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };
            byte[]? CipherTextBytes = null;
            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {
                using MemoryStream MemStream = new MemoryStream();
                using CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);
                CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
                CryptoStream.FlushFinalBlock();
                CipherTextBytes = MemStream.ToArray();
                MemStream.Close();
                CryptoStream.Close();
            }
            SymmetricKey.Clear();
            return Convert.ToBase64String(CipherTextBytes);
        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="CipherText">Text to be decrypted</param>
        /// <param name="Password">Password to decrypt with</param>
        /// <returns>A decrypted string</returns>
        public static string Decrypt(string? CipherText, string Password = "")
        {
            if (string.IsNullOrEmpty(CipherText))
                return string.Empty;

            if (string.IsNullOrEmpty(Password))
            {
                Password = secretKey;
            }
            try
            {
                string Salt = "Kosher";
                string HashAlgorithm = "SHA256";
                int PasswordIterations = 100;
                string InitialVector = "OFRna73m*aze01xY";
                int KeySize = 256;

                if (string.IsNullOrEmpty(CipherText))
                    return "";
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
                byte[] CipherTextBytes = Convert.FromBase64String(CipherText);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
                RijndaelManaged SymmetricKey = new RijndaelManaged()
                {
                    Mode = CipherMode.CBC
                };
                byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
                int ByteCount = 0;
                using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                        {

                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }
                SymmetricKey.Clear();
                return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
            }
            catch (Exception)
            {
                return CipherText;
            }
        }

        #endregion
    }
#pragma warning restore SYSLIB0022 // Type or member is obsolete
}
