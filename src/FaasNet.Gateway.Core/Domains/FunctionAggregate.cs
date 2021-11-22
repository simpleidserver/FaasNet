using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class FunctionAggregate : ICloneable
    {
        #region Properties

        public string Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string MetadataStr { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public JObject Metadata
        {
            get
            {
                if(string.IsNullOrWhiteSpace(MetadataStr))
                {
                    return null;
                }

                return JObject.Parse(MetadataStr);
            }
        }

        #endregion

        public static FunctionAggregate Create(string name, string provider, string metadataStr)
        {
            return new FunctionAggregate
            {
                Id = GenerateId(name),
                Provider = provider,
                CreateDateTime = DateTime.UtcNow,
                Name = name,
                MetadataStr = metadataStr,
                UpdateDateTime = DateTime.UtcNow
            };
        }

        public object Clone()
        {
            return new FunctionAggregate
            {
                Id = Id,
                Provider =  Provider,
                MetadataStr = MetadataStr,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Name = Name
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
