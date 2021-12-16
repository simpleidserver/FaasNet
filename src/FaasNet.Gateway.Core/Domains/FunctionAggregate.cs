using System;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class FunctionAggregate : ICloneable
    {
        #region Properties

        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Command { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        #endregion

        public static FunctionAggregate Create(string name, string image, string command)
        {
            return new FunctionAggregate
            {
                Id = GenerateId(name),
                CreateDateTime = DateTime.UtcNow,
                Name = name,
                UpdateDateTime = DateTime.UtcNow,
                Image = image,
                Command = command
            };
        }

        public object Clone()
        {
            return new FunctionAggregate
            {
                Id = Id,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Name = Name,
                Command = Command,
                Image = Image
            };
        }

        private static string GenerateId(string name)
        {
            return $"{name}-{RandomString(10)}-{RandomString(5)}";
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLowerInvariant();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
