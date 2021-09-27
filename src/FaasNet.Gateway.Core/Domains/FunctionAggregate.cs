using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class FunctionAggregate : ICloneable
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public static FunctionAggregate Create(string name, string image)
        {
            return new FunctionAggregate
            {
                CreateDateTime = DateTime.UtcNow,
                Image = image,
                Name = name,
                UpdateDateTime = DateTime.UtcNow
            };
        }

        public object Clone()
        {
            return new FunctionAggregate
            {
                Image = Image,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Name = Name
            };
        }
    }
}
