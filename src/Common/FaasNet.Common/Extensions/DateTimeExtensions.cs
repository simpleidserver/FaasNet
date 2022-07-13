using System;

namespace FaasNet.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetUnixTimeSeconds(this DateTime dateTime)
        {
            var t = dateTime - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
