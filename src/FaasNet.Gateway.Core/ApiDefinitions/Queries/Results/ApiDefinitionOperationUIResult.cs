using FaasNet.Gateway.Core.Domains;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Results
{
    public class ApiDefinitionOperationUIResult
    {
        public string Html { get; set; }
        public IEnumerable<ApiDefinitionOperationBlockResult> Blocks { get; set; }
        public IEnumerable<ApiDefinitionOperationBlockUIResult> Blockarr { get; set; }

        public ApiDefinitionOperationUI ToDomain()
        {
            return new ApiDefinitionOperationUI
            {
                Html = Html,
                Blocks = Blocks?.Select(b => new ApiDefinitionOperationBlock
                {
                    Data = b.Data?.Select(d => new ApiDefinitionOperationBlockData
                    {
                        Name = d.Name,
                        Value = d.Value
                    }).ToList(),
                    ExternalId =  b.Id,
                    Model = b.Model == null ? null : new ApiDefinitionOperationBlockModel
                    {
                        Configuration = b.Model.Configuration,
                        Info = b.Model.Info == null ? null : new ApiDefinitionOperationBlockModelInfo
                        {
                            Name = b.Model.Info.Name
                        }
                    }
                }).ToList(),
                UIBlocks = Blockarr?.Select(b => new ApiDefinitionOperationBlockUI
                {
                    Childwidth = b.Childwidth,
                    Height = b.Height,
                    Id = b.Id,
                    Parent = b.Parent,
                    Width = b.Width,
                    X = b.X,
                    Y = b.Y
                }).ToList()
            };
        }

        public static ApiDefinitionOperationUIResult ToDto(ApiDefinitionOperationUI ui)
        {
            if (ui == null)
            {
                return null;
            }

            return new ApiDefinitionOperationUIResult
            {
                Html = ui.Html,
                Blockarr = ui.UIBlocks?.Select(s => new ApiDefinitionOperationBlockUIResult
                {
                    Childwidth = s.Childwidth,
                    Height = s.Height,
                    Id = s.Id,
                    Parent = s.Parent,
                    Width = s.Width,
                    X = s.X,
                    Y = s.Y 
                }),
                Blocks = ui.Blocks?.Select(s => new ApiDefinitionOperationBlockResult
                {
                    Id = s.ExternalId,
                    Parent = s.Parent,
                    Model = s.Model == null ? null : new ApiDefinitionOperationBlockModelResult
                    {
                        Configuration = s.Model.Configuration,
                        Info = s.Model.Info == null ? null : new ApiDefinitionOperationBlockModelInfoResult
                        {
                            Name = s.Model.Info.Name
                        }
                    },
                    Data = s.Data?.Select(d => new ApiDefinitionOperationBlockDataResult
                    {
                        Name = d.Name,
                        Value = d.Value
                    }).ToList(),
                })
            };
        }
    }
}
