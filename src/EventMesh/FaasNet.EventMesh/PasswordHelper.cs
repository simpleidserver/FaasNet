using System.Security.Cryptography;
using System.Text;

namespace FaasNet.EventMesh
{
    public static class PasswordHelper
    {
        public static bool CheckPassword(string pwd, string clientSecret)
        {
            return ComputePassword(pwd) == clientSecret;
        }

        public static string ComputePassword(string pwd)
        {
            var payload = ASCIIEncoding.ASCII.GetBytes(pwd);
            using (var sha = SHA256.Create())
            {
                return ASCIIEncoding.ASCII.GetString(sha.ComputeHash(payload));
            }
        }
    }
}
