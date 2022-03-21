﻿using FaasNet.EventMesh.Runtime.Models;
using System;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Results
{
    public class ApplicationDomainResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public static ApplicationDomainResult Build(ApplicationDomain appDomain)
        {
            return new ApplicationDomainResult
            {
                Id = appDomain.Id,
                CreateDateTime = appDomain.CreateDateTime,
                Description = appDomain.Description,
                Name = appDomain.Name,
                RootTopic = appDomain.RootTopic,
                UpdateDateTime = appDomain.UpdateDateTime
            };
        }
    }
}
