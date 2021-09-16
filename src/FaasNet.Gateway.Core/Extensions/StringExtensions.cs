namespace FaasNet.Gateway.Core.Extensions
{
    public static class StringExtensions
    {
        public static string CleanPath(this string str)
        {
            return str.TrimStart('/');
        }
    }
}
