using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TN.TNM.DataAccess.Helper
{
    public static class EncrDecrCrypto
    {
        //private static RijndaelManaged myRijndael = new RijndaelManaged();
        private static string key = "secretmykeys2022";
        //private static int iterations = 1000;
        //private static byte[] salt = Encoding.UTF8.GetBytes(key);
        

        //public EncrDecrCrypto(string value)
        //{
        //    myRijndael.BlockSize = 128;
        //    myRijndael.KeySize = 128 / 8;
        //    myRijndael.IV = Encoding.UTF8.GetBytes(key); //HexStringToByteArray("e84ad660c4721ae0e84ad660c4721ae0");

        //    myRijndael.Padding = PaddingMode.PKCS7;
        //    myRijndael.Mode = CipherMode.CBC;
        //    iterations = 1000;
        //    salt = Encoding.UTF8.GetBytes(key);
        //    myRijndael.Key = GenerateKey(value);
        //}

        public static RijndaelManaged SetRijndaelManaged()
        {
            var myRijndael = new RijndaelManaged();

            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = Encoding.UTF8.GetBytes(key); //HexStringToByteArray("e84ad660c4721ae0e84ad660c4721ae0");

            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Key = Encoding.UTF8.GetBytes(key);

            return myRijndael;
        }

        public static string Encrypt(string strPlainText)
        {
            var myRijndael = SetRijndaelManaged();

            byte[] strText = new UTF8Encoding().GetBytes(strPlainText);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

            string hash = Convert.ToBase64String(cipherText);
            string result = Replace(hash, true);

            return result;
        }

        public static string Decrypt(string encryptedText)
        {
            var myRijndael = SetRijndaelManaged();

            string _encryptedText = Replace(encryptedText, false);

            byte[] encryptedBytes = Convert.FromBase64String(_encryptedText);
            var decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);
            byte[] originalBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            string hash = Encoding.UTF8.GetString(originalBytes);

            return hash;
        }

        //private static byte[] HexStringToByteArray(string strHex)
        //{
        //    dynamic r = new byte[strHex.Length / 2];
        //    for (int i = 0; i <= strHex.Length - 1; i += 2)
        //    {
        //        r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
        //    }
        //    return r;
        //}

        //private static byte[] GenerateKey(string strPassword)
        //{
        //    Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);

        //    return rfc2898.GetBytes(128 / 8);
        //}

        private static string Replace(string hash, bool mode)
        {
            string result = "";

            //Mã hóa
            if (mode)
            {
                result = hash.Replace("+", "--plus--")
                    .Replace(";", "--cp--")
                    .Replace("/", "--ch--")
                    .Replace("?", "--ho--")
                    .Replace(":", "--hc--")
                    .Replace("@", "--ac--")
                    .Replace("&", "--vv--")
                    .Replace("=", "--bb--")
                    .Replace("$", "--tt--")
                    .Replace(",", "--pp--");
            }
            //Giải mã
            else
            {
                result = hash.Replace("--plus--", "+")
                    .Replace("--cp--", ";")
                    .Replace("--ch--", "/")
                    .Replace("--ho--", "?")
                    .Replace("--hc--", ":")
                    .Replace("--ac--", "@")
                    .Replace("--vv--", "&")
                    .Replace("--bb--", "=")
                    .Replace("--tt--", "$")
                    .Replace("--pp--", ",");
            }

            return result;
        }
    }
}
