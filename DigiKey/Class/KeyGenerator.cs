using System;
using System.Security.Cryptography;
using System.Text;

namespace DigiKey.Class
{
    public class KeyGenerator
    {
        /// <summary>
        /// 私鑰
        /// </summary>
        private char[] Key = "DigiDental".ToCharArray();

        /// <summary>
        /// SHA384編碼
        /// </summary>
        /// <param name="source">input string</param>
        /// <returns></returns>
        public string SHA384Encode(string source)
        {
            string newSource = MixKey(source);
            SHA384 sha384 = new SHA384CryptoServiceProvider();
            byte[] tmpSource = Encoding.Default.GetBytes(newSource);
            byte[] crypto = sha384.ComputeHash(tmpSource);
            string result = Convert.ToBase64String(crypto);
            return result;
        }
        /// <summary>
        /// 將資料與私鑰混合
        /// </summary>
        /// <param name="source">input資料</param>
        /// <returns></returns>
        private string MixKey(string source)
        {
            string result = string.Empty;
            char[] caSource = source.ToCharArray();
            int len = source.Length;
            for (int i = 0; i < len; i++)
            {
                result += caSource[i].ToString();
                if (i < Key.Length)
                {
                    result += Key[i].ToString();
                }
            }
            return result;
        }
    }
}