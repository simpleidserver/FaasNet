using FaasNet.Common.Configuration;
using FaasNet.Gateway.Core.ApiDefinitions;
using FaasNet.Gateway.Core.ApiDefinitions.Commands;
using FaasNet.Gateway.Core.Functions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FaasNet.Gateway.Core.Configuration.Commands.Handlers
{
    public class ImportConfigurationCommandHandler : IRequestHandler<ImportConfigurationCommand, bool>
    {
        private readonly IFunctionService _functionService;
        private readonly IApiDefinitionService _apiDefinitionService;

        public ImportConfigurationCommandHandler(
            IFunctionService functionService,
            IApiDefinitionService apiDefinitionService)
        {
            _functionService = functionService;
            _apiDefinitionService = apiDefinitionService;
        }

        public async Task<bool> Handle(ImportConfigurationCommand request, CancellationToken cancellationToken)
        {
            var configuration = Extract(request);
            await PublishFunctions(configuration, cancellationToken);
            await PublishApiDefinitions(configuration, cancellationToken);
            return true;
        }

        protected FaasConfiguration Extract(ImportConfigurationCommand request)
        {
            var yml = Encoding.UTF8.GetString(Convert.FromBase64String(request.SerializedConfigurationFile));
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<FaasConfiguration>(yml);
        }

        protected async Task PublishFunctions(FaasConfiguration configuration, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<bool>>();
            foreach(var fn in configuration.Functions)
            {
                tasks.Add(_functionService.Publish(fn.Name, fn.Image, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        protected async Task PublishApiDefinitions(FaasConfiguration configuration, CancellationToken cancellationToken)
        {
            var commands = configuration.Apis.Select(kvp =>
            {
                return new UpdateApiDefinitionCommand
                {
                    Name = kvp.Key,
                    Path = kvp.Value.Path,
                    Operations = kvp.Value.Operations.Select(op =>
                    {
                        return new ReplaceApiOperation
                        {
                            Name = op.Name,
                            Path = op.Path,
                            Functions = op.Functions.Select(fn => new ReplaceApiFunction
                            {
                                Name = fn.Name,
                                Function = fn.Function,
                                SerializedConfiguration = fn.Configuration,
                                Flows = fn.Flows.Select(fl => new ReplaceApiSequenceFlow
                                {
                                    TargetRef = fl.Next
                                }).ToList()
                            }).ToList()
                        };
                    }).ToList()
                };
            });
            var tasks = new List<Task<bool>>();
            foreach(var cmd in commands)
            {
                tasks.Add(_apiDefinitionService.Replace(cmd, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
    }
}
