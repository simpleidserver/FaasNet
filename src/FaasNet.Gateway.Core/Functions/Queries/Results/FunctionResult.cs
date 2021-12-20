using FaasNet.Gateway.Core.Domains;
using System;

namespace FaasNet.Gateway.Core.Functions.Queries.Results
{
    public class FunctionResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Version { get; set; }
        public string Command { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public static FunctionResult ToDto(FunctionAggregate fn)
        {
            return new FunctionResult
            {
                Id = fn.Id,
                CreateDateTime = fn.CreateDateTime,
                Name = fn.Name,
                UpdateDateTime = fn.UpdateDateTime,
                Command = fn.Command,
                Image = fn.Image,
                Description = fn.Description,
                Version = fn.Version
            };
        }
    }
}
