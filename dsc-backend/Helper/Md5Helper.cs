using System.Security.Cryptography;
using System.Text;

namespace dsc_backend.Helper
{
    public class Md5Helper
    {
        public string MD5Hash(string pass)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(new UTF8Encoding().GetBytes(pass));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                stringBuilder.Append(result[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
