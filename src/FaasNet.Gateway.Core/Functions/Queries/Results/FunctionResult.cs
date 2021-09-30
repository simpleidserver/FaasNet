using FaasNet.Gateway.Core.Domains;
using System;

namespace FaasNet.Gateway.Core.Functions.Queries.Results
{
    public class FunctionResult
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public static FunctionResult ToDto(FunctionAggregate fn)
        {
            return new FunctionResult
            {
                Image = fn.Image,
                CreateDateTime = fn.CreateDateTime,
                Name = fn.Name,
                UpdateDateTime = fn.UpdateDateTime
            };
        }
    }
}
